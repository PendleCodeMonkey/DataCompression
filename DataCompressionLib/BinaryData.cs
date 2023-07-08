namespace PendleCodeMonkey.DataCompressionLib
{
	public class BinaryData
	{
		private int _position = 0;

		private byte buffer = 0;
		private byte numBits = 0;

		private readonly byte[] bitmasks = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

		public List<byte> Data { get; private set; }

		public byte this[int i]
		{
			get { return Data[i]; }
			set { Data[i] = value; }
		}

		public int Length { get => Data.Count; }

		public BinaryData()
		{
			Data = new List<byte>();
		}

		public BinaryData(byte[] data)
		{
			Data = new List<byte>(data);
		}

		private void WriteAndClearBuffer()
		{
			if (numBits > 0)
			{
				buffer <<= (8 - numBits);
				Data.Add(buffer);
				buffer = numBits = 0;
			}
		}

		private void InternalWriteBit(bool bit)
		{
			buffer <<= 1;
			buffer |= (byte)(bit ? 1 : 0);

			numBits++;
			// if we've filled 8 bits (i.e. a byte) then write the byte of data and clear the bit buffer
			if (numBits == 8)
			{
				WriteAndClearBuffer();
			}
		}

		private void InternalWriteByte(byte value)
		{
			if (numBits == 0)
			{
				Data.Add(value);
				return;
			}

			foreach (byte mask in bitmasks)
			{
				bool bit = (value & mask) != 0;
				InternalWriteBit(bit);
			}
		}

		internal void WriteBit(bool value)
		{
			InternalWriteBit(value);
		}

		internal void WriteByte(byte value)
		{
			InternalWriteByte(value);
		}

		internal void WriteBytes(byte[] arr)
		{
			foreach (byte b in arr)
			{
				InternalWriteByte(b);
			}
		}

		internal void WriteInt(int x)
		{
			InternalWriteByte((byte)((x >> 24) & 0xff));
			InternalWriteByte((byte)((x >> 16) & 0xff));
			InternalWriteByte((byte)((x >> 8) & 0xff));
			InternalWriteByte((byte)(x & 0xff));
		}

		internal void Flush()
		{
			WriteAndClearBuffer();
		}

		internal void ResetPosition()
		{
			_position = 0;
		}

		internal bool IsEndOfData()
		{
			return _position >= Data.Count;
		}

		private void PopulateBuffer()
		{
			if (_position >= Data.Count)
			{
				throw new InvalidOperationException("An attempt has been made to read beyond the end of the data.");
			}
			buffer = Data[_position++];
			numBits = 8;
		}

		private bool InternalReadBit()
		{
			if (numBits == 0)
			{
				PopulateBuffer();
			}
			bool bit = (buffer & bitmasks[8 -  numBits]) != 0;
			numBits--;
			return bit;
		}

		private byte InternalReadByte()
		{
			if (numBits == 0)
			{
				PopulateBuffer();
				numBits = 0;
				return buffer;
			}

			byte value = (byte)(buffer << (8 - numBits));
			byte prevNumBits = numBits;
			PopulateBuffer();
			numBits = prevNumBits;
			value |= (byte)(buffer >> numBits);
			return value;
		}

		internal bool ReadBit()
		{
			return InternalReadBit();
		}

		internal byte ReadByte()
		{
			return InternalReadByte();
		}

		internal int ReadInt()
		{
			int x = 0;
			for (int i = 0; i < 4; i++)
			{
				byte b = ReadByte();
				x <<= 8;
				x |= b;
			}
			return x;
		}
	}
}
