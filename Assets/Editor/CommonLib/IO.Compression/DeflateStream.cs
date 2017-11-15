using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;

namespace Unity.IO.Compression
{
	public class DeflateStream : Stream
	{
		internal delegate void AsyncWriteDelegate(byte[] array, int offset, int count, bool isAsync);

		private enum WorkerType : byte
		{
			Managed,
			Unknown
		}

		private class CachedBuffer
		{
			public byte[] buffer = new byte[8192];
		}

		internal const int DefaultBufferSize = 8192;

		private Stream _stream;

		private CompressionMode _mode;

		private bool _leaveOpen;

		private Inflater inflater;

		private IDeflater deflater;

		private byte[] buffer;

		private int asyncOperations;

		private readonly AsyncCallback m_CallBack;

		private readonly DeflateStream.AsyncWriteDelegate m_AsyncWriterDelegate;

		private IFileFormatWriter formatWriter;

		private bool wroteHeader;

		private bool wroteBytes;

		private const int kMaxCachedBufferPoolSize = 4;

		private static DeflateStream.CachedBuffer[] s_bufferPool = new DeflateStream.CachedBuffer[4];

		private static int s_fetchIndex = 0;

		public Stream BaseStream
		{
			get
			{
				return this._stream;
			}
		}

		public override bool CanRead
		{
			get
			{
				bool flag = this._stream == null;
				return !flag && this._mode == CompressionMode.Decompress && this._stream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				bool flag = this._stream == null;
				return !flag && this._mode == CompressionMode.Compress && this._stream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException(SR.GetString("Not supported"));
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException(SR.GetString("Not supported"));
			}
			set
			{
				throw new NotSupportedException(SR.GetString("Not supported"));
			}
		}

		private static byte[] FetchCachedBuffer()
		{
			bool flag = DeflateStream.s_bufferPool[DeflateStream.s_fetchIndex] == null;
			DeflateStream.CachedBuffer cachedBuffer;
			if (flag)
			{
				cachedBuffer = new DeflateStream.CachedBuffer();
				DeflateStream.s_bufferPool[DeflateStream.s_fetchIndex] = cachedBuffer;
			}
			else
			{
				cachedBuffer = DeflateStream.s_bufferPool[DeflateStream.s_fetchIndex];
			}
			DeflateStream.s_fetchIndex = (DeflateStream.s_fetchIndex + 1) % 4;
			return cachedBuffer.buffer;
		}

		public DeflateStream(Stream stream, CompressionMode mode) : this(stream, mode, false)
		{
		}

		public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = CompressionMode.Compress != mode && mode > CompressionMode.Decompress;
			if (flag2)
			{
				throw new ArgumentException(SR.GetString("Argument out of range"), "mode");
			}
			this._stream = stream;
			this._mode = mode;
			this._leaveOpen = leaveOpen;
			CompressionMode mode2 = this._mode;
			if (mode2 != CompressionMode.Decompress)
			{
				if (mode2 == CompressionMode.Compress)
				{
					bool flag3 = !this._stream.CanWrite;
					if (flag3)
					{
						throw new ArgumentException(SR.GetString("Not a writeable stream"), "stream");
					}
					this.deflater = DeflateStream.CreateDeflater();
					this.m_AsyncWriterDelegate = new DeflateStream.AsyncWriteDelegate(this.InternalWrite);
					this.m_CallBack = new AsyncCallback(this.WriteCallback);
				}
			}
			else
			{
				bool flag4 = !this._stream.CanRead;
				if (flag4)
				{
					throw new ArgumentException(SR.GetString("Not a readable stream"), "stream");
				}
				this.inflater = new Inflater();
				this.m_CallBack = new AsyncCallback(this.ReadCallback);
			}
			this.buffer = DeflateStream.FetchCachedBuffer();
		}

		private static IDeflater CreateDeflater()
		{
			DeflateStream.WorkerType deflaterType = DeflateStream.GetDeflaterType();
			if (deflaterType != DeflateStream.WorkerType.Managed)
			{
				throw new SystemException("Program entered an unexpected state.");
			}
			return new DeflaterManaged();
		}

