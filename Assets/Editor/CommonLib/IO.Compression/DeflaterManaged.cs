using System;
using System.Diagnostics;

namespace Unity.IO.Compression
{
	internal class DeflaterManaged : IDeflater, IDisposable
	{
		private enum DeflaterState
		{
			NotStarted,
			SlowDownForIncompressible1,
			SlowDownForIncompressible2,
			StartingSmallData,
			CompressThenCheck,
			CheckingForIncompressible,
			HandlingSmallData
		}

		private const int MinBlockSize = 256;

		private const int MaxHeaderFooterGoo = 120;

		private const int CleanCopySize = 8072;

		private const double BadCompressionThreshold = 1.0;

		private FastEncoder deflateEncoder;

		private CopyEncoder copyEncoder;

		private DeflateInput input;

		private OutputBuffer output;

		private DeflaterManaged.DeflaterState processingState;

		private DeflateInput inputFromHistory;

		internal DeflaterManaged()
		{
			this.deflateEncoder = new FastEncoder();
			this.copyEncoder = new CopyEncoder();
			this.input = new DeflateInput();
			this.output = new OutputBuffer();
			this.processingState = DeflaterManaged.DeflaterState.NotStarted;
		}

		private bool NeedsInput()
		{
			return ((IDeflater)this).NeedsInput();
		}

		bool IDeflater.NeedsInput()
		{
			return this.input.Count == 0 && this.deflateEncoder.BytesInHistory == 0;
		}

		void IDeflater.SetInput(byte[] inputBuffer, int startIndex, int count)
		{
			Debug.Assert(this.input.Count == 0, "We have something left in previous input!");
			this.input.Buffer = inputBuffer;
			this.input.Count = count;
			this.input.StartIndex = startIndex;
			bool flag = count > 0 && count < 256;
			if (flag)
			{
				DeflaterManaged.DeflaterState deflaterState = this.processingState;
				if (deflaterState != DeflaterManaged.DeflaterState.NotStarted)
				{
					if (deflaterState == DeflaterManaged.DeflaterState.CompressThenCheck)
					{
						this.processingState = DeflaterManaged.DeflaterState.HandlingSmallData;
						goto IL_7F;
					}
					if (deflaterState != DeflaterManaged.DeflaterState.CheckingForIncompressible)
					{
						goto IL_7F;
					}
				}
				this.processingState = DeflaterManaged.DeflaterState.StartingSmallData;
				IL_7F:;
			}
		}

