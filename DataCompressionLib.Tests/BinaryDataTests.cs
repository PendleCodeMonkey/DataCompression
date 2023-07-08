namespace PendleCodeMonkey.DataCompressionLib.Tests
{
	public class BinaryDataTests
	{
		[Fact]
		public void ConstructEmptyBinaryData_Succeeds()
		{
			BinaryData data = new();

			Assert.NotNull(data);
			Assert.NotNull(data.Data);
			Assert.Empty(data.Data);
		}

		[Fact]
		public void ConstructPopulatedBinaryData_Succeeds()
		{
			BinaryData data = new(new byte[] { 10, 20, 30, 40, 50, 60, 70 });

			Assert.NotNull(data);
			Assert.NotNull(data.Data);
			Assert.Equal(7, data.Data.Count);
		}

		[Fact]
		public void WriteBit_NoFlush_DoesNotChangeData()
		{
			BinaryData data = new ();

			data.WriteBit(true);

			Assert.Empty(data.Data);
		}

		[Fact]
		public void WriteBit_FollowedByFlush_ChangesData()
		{
			BinaryData data = new ();

			data.WriteBit(true);
			data.Flush();

			Assert.NotEmpty(data.Data);
			Assert.Equal(128, data.Data[0]);
		}

		[Fact]
		public void WriteByte_ChangesData()
		{
			BinaryData data = new();

			data.WriteByte(0xAA);

			Assert.NotEmpty(data.Data);
			Assert.Equal(0xAA, data.Data[0]);
		}

		[Fact]
		public void WriteBitFollowedByWriteByte_YieldsCorrectData()
		{
			BinaryData data = new();

			data.WriteBit(true);
			data.WriteByte(0xAA);
			data.Flush();

			Assert.Equal(2, data.Data.Count);
			Assert.Equal(0xD5, data.Data[0]);
			Assert.Equal(0, data.Data[1]);
		}

		[Fact]
		public void WriteInt_YieldsCorrectData()
		{
			BinaryData data = new();

			data.WriteInt(0x12345678);
			data.Flush();

			Assert.Equal(4, data.Data.Count);
			Assert.Equal(0x12, data.Data[0]);
			Assert.Equal(0x34, data.Data[1]);
			Assert.Equal(0x56, data.Data[2]);
			Assert.Equal(0x78, data.Data[3]);
		}
	}
}