		[SecuritySafeCritical]
		private static DeflateStream.WorkerType GetDeflaterType()
		{
			return DeflateStream.WorkerType.Managed;
		}

		internal void SetFileFormatReader(IFileFormatReader reader)
		{
			bool flag = reader != null;
			if (flag)
			{
				this.inflater.SetFileFormatReader(reader);
			}
		}

		internal void SetFileFormatWriter(IFileFormatWriter writer)
		{
			bool flag = writer != null;
			if (flag)
			{
				this.formatWriter = writer;
			}
		}

		public override void Flush()
		{
			this.EnsureNotDisposed();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException(SR.GetString("Not supported"));
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(SR.GetString("Not supported"));
		}

		public override int Read(byte[] array, int offset, int count)
		{
			this.EnsureDecompressionMode();
			this.ValidateParameters(array, offset, count);
			this.EnsureNotDisposed();
			int num = offset;
			int num2 = count;
			while (true)
			{
				int num3 = this.inflater.Inflate(array, num, num2);
				num += num3;
				num2 -= num3;
				bool flag = num2 == 0;
				if (flag)
				{
					break;
				}
				bool flag2 = this.inflater.Finished();
				if (flag2)
				{
					break;
				}
				int num4 = this._stream.Read(this.buffer, 0, this.buffer.Length);
				bool flag3 = num4 == 0;
				if (flag3)
				{
					break;
				}
				this.inflater.SetInput(this.buffer, 0, num4);
			}
			return count - num2;
		}