		int IDeflater.GetDeflateOutput(byte[] outputBuffer)
		{
			Debug.Assert(outputBuffer != null, "Can't pass in a null output buffer!");
			Debug.Assert(!this.NeedsInput(), "GetDeflateOutput should only be called after providing input");
			this.output.UpdateBuffer(outputBuffer);
			switch (this.processingState)
			{
			case DeflaterManaged.DeflaterState.NotStarted:
			{
				Debug.Assert(this.deflateEncoder.BytesInHistory == 0, "have leftover bytes in window");
				DeflateInput.InputState state = this.input.DumpState();
				OutputBuffer.BufferState state2 = this.output.DumpState();
				this.deflateEncoder.GetBlockHeader(this.output);
				this.deflateEncoder.GetCompressedData(this.input, this.output);
				bool flag = !this.UseCompressed(this.deflateEncoder.LastCompressionRatio);
				if (flag)
				{
					this.input.RestoreState(state);
					this.output.RestoreState(state2);
					this.copyEncoder.GetBlock(this.input, this.output, false);
					this.FlushInputWindows();
					this.processingState = DeflaterManaged.DeflaterState.CheckingForIncompressible;
				}
				else
				{
					this.processingState = DeflaterManaged.DeflaterState.CompressThenCheck;
				}
				goto IL_2D4;
			}
			case DeflaterManaged.DeflaterState.SlowDownForIncompressible1:
				this.deflateEncoder.GetBlockFooter(this.output);
				this.processingState = DeflaterManaged.DeflaterState.SlowDownForIncompressible2;
				break;
			case DeflaterManaged.DeflaterState.SlowDownForIncompressible2:
				break;
			case DeflaterManaged.DeflaterState.StartingSmallData:
				this.deflateEncoder.GetBlockHeader(this.output);
				this.processingState = DeflaterManaged.DeflaterState.HandlingSmallData;
				goto IL_2B9;
			case DeflaterManaged.DeflaterState.CompressThenCheck:
			{
				this.deflateEncoder.GetCompressedData(this.input, this.output);
				bool flag2 = !this.UseCompressed(this.deflateEncoder.LastCompressionRatio);
				if (flag2)
				{
					this.processingState = DeflaterManaged.DeflaterState.SlowDownForIncompressible1;
					this.inputFromHistory = this.deflateEncoder.UnprocessedInput;
				}
				goto IL_2D4;
			}
			case DeflaterManaged.DeflaterState.CheckingForIncompressible:
			{
				Debug.Assert(this.deflateEncoder.BytesInHistory == 0, "have leftover bytes in window");
				DeflateInput.InputState state3 = this.input.DumpState();
				OutputBuffer.BufferState state4 = this.output.DumpState();
				this.deflateEncoder.GetBlock(this.input, this.output, 8072);
				bool flag3 = !this.UseCompressed(this.deflateEncoder.LastCompressionRatio);
				if (flag3)
				{
					this.input.RestoreState(state3);
					this.output.RestoreState(state4);
					this.copyEncoder.GetBlock(this.input, this.output, false);
					this.FlushInputWindows();
				}
				goto IL_2D4;
			}
			case DeflaterManaged.DeflaterState.HandlingSmallData:
				goto IL_2B9;
			default:
				goto IL_2D4;
			}
			bool flag4 = this.inputFromHistory.Count > 0;
			if (flag4)
			{
				this.copyEncoder.GetBlock(this.inputFromHistory, this.output, false);
			}
			bool flag5 = this.inputFromHistory.Count == 0;
			if (flag5)
			{
				this.deflateEncoder.FlushInput();
				this.processingState = DeflaterManaged.DeflaterState.CheckingForIncompressible;
			}
			goto IL_2D4;
			IL_2B9:
			this.deflateEncoder.GetCompressedData(this.input, this.output);
			IL_2D4:
			return this.output.BytesWritten;
		}

		bool IDeflater.Finish(byte[] outputBuffer, out int bytesRead)
		{
			Debug.Assert(outputBuffer != null, "Can't pass in a null output buffer!");
			Debug.Assert(this.processingState == DeflaterManaged.DeflaterState.NotStarted || this.processingState == DeflaterManaged.DeflaterState.CheckingForIncompressible || this.processingState == DeflaterManaged.DeflaterState.HandlingSmallData || this.processingState == DeflaterManaged.DeflaterState.CompressThenCheck || this.processingState == DeflaterManaged.DeflaterState.SlowDownForIncompressible1, "got unexpected processing state = " + this.processingState);
			Debug.Assert(this.NeedsInput());
			bool flag = this.processingState == DeflaterManaged.DeflaterState.NotStarted;
			bool result;
			if (flag)
			{
				bytesRead = 0;
				result = true;
			}
			else
			{
				this.output.UpdateBuffer(outputBuffer);
				bool flag2 = this.processingState == DeflaterManaged.DeflaterState.CompressThenCheck || this.processingState == DeflaterManaged.DeflaterState.HandlingSmallData || this.processingState == DeflaterManaged.DeflaterState.SlowDownForIncompressible1;
				if (flag2)
				{
					this.deflateEncoder.GetBlockFooter(this.output);
				}
				this.WriteFinal();
				bytesRead = this.output.BytesWritten;
				result = true;
			}
			return result;
		}

		void IDisposable.Dispose()
		{
		}

		protected void Dispose(bool disposing)
		{
		}

		private bool UseCompressed(double ratio)
		{
			return ratio <= 1.0;
		}

		private void FlushInputWindows()
		{
			this.deflateEncoder.FlushInput();
		}

		private void WriteFinal()
		{
			this.copyEncoder.GetBlock(null, this.output, true);
		}
	}
}
