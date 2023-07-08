namespace PendleCodeMonkey.DataCompressionLib
{
	/// <summary>
	/// The Burrows–Wheeler transform rearranges a stream of data into runs of similar data values.
	/// This is useful when run as a preprocessor for compression, as it tends to be easy to compress data
	/// that has runs of repeated values.
	/// </summary>
	internal static class BurrowsWheelerTransform
	{
		// Apply a Burrows-Wheeler Transform to the supplied data, returning the transformed data.
		internal static BinaryData Transform(BinaryData inputData)
		{
			int dataLen = inputData.Length;

			// Generate a list of index values (0 -> dataLen-1)
			var suffixArr = Enumerable.Range(0, dataLen).ToArray();

			//// Sort the index values by the values of the data to which they point.
			Array.Sort(suffixArr, (x, y) =>
			{
				if (x != y)
				{
					for (int idx = 0; idx < dataLen; idx++)
					{
						int xIdx = (x + idx) % dataLen;
						int yIdx = (y + idx) % dataLen;
						if (inputData[xIdx] > inputData[yIdx])
						{
							return 1;
						}
						if (inputData[xIdx] < inputData[yIdx])
						{
							return -1;
						}
					}
				}
				return 0;
			});

			// Determine the index of the first value in the original data (i.e. the one with a zero index value)
			int startIndex = suffixArr.TakeWhile(idx => idx != 0).Count();

			BinaryData outData = new();

			// Write the start index to the output data (as this is needed by the InverseTransform method)
			outData.WriteInt(startIndex);

			// Iterate over the suffix array to determine the last byte of each cyclic rotation and write them to the
			// output data.
			for (int i = 0; i < dataLen; i++)
			{
				// The last byte of this cyclic rotation is at index ((suffixArr[i] + dataLen - 1) % dataLen).
				outData.WriteByte(inputData[(suffixArr[i] + dataLen - 1) % dataLen]);
			}

			// Return the computed Burrows-Wheeler transform
			return outData;
		}

		// Perform an inverse Burrows-Wheeler Transform on the supplied data (which has been previously transformed), returning
		// the resulting inverse-transformed data (which will be the original data prior to applying a Burrows-Wheeler Transform)
		// Basically, this reverses the operation of the Transform method.
		internal static BinaryData InverseTransform(BinaryData inputData)
		{
			// Read the start index from the first integer in the supplied data.
			int start = inputData.ReadInt();

			// The length of the actual data is therefore the length of the entire data minus the length of the integer
			// that we have just read.
			int dataLen = inputData.Length - 4;

			// Use an array of tuples pairing data values to their corresponding index, allowing us to access the index values after
			// sorting by data value.
			List<(int index, byte data)> idxData = new();
			for (int i = 0; i < dataLen; i++)
			{
				idxData.Add((i, inputData.ReadByte()));
			}

			// Sort the tuples by the values of their data elements.
			// Note that the sort we perform MUST be stable (which, fortunately, the LINQ OrderBy operation is!)
			idxData = idxData.OrderBy(x => x.data).ToList();

			// Use the sorted tuples to regenerate the original data.
			List<byte> decoded = new()
			{
				idxData[start].data
			};
			int next = idxData[start].index;
			while (next != start)
			{
				decoded.Add(idxData[next].data);
				next = idxData[next].index;
			}

			// Return the decoded data.
			return new BinaryData(decoded.ToArray());
		}
	}
}
