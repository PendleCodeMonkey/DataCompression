using PendleCodeMonkey.DataCompressionLib;

// Read binary data from the included test data file (which happens to be a source file from my Z80 emulator project)
string fileName = @"TestData\TestDataFile.txt";
byte[] buff = File.ReadAllBytes(fileName);

// Create a BinaryData object containing the data we have just read.
BinaryData binDataFile = new(buff);
if (binDataFile != null)
{
	// Compress the loaded binary data using Burrows Wheeler Transform encoding, followed by
	// Move-To-Front Transform encoding, followed by Huffman encoding.
	var huffCompressedData = binDataFile.BWT_Transform().MTF_Encode().Huffman_Compress();

	// Reverse the compression by calling Huffman decoding, followed by Move-To-Front Transform decoding, finishing
	// off with a Burrows Wheeler inverse Transform.
	var huffExpandedData = huffCompressedData.Huffman_Expand().MTF_Decode().BWT_InverseTransform();

	// Check that the expanded data exactly matches the original binary data.
	if (huffExpandedData.Data.Count != binDataFile.Data.Count || !Enumerable.SequenceEqual(huffExpandedData.Data, binDataFile.Data))
	{
		// Mismatch, so something went wrong!
		Console.WriteLine("**** The Huffman decoded data does not match the original data ****");
	}
	else
	{
		// Success, so report the lengths of the original data and the compressed data.
		Console.WriteLine($"Original data: {binDataFile.Length} bytes.   Huffman compressed data: {huffCompressedData.Length} bytes.");
	}
	Console.WriteLine();


	// Perform the same operations, but this time using Run-length Encoding instead of Huffman encoding.
	var rleCompressedData = binDataFile.BWT_Transform().RLE_Encode();
	var rleExpandedData = rleCompressedData.RLE_Decode().BWT_InverseTransform();
	if (rleExpandedData.Data.Count != binDataFile.Data.Count || !Enumerable.SequenceEqual(rleExpandedData.Data, binDataFile.Data))
	{
		Console.WriteLine("**** The RLE decoded data does not match the original data ****");
	}
	else
	{
		Console.WriteLine($"Original data: {binDataFile.Length} bytes.   RLE compressed data: {rleCompressedData.Length} bytes.");
	}
	Console.WriteLine();

    Console.WriteLine("Press any key");
    Console.ReadKey();
}
