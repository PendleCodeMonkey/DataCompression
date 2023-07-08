namespace PendleCodeMonkey.DataCompressionLib
{
	public static class Extensions
	{
		public static IEnumerable<IEnumerable<(int index, T data)>> GroupWhile<T>(this IEnumerable<T> seq, Func<T, T, bool> condition)
		{
			T prev = seq.First();
			List<(int, T)> list = new() { (0, prev) };
			int index = 1;
			foreach (T item in seq.Skip(1))
			{
				if (condition(prev, item) == false)
				{
					yield return list;
					list = new List<(int, T)>();
				}
				list.Add((index, item));
				prev = item;
				index++;
			}

			yield return list;
		}

		public static BinaryData RLE_Encode(this BinaryData data)
		{
			return RunLengthEncoding.Encode(data);
		}

		public static BinaryData RLE_Decode(this BinaryData data)
		{
			return RunLengthEncoding.Decode(data);
		}

		public static BinaryData BWT_Transform(this BinaryData data)
		{
			return BurrowsWheelerTransform.Transform(data);
		}

		public static BinaryData BWT_InverseTransform(this BinaryData data)
		{
			return BurrowsWheelerTransform.InverseTransform(data);
		}

		public static BinaryData MTF_Encode(this BinaryData data)
		{
			return MoveToFrontTransform.Encode(data);
		}

		public static BinaryData MTF_Decode(this BinaryData data)
		{
			return MoveToFrontTransform.Decode(data);
		}

		public static BinaryData Huffman_Compress(this BinaryData data)
		{
			return Huffman.Compress(data);
		}

		public static BinaryData Huffman_Expand(this BinaryData data)
		{
			return Huffman.Expand(data);
		}
	}
}
