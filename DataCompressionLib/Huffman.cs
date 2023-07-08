namespace PendleCodeMonkey.DataCompressionLib
{
	internal static class Huffman
	{
		// Compress the supplied binary data using Huffman encoding and return the encoded data.
		internal static BinaryData Compress(BinaryData data)
		{
			// Local method that builds a Huffman tree using the given data value frequencies and
			// returns the root node of the generated tree.
			static HuffmanNode BuildTree(int[] frequencies)
			{
				PriorityQueue<HuffmanNode, int> pq = new();
				for (int i = 0; i < 256; i++)
				{
					if (frequencies[i] > 0)
					{
						pq.Enqueue(new HuffmanNode((byte)i, frequencies[i], null, null), frequencies[i]);
					}
				}

				// Merge nodes until the priority queue contains only a single element (the root).
				while (pq.Count > 1)
				{
					HuffmanNode left = pq.Dequeue();
					HuffmanNode right = pq.Dequeue();
					int parentFreq = left.Frequency + right.Frequency;
					HuffmanNode parent = new(0, parentFreq, left, right);
					pq.Enqueue(parent, parentFreq);
				}

				// Remove and return the minimal element from the priority queue (the root node of the tree).
				return pq.Dequeue();
			}

			// Local method that writes the encoded tree to the binary data output.
			static void WriteTree(HuffmanNode? x, BinaryData outputData)
			{
				if (x != null)
				{
					if (x.IsLeaf)
					{
						outputData.WriteBit(true);
						outputData.WriteByte(x.Data);
						return;
					}
					outputData.WriteBit(false);
					WriteTree(x.Left, outputData);
					WriteTree(x.Right, outputData);
				}
			}

			// Local method that builds a lookup table mapping data values to their encodings.
			static void BuildCodeLookupTable(string[] codeTable, HuffmanNode? x, string s)
			{
				if (x != null)
				{
					if (!x.IsLeaf)
					{
						BuildCodeLookupTable(codeTable, x.Left, s + '0');
						BuildCodeLookupTable(codeTable, x.Right, s + '1');
					}
					else
					{
						codeTable[x.Data] = s;
					}
				}
			}

			BinaryData outputData = new();

			// Write the size of the original (uncompressed) data to the output.
			outputData.WriteInt(data.Length);

			// Determine the frequency count of each byte in the input data.
			int[] frequencies = new int[256];
			foreach (byte dataValue in data.Data)
			{
				frequencies[dataValue]++;
			}

			// Build the Huffman tree using these frequencies.
			HuffmanNode root = BuildTree(frequencies);

			// Write the Huffman tree to the output data
			WriteTree(root, outputData);

			// Build the data->encoding lookup table
			string[] codeTable = new string[256];
			BuildCodeLookupTable(codeTable, root, string.Empty);

			// Use the generated code table to encode the input data.
			foreach (byte dataValue in data.Data)
			{
				foreach (var c in codeTable[dataValue])
				{
					outputData.WriteBit(c == '1');
				}
			}

			// Flush the output data and return it.
			outputData.Flush();
			return outputData;
		}

		// Expand the supplied Huffman-encoded binary data and return the decoded data (i.e. the original [uncompressed] data).
		internal static BinaryData Expand(BinaryData inputData)
		{
			// Local method that reads the Huffman tree from the input [encoded] data.
			static HuffmanNode ReadTree(BinaryData inputData)
			{
				bool isLeaf = inputData.ReadBit();
				if (isLeaf)
				{
					return new HuffmanNode(inputData.ReadByte(), -1, null, null);
				}
				else
				{
					return new HuffmanNode(0, -1, ReadTree(inputData), ReadTree(inputData));
				}
			}

			// Read in the first integer value (which holds the length of the original [uncompressed] data).
			int length = inputData.ReadInt();

			// Read in the Huffman tree from the input data.
			HuffmanNode root = ReadTree(inputData);

			BinaryData outputData = new();

			// Decode using the Huffman tree.
			for (int i = 0; i < length; i++)
			{
				HuffmanNode? x = root;
				while (x != null && !x.IsLeaf)
				{
					bool bit = inputData.ReadBit();
					x = bit ? x.Right : x.Left;
				}
				if (x != null)
				{
					outputData.WriteByte(x.Data);
				}
			}

			// Flush the output data and return it.
			outputData.Flush();
			return outputData;
		}
	}
}
