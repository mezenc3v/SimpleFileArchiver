using System;
using System.IO;
using System.IO.Compression;

namespace FileArchiver.Compression
{
	public class GZipDecompressor : CompressorBase
	{
		public GZipDecompressor(int blockSize) : base(blockSize)
		{
		}

		protected override void ProcessBlock(Block block)
		{
			using (var ms = new MemoryStream(block.CompressedBuffer))
			using (var gz = new GZipStream(ms, CompressionMode.Decompress))
			{
				gz.Read(block.Buffer, 0, block.Buffer.Length);
			}
		}

		protected override Block ReadBlock(Stream stream, int counter)
		{
			byte[] lengthBuffer = new byte[8];
			stream.Read(lengthBuffer, 0, lengthBuffer.Length);
			int blockLength = BitConverter.ToInt32(lengthBuffer, 4);
			byte[] compressedData = new byte[blockLength];
			lengthBuffer.CopyTo(compressedData, 0);
			stream.Read(compressedData, 8, blockLength - 8);
			int dataSize = BitConverter.ToInt32(compressedData, blockLength - 4);
			byte[] lastBuffer = new byte[dataSize];
			return new Block(counter, lastBuffer, compressedData);
		}

		protected override void WriteBlock(Stream stream, Block block)
		{
			stream.Write(block.Buffer, 0, block.Buffer.Length);
		}
	}
}