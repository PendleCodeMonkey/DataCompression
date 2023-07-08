namespace PendleCodeMonkey.DataCompressionLib
{
	internal static class RunLengthEncoding
	{
		// Maximum size of a small block (the size value must fit in 6 bits so this value is 2^6 - 1, which is 63)
		private const int MaxSmallBlockSize = 63;

		// Maximum size of a large block (the size value must fit in 14 bits so this value is 2^14 - 1, which is 16383)
		private const int MaxLargeBlockSize = 16383;

		// Flag that indicates that a block represents a repeating run of values.
		private const byte RepeatingRunFlag = 0x80;

		// Flag that indicates that a block contains more than 63 (i.e. MaxSmallBlockSize) data values (and is therefore a large block)
		private const byte LargeBlockFlag = 0x40;

		// Method that encodes the supplied binary data using Run-length encoding, returning the encoded data.
		internal static BinaryData Encode(BinaryData binData)
		{
			// Local function that creates objects representing blocks of data, splitting runs that are longer than 16383 bytes (the maximum
			// block size) into separate blocks.
			static List<(int start, int count, bool repeatRun)> GenerateBlocks(int startIndex, int runLength, bool repeatRun)
			{
				List<(int start, int count, bool repeatRun)> blocks = new();
				while (runLength > MaxLargeBlockSize)
				{
					blocks.Add((startIndex, MaxLargeBlockSize, repeatRun));
					startIndex += MaxLargeBlockSize;
					runLength -= MaxLargeBlockSize;
				}
				if (runLength > 0)
				{
					blocks.Add((startIndex, runLength, repeatRun));
				}
				return blocks;
			}

			// Generate a list of groups of consecutive equal values in the data (i.e. runs of equal values)
			// Note: we only consider a run of equal values to be a repeating run when there are more than 2 equal consecutive values.
			var repeatedDataGroups = binData.Data.GroupWhile((x, y) => x == y)
				.Where(x => x.Count() > 2)
				.Select(x => new { startIndex = x.First().index, runLength = x.Count() })
				.ToList();

			// Generate a list of tuples that describe the blocks of data (both repeating runs and non-repeating runs)
			List<(int start, int count, bool isRepeatingRun)> blocks = new();
			int dataIndex = 0;
			foreach (var item in repeatedDataGroups)
			{
				if (dataIndex < item.startIndex)
				{
					blocks.AddRange(GenerateBlocks(dataIndex, item.startIndex - dataIndex, false));
				}
				blocks.AddRange(GenerateBlocks(item.startIndex, item.runLength, true));
				dataIndex = item.startIndex + item.runLength;
			}
			if (dataIndex < binData.Length)
			{
				blocks.AddRange(GenerateBlocks(dataIndex, binData.Length - dataIndex, false));
			}

			BinaryData outputData = new();
			foreach (var block in blocks)
			{
				if (block.count > MaxSmallBlockSize)
				{
					byte headerByte = (byte)(((block.count >> 8) & 0x3f) | LargeBlockFlag);
					byte byt2 = (byte)(block.count & 0xff);
					headerByte |= (byte)(block.isRepeatingRun ? RepeatingRunFlag : 0);
					outputData.WriteByte(headerByte);
					outputData.WriteByte(byt2);
				}
				else
				{
					byte headerByte = (byte)(block.count | (block.isRepeatingRun ? RepeatingRunFlag : 0));
					outputData.WriteByte(headerByte);
				}
				if (block.isRepeatingRun)
				{
					outputData.WriteByte(binData[block.start]);
				}
				else
				{
					for (int i = block.start; i < block.start + block.count; i++)
					{
						outputData.WriteByte(binData[i]);
					}
				}
			}
			return outputData;
		}

		// Method that decodes the supplied run-length encoded binary data, returning the decoded data.
		internal static BinaryData Decode(BinaryData binData)
		{
			BinaryData outputData = new();

			int start = 0;
			while (start < binData.Length)
			{
				byte byt1 = binData[start++];
				bool repeatingRun = (byt1 & RepeatingRunFlag) == RepeatingRunFlag;
				int runLength = byt1 & 0x3f;
				if ((byt1 & LargeBlockFlag) == LargeBlockFlag)
				{
					byte byt2 = binData[start++];
					runLength = (runLength << 8) | byt2;
				}
				if (repeatingRun)
				{
					byte repeatedData = binData[start++];
					for (int i = 0; i < runLength; i++)
					{
						outputData.WriteByte(repeatedData);
					}
				}
				else
				{
					for (int i = 0; i < runLength; i++)
					{
						outputData.WriteByte(binData[start++]);
					}
				}
			}

			return outputData;
		}
	}
}