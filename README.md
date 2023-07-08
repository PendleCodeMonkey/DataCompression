# A Data Compression library, implemented in C# #

This repository contains the code for a Data Compression class library written in C# and a simple Windows console application that demonstrates the use of this library.

<br>

The DataCompression solution consists of the following projects:

- **DataCompressionLib**: The code for the Data Compression class library itself.
- **DataCompressionLib.Tests**: A handful of unit tests for some of the class library functionality.
- **DataCompressionTestApp**: A simple Windows console application that demonstrates the functionality of the library.

<br>

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

<br>

### What's here?

A class library that implements data compression using Huffman and Run-length Encoding techniques.  
The library also implements the Burrows Wheeler Transform (BWT), which can optionally be applied to restructure the data in such as way as to make it more compressible.  
Additionally, the library implements a Move-To-Front Transform (MTF), which can also be optionally applied in order to make the data more compressible.  

<br>

### How do you use it?

The library implements a BinaryData class that is used to hold binary data and perform operations on it. Extension methods are implemented for this BinaryData class that allow Huffman encoding, Run-length encoding, the Burrows Wheeler Transform, and the Move-To-Front Transform to be applied to the binary data.  As each extension method returns an instance of the BinaryData class, they can be chained together (in a fluent manner); so, assuming that `binDataFile` is an instance of the BinaryData class populated with data to be compressed, then a Burrows Wheeler Transform (BWT), Move-To-Front Transform (MTF), and Huffman encoding can be applied to the data as follows:  
  
 `var huffCompressedData = binDataFile.BWT_Transform().MTF_Encode().Huffman_Compress();`  
  
In order to reverse the compression (i.e. expand back to the original data), the decoding operations must be performed in the reverse order of encoding (i.e. the above BWT encoding, followed by MTF encoding, followed by Huffman encoding must be expanded by using Huffman decoding, followed by MTF decoding, followed by BWT decoding), as follows:    

 `var huffExpandedData = huffCompressedData.Huffman_Expand().MTF_Decode().BWT_InverseTransform();`  
<br>

The included Windows console application has a simple demonstration of the functionality.  

<br>
<br>

### History

| Version | Details
|---:| ---
| 1.0.0 | Initial implementation of the Data Compression class library and console application.