		private void ValidateParameters(byte[] array, int offset, int count)
		{
			bool flag = array == null;
			if (flag)
			{
				throw new ArgumentNullException("array");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			bool flag4 = array.Length - offset < count;
			if (flag4)
			{
				throw new ArgumentException(SR.GetString("Invalid argument offset count"));
			}
		}

		private void EnsureNotDisposed()
		{
			bool flag = this._stream == null;
			if (flag)
			{
				throw new ObjectDisposedException(null, SR.GetString("Object disposed"));
			}
		}

		private void EnsureDecompressionMode()
		{
			bool flag = this._mode > CompressionMode.Decompress;
			if (flag)
			{
				throw new InvalidOperationException(SR.GetString("Cannot read from deflate stream"));
			}
		}

		private void EnsureCompressionMode()
		{
			bool flag = this._mode != CompressionMode.Compress;
			if (flag)
			{
				throw new InvalidOperationException(SR.GetString("Cannot write to deflate stream"));
			}
		}

		public override IAsyncResult BeginRead(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			this.EnsureDecompressionMode();
			bool flag = this.asyncOperations != 0;
			if (flag)
			{
				throw new InvalidOperationException(SR.GetString("Invalid begin call"));
			}
			this.ValidateParameters(array, offset, count);
			this.EnsureNotDisposed();
			Interlocked.Increment(ref this.asyncOperations);
			IAsyncResult result;
			try
			{
				DeflateStreamAsyncResult deflateStreamAsyncResult = new DeflateStreamAsyncResult(this, asyncState, asyncCallback, array, offset, count);
				deflateStreamAsyncResult.isWrite = false;
				int num = this.inflater.Inflate(array, offset, count);
				bool flag2 = num != 0;
				if (flag2)
				{
					deflateStreamAsyncResult.InvokeCallback(true, num);
					result = deflateStreamAsyncResult;
				}
				else
				{
					bool flag3 = this.inflater.Finished();
					if (flag3)
					{
						deflateStreamAsyncResult.InvokeCallback(true, 0);
						result = deflateStreamAsyncResult;
					}
					else
					{
						this._stream.BeginRead(this.buffer, 0, this.buffer.Length, this.m_CallBack, deflateStreamAsyncResult);
						deflateStreamAsyncResult.m_CompletedSynchronously &= deflateStreamAsyncResult.IsCompleted;
						result = deflateStreamAsyncResult;
					}
				}
			}
			catch
			{
				Interlocked.Decrement(ref this.asyncOperations);
				throw;
			}
			return result;
		}

		private void ReadCallback(IAsyncResult baseStreamResult)
		{
			DeflateStreamAsyncResult deflateStreamAsyncResult = (DeflateStreamAsyncResult)baseStreamResult.AsyncState;
			deflateStreamAsyncResult.m_CompletedSynchronously &= baseStreamResult.CompletedSynchronously;
			try
			{
				this.EnsureNotDisposed();
				int num = this._stream.EndRead(baseStreamResult);
				bool flag = num <= 0;
				if (flag)
				{
					deflateStreamAsyncResult.InvokeCallback(0);
				}
				else
				{
					this.inflater.SetInput(this.buffer, 0, num);
					num = this.inflater.Inflate(deflateStreamAsyncResult.buffer, deflateStreamAsyncResult.offset, deflateStreamAsyncResult.count);
					bool flag2 = num == 0 && !this.inflater.Finished();
					if (flag2)
					{
						this._stream.BeginRead(this.buffer, 0, this.buffer.Length, this.m_CallBack, deflateStreamAsyncResult);
					}
					else
					{
						deflateStreamAsyncResult.InvokeCallback(num);
					}
				}
			}
			catch (Exception result)
			{
				deflateStreamAsyncResult.InvokeCallback(result);
			}
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			this.EnsureDecompressionMode();
			this.CheckEndXxxxLegalStateAndParams(asyncResult);
			DeflateStreamAsyncResult deflateStreamAsyncResult = (DeflateStreamAsyncResult)asyncResult;
			this.AwaitAsyncResultCompletion(deflateStreamAsyncResult);
			Exception ex = deflateStreamAsyncResult.Result as Exception;
			bool flag = ex != null;
			if (flag)
			{
				throw ex;
			}
			return (int)deflateStreamAsyncResult.Result;
		}

		public override void Write(byte[] array, int offset, int count)
		{
			this.EnsureCompressionMode();
			this.ValidateParameters(array, offset, count);
			this.EnsureNotDisposed();
			this.InternalWrite(array, offset, count, false);
		}

		internal void InternalWrite(byte[] array, int offset, int count, bool isAsync)
		{
			this.DoMaintenance(array, offset, count);
			this.WriteDeflaterOutput(isAsync);
			this.deflater.SetInput(array, offset, count);
			this.WriteDeflaterOutput(isAsync);
		}

		private void WriteDeflaterOutput(bool isAsync)
		{
			while (!this.deflater.NeedsInput())
			{
				int deflateOutput = this.deflater.GetDeflateOutput(this.buffer);
				bool flag = deflateOutput > 0;
				if (flag)
				{
					this.DoWrite(this.buffer, 0, deflateOutput, isAsync);
				}
			}
		}

		private void DoWrite(byte[] array, int offset, int count, bool isAsync)
		{
			Debug.Assert(array != null);
			Debug.Assert(count != 0);
			if (isAsync)
			{
				IAsyncResult asyncResult = this._stream.BeginWrite(array, offset, count, null, null);
				this._stream.EndWrite(asyncResult);
			}
			else
			{
				this._stream.Write(array, offset, count);
			}
		}

		private void DoMaintenance(byte[] array, int offset, int count)
		{
			bool flag = count <= 0;
			if (!flag)
			{
				this.wroteBytes = true;
				bool flag2 = this.formatWriter == null;
				if (!flag2)
				{
					bool flag3 = !this.wroteHeader;
					if (flag3)
					{
						byte[] header = this.formatWriter.GetHeader();
						this._stream.Write(header, 0, header.Length);
						this.wroteHeader = true;
					}
					this.formatWriter.UpdateWithBytesRead(array, offset, count);
				}
			}
		}

		private void PurgeBuffers(bool disposing)
		{
			bool flag = !disposing;
			if (!flag)
			{
				bool flag2 = this._stream == null;
				if (!flag2)
				{
					this.Flush();
					bool flag3 = this._mode != CompressionMode.Compress;
					if (!flag3)
					{
						bool flag4 = this.wroteBytes;
						if (flag4)
						{
							this.WriteDeflaterOutput(false);
							bool flag5;
							do
							{
								int num;
								flag5 = this.deflater.Finish(this.buffer, out num);
								bool flag6 = num > 0;
								if (flag6)
								{
									this.DoWrite(this.buffer, 0, num, false);
								}
							}
							while (!flag5);
						}
						bool flag7 = this.formatWriter != null && this.wroteHeader;
						if (flag7)
						{
							byte[] footer = this.formatWriter.GetFooter();
							this._stream.Write(footer, 0, footer.Length);
						}
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				this.PurgeBuffers(disposing);
			}
			finally
			{
				try
				{
					bool flag = disposing && !this._leaveOpen && this._stream != null;
					if (flag)
					{
						this._stream.Dispose();
					}
				}
				finally
				{
					this._stream = null;
					try
					{
						bool flag2 = this.deflater != null;
						if (flag2)
						{
							this.deflater.Dispose();
						}
					}
					finally
					{
						this.deflater = null;
						base.Dispose(disposing);
					}
				}
			}
		}

		public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			this.EnsureCompressionMode();
			bool flag = this.asyncOperations != 0;
			if (flag)
			{
				throw new InvalidOperationException(SR.GetString("Invalid begin call"));
			}
			this.ValidateParameters(array, offset, count);
			this.EnsureNotDisposed();
			Interlocked.Increment(ref this.asyncOperations);
			IAsyncResult result;
			try
			{
				DeflateStreamAsyncResult deflateStreamAsyncResult = new DeflateStreamAsyncResult(this, asyncState, asyncCallback, array, offset, count);
				deflateStreamAsyncResult.isWrite = true;
				this.m_AsyncWriterDelegate.BeginInvoke(array, offset, count, true, this.m_CallBack, deflateStreamAsyncResult);
				deflateStreamAsyncResult.m_CompletedSynchronously &= deflateStreamAsyncResult.IsCompleted;
				result = deflateStreamAsyncResult;
			}
			catch
			{
				Interlocked.Decrement(ref this.asyncOperations);
				throw;
			}
			return result;
		}

		private void WriteCallback(IAsyncResult asyncResult)
		{
			DeflateStreamAsyncResult deflateStreamAsyncResult = (DeflateStreamAsyncResult)asyncResult.AsyncState;
			deflateStreamAsyncResult.m_CompletedSynchronously &= asyncResult.CompletedSynchronously;
			try
			{
				this.m_AsyncWriterDelegate.EndInvoke(asyncResult);
			}
			catch (Exception result)
			{
				deflateStreamAsyncResult.InvokeCallback(result);
				return;
			}
			deflateStreamAsyncResult.InvokeCallback(null);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			this.EnsureCompressionMode();
			this.CheckEndXxxxLegalStateAndParams(asyncResult);
			DeflateStreamAsyncResult deflateStreamAsyncResult = (DeflateStreamAsyncResult)asyncResult;
			this.AwaitAsyncResultCompletion(deflateStreamAsyncResult);
			Exception ex = deflateStreamAsyncResult.Result as Exception;
			bool flag = ex != null;
			if (flag)
			{
				throw ex;
			}
		}

		private void CheckEndXxxxLegalStateAndParams(IAsyncResult asyncResult)
		{
			bool flag = this.asyncOperations != 1;
			if (flag)
			{
				throw new InvalidOperationException(SR.GetString("Invalid end call"));
			}
			bool flag2 = asyncResult == null;
			if (flag2)
			{
				throw new ArgumentNullException("asyncResult");
			}
			this.EnsureNotDisposed();
			DeflateStreamAsyncResult deflateStreamAsyncResult = asyncResult as DeflateStreamAsyncResult;
			bool flag3 = deflateStreamAsyncResult == null;
			if (flag3)
			{
				throw new ArgumentNullException("asyncResult");
			}
		}

		private void AwaitAsyncResultCompletion(DeflateStreamAsyncResult asyncResult)
		{
			try
			{
				bool flag = !asyncResult.IsCompleted;
				if (flag)
				{
					asyncResult.AsyncWaitHandle.WaitOne();
				}
			}
			finally
			{
				Interlocked.Decrement(ref this.asyncOperations);
				asyncResult.Close();
			}
		}
	}
}
