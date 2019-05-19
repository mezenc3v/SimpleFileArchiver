using System;
using System.IO;
using System.IO.Compression;

namespace FileArchiver.Compression
{
	public class GZipCompressor : CompressorBase
	{
		public GZipCompressor(int blockSize) : base(blockSize)
		{
		}

		protected override void ProcessBlock(Block block)
		{
			using (var memoryStream = new MemoryStream())
			{
				using (var cs = new GZipStream(memoryStream, CompressionMode.Compress))
				{
					cs.Write(block.Buffer, 0, block.Buffer.Length);
				}
				block.Buffer = memoryStream.ToArray();
			}
		}

		protected override Block ReadBlock(Stream stream, int counter)
		{
			var blockSize = stream.Length - stream.Position <= BlockSize
				? (int)(stream.Length - stream.Position)
				: BlockSize;

			var buffer = new byte[blockSize];
			stream.Read(buffer, 0, blockSize);
			return new Block(counter, buffer);
		}

		protected override void WriteBlock(Stream stream, Block block)
		{
			BitConverter.GetBytes(block.Buffer.Length).CopyTo(block.Buffer, 4);
			stream.Write(block.Buffer, 0, block.Buffer.Length);
		}
	}
}