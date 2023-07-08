namespace PendleCodeMonkey.DataCompressionLib
{
	internal class HuffmanNode
	{
		internal byte Data { get; set; }
		internal int Frequency { get; set; }
		internal HuffmanNode? Left { get; set; }
		internal HuffmanNode? Right { get; set; }

		internal HuffmanNode(byte data, int freq, HuffmanNode? left, HuffmanNode? right)
		{
			Data = data;
			Frequency = freq;
			Left = left;
			Right = right;
		}

		// Indicates if this is a leaf node.
		internal bool IsLeaf => Left == null && Right == null;
	}
}
