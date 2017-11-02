using System;
using System.IO;

namespace Unity.IO.Compression
{
	public class GZipStream : Stream
	{
		private DeflateStream deflateStream;

		public override bool CanRead
		{
			get
			{
				bool flag = this.deflateStream == null;
				return !flag && this.deflateStream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				bool flag = this.deflateStream == null;
				return !flag && this.deflateStream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				bool flag = this.deflateStream == null;
				return !flag && this.deflateStream.CanSeek;
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

		public Stream BaseStream
		{
			get
			{
				bool flag = this.deflateStream != null;
				Stream result;
				if (flag)
				{
					result = this.deflateStream.BaseStream;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public GZipStream(Stream stream, CompressionMode mode) : this(stream, mode, false)
		{
		}

		public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
		{
			this.deflateStream = new DeflateStream(stream, mode, leaveOpen);
			this.SetDeflateStreamFileFormatter(mode);
		}

		private void SetDeflateStreamFileFormatter(CompressionMode mode)
		{
			bool flag = mode == CompressionMode.Compress;
			if (flag)
			{
				IFileFormatWriter fileFormatWriter = new GZipFormatter();
				this.deflateStream.SetFileFormatWriter(fileFormatWriter);
			}
			else
			{
				IFileFormatReader fileFormatReader = new GZipDecoder();
				this.deflateStream.SetFileFormatReader(fileFormatReader);
			}
		}

		public override void Flush()
		{
			bool flag = this.deflateStream == null;
			if (flag)
			{
				throw new ObjectDisposedException(null, SR.GetString("Object disposed"));
			}
			this.deflateStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException(SR.GetString("Not supported"));
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(SR.GetString("Not supported"));
		}

		public override IAsyncResult BeginRead(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			bool flag = this.deflateStream == null;
			if (flag)
			{
				throw new InvalidOperationException(SR.GetString("Object disposed"));
			}
			return this.deflateStream.BeginRead(array, offset, count, asyncCallback, asyncState);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			bool flag = this.deflateStream == null;
			if (flag)
			{
				throw new InvalidOperationException(SR.GetString("Object disposed"));
			}
			return this.deflateStream.EndRead(asyncResult);
		}

		public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			bool flag = this.deflateStream == null;
			if (flag)
			{
				throw new InvalidOperationException(SR.GetString("Object disposed"));
			}
			return this.deflateStream.BeginWrite(array, offset, count, asyncCallback, asyncState);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			bool flag = this.deflateStream == null;
			if (flag)
			{
				throw new InvalidOperationException(SR.GetString("Object disposed"));
			}
			this.deflateStream.EndWrite(asyncResult);
		}

		public override int Read(byte[] array, int offset, int count)
		{
			bool flag = this.deflateStream == null;
			if (flag)
			{
				throw new ObjectDisposedException(null, SR.GetString("Object disposed"));
			}
			return this.deflateStream.Read(array, offset, count);
		}

		public override void Write(byte[] array, int offset, int count)
		{
			bool flag = this.deflateStream == null;
			if (flag)
			{
				throw new ObjectDisposedException(null, SR.GetString("Object disposed"));
			}
			this.deflateStream.Write(array, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				bool flag = disposing && this.deflateStream != null;
				if (flag)
				{
					this.deflateStream.Dispose();
				}
				this.deflateStream = null;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
	}
}
