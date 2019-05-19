using System;

namespace FileArchiver.Compression
{
	public class Block
	{
		public int Id { get; }
		public byte[] Buffer { get; set; }
		public byte[] CompressedBuffer { get; set; }

		public Block(int id, byte[] buffer) : this(id, buffer, new byte[0])
		{
		}

		public Block(int id, byte[] buffer, byte[] compressedBuffer)
		{
			Id = id;
			Buffer = buffer;
			CompressedBuffer = compressedBuffer;
		}
	}
}