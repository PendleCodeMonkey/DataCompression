namespace PendleCodeMonkey.DataCompressionLib
{
	internal static class MoveToFrontTransform
	{
		// Apply move-to-front encoding to the supplied binary data, returning the encoded data.
		internal static BinaryData Encode(BinaryData inputData)
		{
			BinaryData outputData = new();
			var byteArr = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
			foreach (byte b in inputData.Data)
			{
				byte count;
				byte tmp = byteArr[0];
				for (count = 0; b != byteArr[count]; count++)
				{
					(tmp, byteArr[count]) = (byteArr[count], tmp);
				}
				byteArr[count] = tmp;
				outputData.WriteByte(count);
				byteArr[0] = b;
			}
			outputData.Flush();
			return outputData;
		}

		// Apply move-to-front decoding to the supplied data (which will have previously had move-to-front encoding
		// applied to it), returning the decoded data.
		internal static BinaryData Decode(BinaryData inputData)
		{
			BinaryData outputData = new();
			var byteArr = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
			foreach (byte b in inputData.Data)
			{
				byte byt = byteArr[b];
				for (byte count = b; count > 0; byteArr[count] = byteArr[--count]) ;
				byteArr[0] = byt;
				outputData.WriteByte(byt);
			}
			outputData.Flush();
			return outputData;
		}
	}
}
