using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet
{
	/// <summary>
	/// This type supports the Fluorine infrastructure and is not intended to be used directly from your code.
	/// http://java.sun.com/j2se/1.5.0/docs/api/java/nio/ByteBuffer.html
	/// 
	/// The following invariant holds for the mark, position, limit, and capacity values: 
	/// 0 lte mark lte position lte limit lte capacity 
	/// </summary>
	public class ByteBuffer : Stream
	{
		private MemoryStream _stream;
		private bool _autoExpand;
		private long _bookmark;

		/// <summary>
		/// Initializes a new instance of the ByteBuffer class.
		/// </summary>
		/// <param name="stream">Wraps the MemoryStream into a buffer.</param>
		public ByteBuffer(MemoryStream stream)
		{
			_stream = stream;
			ClearBookmark();
		}

		/// <summary>
		/// Allocates a new byte buffer.
		/// The new buffer's position will be zero, its limit will be its capacity, 
		/// and its mark will be undefined. 
		/// It will have a backing array, and its array offset will be zero. 
		/// </summary>
		/// <param name="capacity"></param>
		/// <returns></returns>
		public static ByteBuffer Allocate(int capacity)
		{
			MemoryStream ms = new MemoryStream(capacity);
            ByteBuffer buffer = new ByteBuffer(ms)
            {
                Limit = capacity
            };
            return buffer;
		}

		/// <summary>
		/// Wraps a byte array into a buffer.
		/// The new buffer will be backed by the given byte array; that is, modifications 
		/// to the buffer will cause the array to be modified and vice versa. 
		/// The new buffer's capacity will be array.length, its position will be offset, 
		/// its limit will be offset + length, and its mark will be undefined.
		/// </summary>
		/// <param name="array">Byte array to wrap.</param>
		/// <param name="offset">Offset in the byte array.</param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static ByteBuffer Wrap(byte[] array, int offset, int length)
		{
			MemoryStream ms = new MemoryStream(array, offset, length, true, true);
			ms.Capacity = array.Length;
			ms.SetLength(offset + length);
			ms.Position = offset;
			return new ByteBuffer(ms);
		}
		/// <summary>
		/// Wraps a byte array into a buffer. 
		/// The new buffer will be backed by the given byte array; that is, modifications 
		/// to the buffer will cause the array to be modified and vice versa. 
		/// The new buffer's capacity and limit will be array.length, its position will be zero,
		/// and its mark will be undefined.
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static ByteBuffer Wrap(byte[] array)
		{
			return Wrap(array, 0, array.Length);
		}

		/// <summary>
		/// Turns on or off autoExpand
		/// </summary>
		public bool AutoExpand
		{
			get { return _autoExpand; }
			set { _autoExpand = value; }
		}
		/// <summary>
		/// Returns this buffer's capacity.
		/// </summary>
		public int Capacity
		{
			get { return (int)_stream.Capacity; }
		}
		/// <summary>
		/// Returns this buffer's limit. 
		/// </summary>
		public int Limit
		{
			get { return (int)_stream.Length; }
			set { _stream.SetLength(value); }
		}
		/// <summary>
		/// Returns the number of elements between the current position and the limit. 
		/// </summary>
		public int Remaining
		{
			get { return this.Limit - (int)this.Position; }
		}
		/// <summary>
		/// Tells whether there are any elements between the current position and the limit. 
		/// </summary>
		public bool HasRemaining
		{
			get { return this.Remaining > 0; }
		}
		/// <summary>
		/// Gets the current bookmark value.
		/// </summary>
		public long Bookmark
		{
			get { return _bookmark; }
		}
		/// <summary>
		/// Sets this buffer's bookmark at its position.
		/// </summary>
		/// <returns>Returns this bookmark value.</returns>
		public long Mark()
		{
			_bookmark = this.Position;
			return _bookmark;
		}
		/// <summary>
		/// Clears the current bookmark.
		/// </summary>
		public void ClearBookmark()
		{
			_bookmark = -1;
		}
		/// <summary>
		/// Resets this buffer's position to the previously-marked position. 
		/// Invoking this method neither changes nor discards the mark's value. 
		/// </summary>
		public void Reset()
		{
			if (_bookmark != -1)
				this.Position = _bookmark;
		}
		/// <summary>
		/// Clears this buffer. The position is set to zero, the limit is set to the capacity, and the mark is discarded.
		/// </summary>
		public void Clear()
		{
			ClearBookmark();
			Position = 0;
			SetLength(0);
		}

#if !(NET_1_1)
		/// <summary>
		/// Releases all resources used by this object.
		/// </summary>
		/// <param name="disposing">Indicates if this is a dispose call dispose.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_stream != null)
					_stream.Dispose();
				_stream = null;
			}
			base.Dispose(disposing);
		}
#endif

		/// <summary>
		/// Flips this buffer. The limit is set to the current position and then 
		/// the position is set to zero. If the mark is defined then it is discarded.
		/// </summary>
		public void Flip()
		{
			ClearBookmark();
			this.Limit = (int)this.Position;
			this.Position = 0;
		}
		/// <summary>
		/// Rewinds this buffer. The position is set to zero and the mark is discarded.
		/// </summary>
		public void Rewind()
		{
			ClearBookmark();
			this.Position = 0;
		}
		/// <summary>
		/// Writes the given byte into this buffer at the current position, and then increments the position.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public void Put(byte value)
		{
			this.WriteByte(value);
		}
		/// <summary>
		/// Relative bulk put method.
		/// 
		/// This method transfers bytes into this buffer from the given source array. 
		/// If there are more bytes to be copied from the array than remain in this buffer, 
		/// that is, if length > remaining(), then no bytes are transferred and a 
		/// BufferOverflowException is thrown. 
		/// 
		/// Otherwise, this method copies length bytes from the given array into this buffer, 
		/// starting at the given offset in the array and at the current position of this buffer. 
		/// The position of this buffer is then incremented by length. 
		/// </summary>
		/// <param name="src">The array from which bytes are to be read.</param>
		/// <param name="offset">The offset within the array of the first byte to be read; must be non-negative and no larger than the array length.</param>
		/// <param name="length">The number of bytes to be read from the given array; must be non-negative and no larger than length - offset.</param>
		public void Put(byte[] src, int offset, int length)
		{
			_stream.Write(src, offset, length);
		}
		/// <summary>
		/// This method transfers the entire content of the given source byte array into this buffer. 
		/// </summary>
		/// <param name="src">The array from which bytes are to be read.</param>
		public void Put(byte[] src)
		{
			Put(src, 0, src.Length);
		}
		/// <summary>
		/// Appends a byte buffer to this ByteArray.
		/// </summary>
		/// <param name="src">The byte buffer to append.</param>
		public void Append(byte[] src)
		{
			Append(src, 0, src.Length);
		}
		/// <summary>
		/// Appends a byte buffer to this ByteArray.
		/// </summary>
		/// <param name="src">The byte buffer to append.</param>
		/// <param name="offset">Offset in the byte buffer.</param>
		/// <param name="length">Number of bytes to append.</param>
		public void Append(byte[] src, int offset, int length)
		{
			long position = this.Position;
			this.Position = this.Limit;
			Put(src, offset, length);
			this.Position = position;
		}

		/// <summary>
		/// This method transfers the bytes remaining in the given source buffer into this buffer. 
		/// If there are more bytes remaining in the source buffer than in this buffer, 
		/// that is, if src.remaining() > remaining(), then no bytes are transferred 
		/// and a BufferOverflowException is thrown. 
		/// 
		/// Otherwise, this method copies n = src.remaining() bytes from the given buffer into this buffer, 
		/// starting at each buffer's current position. The positions of both buffers are then 
		/// incremented by n. 
		/// </summary>
		/// <param name="src">The source buffer from which bytes are to be read; must not be this buffer.</param>
		public void Put(ByteBuffer src)
		{
			while (src.HasRemaining)
				Put(src.Get());
		}
		/// <summary>
		/// Transfers the specified number of bytes from the given source buffer into this buffer.
		/// </summary>
		/// <param name="src">The source buffer from which bytes are to be read; must not be this buffer.</param>
		/// <param name="count">Number of bytes to transfer.</param>
		public void Put(ByteBuffer src, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Put(src.Get());
			}
		}
		/// <summary>
		/// Absolute put method.
		/// Writes the given byte into this buffer at the given index. 
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The byte to write.</param>
		public void Put(int index, byte value)
		{
			_stream.GetBuffer()[index] = value;
		}
		/// <summary>
		/// Relative get method. Reads the byte at this buffer's current position, and then increments the position.
		/// </summary>
		/// <returns></returns>
		public byte Get()
		{
			return (byte)this.ReadByte();
		}
		/// <summary>
		/// Reads a 4-byte signed integer using network byte order encoding.
		/// </summary>
		/// <returns>The 4-byte signed integer.</returns>
		public int GetInt()
		{
			// Read the next 4 bytes, shift and add
			byte[] bytes = this.ReadBytes(4);
			return ((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]);
		}
		/// <summary>
		/// Reads a 2-byte signed integer using network byte order encoding.
		/// </summary>
		/// <returns>The 2-byte signed integer.</returns>
		public short GetShort()
		{
			//Read the next 2 bytes, shift and add.
			byte[] bytes = this.ReadBytes(2);
			return (short)((bytes[0] << 8) | bytes[1]);
		}
		/// <summary>
		/// Absolute get method. Reads the byte at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public byte Get(int index)
		{
			return _stream.GetBuffer()[index];
		}

		public byte this[int index]
		{
			get { return Get(index); }
			set { Put(index, value); }
		}

		/// <summary>
		/// Writes the stream contents to a byte array, regardless of the Position property.
		/// </summary>
		/// <returns>A new byte array.</returns>
		/// <remarks>
		/// This method omits unused bytes in ByteBuffer from the array. To get the entire buffer, use the GetBuffer method. 
		/// </remarks>
		public byte[] ToArray()
		{
			return _stream.ToArray();
		}

		/// <summary>
		/// Returns the array of unsigned bytes from which this stream was created.
		/// </summary>
		/// <returns>
		/// The byte array from which this ByteBuffer was created, or the underlying array if a byte array was not provided to the ByteBuffer constructor during construction of the current instance. 
		/// </returns>
		public byte[] GetBuffer()
		{
			return _stream.GetBuffer();
		}
		/// <summary>
		/// Compacts this buffer
		/// 
		/// The bytes between the buffer's current position and its limit, if any, 
		/// are copied to the beginning of the buffer. That is, the byte at 
		/// index p = position() is copied to index zero, the byte at index p + 1 is copied 
		/// to index one, and so forth until the byte at index limit() - 1 is copied 
		/// to index n = limit() - 1 - p. 
		/// The buffer's position is then set to n+1 and its limit is set to its capacity. 
		/// The mark, if defined, is discarded. 
		/// The buffer's position is set to the number of bytes copied, rather than to zero, 
		/// so that an invocation of this method can be followed immediately by an invocation of 
		/// another relative put method. 
		/// </summary>
		public void Compact()
		{
			if (this.Position == 0)
				return;
			for (int i = (int)this.Position; i < this.Limit; i++)
			{
				byte value = this.Get(i);
				this.Put(i - (int)this.Position, value);
			}
			//this.Position = this.Limit - this.Position;
			//this.Limit = this.Capacity;
			this.Limit = this.Limit - (int)this.Position;
			this.Position = 0;
		}

		/// <summary>
		/// Forwards the position of this buffer as the specified size bytes.
		/// </summary>
		/// <param name="size"></param>
		public void Skip(int size)
		{
			this.Position += size;
		}
		/// <summary>
		/// Fills this buffer with the specified value.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="count"></param>
		public void Fill(byte value, int count)
		{
			for (int i = 0; i < count; i++)
				this.Put(value);
		}

		#region Stream

		/// <summary>
		/// Gets a value indicating whether the current stream supports reading.
		/// </summary>
		public override bool CanRead
		{
			get
			{
				return _stream.CanRead;
			}
		}
		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		public override bool CanSeek
		{
			get
			{
				return _stream.CanSeek;
			}
		}
		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		public override bool CanWrite
		{
			get
			{
				return _stream.CanWrite;
			}
		}
		/// <summary>
		/// Closes the current stream and releases any resources associated with the current stream.
		/// </summary>
		public override void Close()
		{
			_stream.Close();
		}
		/// <summary>
		/// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
		/// </summary>
		public override void Flush()
		{
			_stream.Flush();
		}
		/// <summary>
		/// Gets the length in bytes of the stream.
		/// </summary>
		public override long Length
		{
			get
			{
				return _stream.Length;
			}
		}
		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		public override long Position
		{
			get
			{
				return _stream.Position;
			}
			set
			{
				_stream.Position = value;
				if (_bookmark > value)
				{
					//discard bookmark
					_bookmark = 0;
				}
			}
		}
		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			return _stream.Read(buffer, offset, count);
		}
		/// <summary>
		/// Relative bulk get method. 
		/// This method transfers bytes from this buffer into the given destination array. 
		/// An invocation of this method behaves in exactly the same way as the invocation buffer.Get(a, 0, a.Length)
		/// </summary>
		/// <param name="buffer">An array of bytes.</param>
		/// <returns>The total number of bytes read into the buffer.</returns>
		public int Read(byte[] buffer)
		{
			return _stream.Read(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
		/// </summary>
		/// <returns>The unsigned byte cast to an Int32, or -1 if at the end of the stream.</returns>
		public override int ReadByte()
		{
			return _stream.ReadByte();
		}
		/// <summary>
		/// Sets the position within the current stream. 
		/// </summary>
		/// <param name="offset">A byte offset relative to the origin parameter.</param>
		/// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
		/// <returns>The new position within the current stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			return _stream.Seek(offset, origin);
		}
		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="value">The desired length of the current stream in bytes.</param>
		public override void SetLength(long value)
		{
			_stream.SetLength(value);
		}
		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			_stream.Write(buffer, offset, count);
		}
		/// <summary>
		/// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
		/// </summary>
		/// <param name="value">The byte to write to the stream.</param>
		public override void WriteByte(byte value)
		{
			_stream.WriteByte(value);
		}

		#endregion Stream

		/// <summary>
		/// Reads count bytes from the current stream into a byte array and advances the current position by count bytes. 
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public byte[] ReadBytes(int count)
		{
			byte[] bytes = new byte[count];
			for (int i = 0; i < count; i++)
			{
				bytes[i] = (byte)this.ReadByte();
			}
			return bytes;
		}
		/// <summary>
		/// Writes a 32-bit signed integer to the current position using variable length unsigned 29-bit integer encoding.
		/// </summary>
		/// <param name="value">A 32-bit signed integer.</param>
		public void WriteMediumInt(int value)
		{
			byte[] bytes = new byte[3];
			bytes[0] = (byte)(0xFF & (value >> 16));
			bytes[1] = (byte)(0xFF & (value >> 8));
			bytes[2] = (byte)(0xFF & (value >> 0));
			this.Write(bytes, 0, bytes.Length);
		}

		public void WriteReverseInt(int value)
		{
			byte[] bytes = new byte[4];
			bytes[3] = (byte)(0xFF & (value >> 24));
			bytes[2] = (byte)(0xFF & (value >> 16));
			bytes[1] = (byte)(0xFF & (value >> 8));
			bytes[0] = (byte)(0xFF & value);
			this.Write(bytes, 0, bytes.Length);
		}

		private void WriteBigEndian(byte[] bytes)
		{
			WriteBigEndian((int)this.Position, bytes);
		}

		private void WriteBigEndian(int index, byte[] bytes)
		{
			for (int i = bytes.Length - 1, j = 0; i >= 0; i--, j++)
			{
				this.Put(index + j, bytes[i]);
			}
			this.Position += bytes.Length;
		}

		private void WriteBytes(int index, byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				this.Put(index + i, bytes[i]);
			}
		}
		/// <summary>
		/// Writes a 16-bit unsigned integer to the current position.
		/// </summary>
		/// <param name="value">A 16-bit unsigned integer.</param>
		public void PutShort(short value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			WriteBigEndian(bytes);
		}

		/// <summary>
		/// Relative put method for writing an int value.
		/// Writes four bytes containing the given int value, in the current byte order, into this buffer at the current position, and then increments the position by four.
		/// </summary>
		/// <param name="value">The int value to be written.</param>
		public void PutInt(int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			//this.Write(bytes, 0, bytes.Length);
			WriteBigEndian(bytes);
		}
		/// <summary>
		/// Absolute put method for writing an int value.
		/// Writes four bytes containing the given int value, in the current byte order, into this buffer at the given index.
		/// </summary>
		/// <param name="index">The index at which the bytes will be written.</param>
		/// <param name="value">The int value to be written.</param>
		public void PutInt(int index, int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			for (int i = bytes.Length - 1, j = 0; i >= 0; i--, j++)
			{
				this.Put(index + j, bytes[i]);
			}
		}

		public void PutLong(long value)
        {
			byte[] bytes = BitConverter.GetBytes(value);
			WriteBigEndian(bytes);
		}

		/// <summary>
		/// Absolute put method for writing an int value.
		/// Writes four bytes containing the given int value, in the current byte order, into this buffer at the given index.
		/// </summary>
		/// <param name="index">The index at which the bytes will be written.</param>
		/// <param name="value">The int value to be written.</param>
		public void Put(int index, UInt32 value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			this.WriteBytes(index, bytes);
		}


		public void Put(int index, UInt16 value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			this.WriteBytes(index, bytes);
		}

		public int ReadUInt24()
		{
			byte[] bytes = this.ReadBytes(3);
			int value = bytes[0] << 16 | bytes[1] << 8 | bytes[2];
			return value;
		}
		/// <summary>
		/// Reads a 4-byte signed integer.
		/// </summary>
		/// <returns>The 4-byte signed integer.</returns>
		public int ReadReverseInt()
		{
			byte[] bytes = this.ReadBytes(4);
			int val = 0;
			val += bytes[3] << 24;
			val += bytes[2] << 16;
			val += bytes[1] << 8;
			val += bytes[0];
			return val;
		}
		/// <summary>
		/// Puts an in buffer stream onto an out buffer stream and returns the bytes written.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="input"></param>
		/// <param name="numBytesMax"></param>
		/// <returns></returns>
		public static int Put(ByteBuffer output, ByteBuffer input, int numBytesMax)
		{
			int limit = input.Limit;
			int numBytesRead = (numBytesMax > input.Remaining) ? input.Remaining : numBytesMax;
			/*
			input.Limit = (int)input.Position + numBytesRead;
			output.Put(input);
			input.Limit = limit;
			*/
			output.Put(input, numBytesRead);
			return numBytesRead;
		}
		/// <summary>
		/// Write the buffer content to a file.
		/// </summary>
		/// <param name="file"></param>
		public void Dump(string file)
		{
			using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				byte[] buffer = this.ToArray();
				fs.Write(buffer, 0, buffer.Length);
				fs.Close();
			}
		}
	}
}
