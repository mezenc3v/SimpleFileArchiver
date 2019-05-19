using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using FileArchiver.Compression;
using NUnit.Framework;

namespace FileArchiver.Tests
{
	[TestFixture]
	public class FileCompressorTests
	{
		[Test]
		public void ShouldBeCompressAndDecompressFile()
		{
			//arrange
			var testFile = GetTestFile("sample1.txt");
			var compressedFile = Path.Combine(Path.GetTempPath(), "result.gz");
			var decompressedFile = Path.Combine(Path.GetTempPath(), "result.txt");
			var compressor = new GZipCompressor(1000000);
			var decompressor = new GZipDecompressor(1000000);
			//act
			var sw = Stopwatch.StartNew();
			compressor.Do(testFile, compressedFile);
			TestContext.Out.WriteLine($"Время упаковки: {sw.Elapsed}");
			sw.Restart();
			
			decompressor.Do(compressedFile, decompressedFile);
			TestContext.Out.WriteLine($"Время распаковки: {sw.Elapsed}");
			//asserts
			Assert.True(File.Exists(compressedFile));
			Assert.True(File.Exists(decompressedFile));
			Assert.True(FilesAreEquals(testFile, decompressedFile));
		}

		private static bool FilesAreEquals(string inputFile, string decompressedFile)
		{
			using (var md5 = MD5.Create())
			using (var inputFileStream = File.OpenRead(inputFile))
			using (var decompressedFileStream = File.OpenRead(decompressedFile))
			{
				var inputFileSum = md5.ComputeHash(inputFileStream);
				var decompressedFileSum = md5.ComputeHash(decompressedFileStream);
				var hashesAreEquals = false;
				int i = 0;

				while (i < inputFileSum.Length && inputFileSum[i] == decompressedFileSum[i])
				{
					i++;
				}

				if (i == inputFileSum.Length)
				{
					hashesAreEquals = true;
				}

				return hashesAreEquals;
			}
		}

		[Test]
		public void ShouldBeCompressFile()
		{
			//arrange
			var testFile = GetTestFile("sample1.txt");
			var compressedFile = Path.Combine(Path.GetTempPath(), "result.gz");
			var compressor = new GZipCompressor(1000000);
			//act
			compressor.Do(testFile, compressedFile);
			//asserts
			Assert.True(File.Exists(compressedFile));
		}

		[Test]
		public void ShouldBeDecompressFile()
		{
			//arrange
			var testFile = GetTestFile("sample2.gz");
			var decompressedFile = Path.Combine(Path.GetTempPath(), "result.txt");
			var decompressor = new GZipDecompressor(1000000);
			//act
			decompressor.Do(testFile, decompressedFile);
			//asserts
			Assert.True(File.Exists(decompressedFile));
		}

		private static string GetTestFile(string fileName)
		{
			var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var resourcePath = Path.Combine(currentFolder, "Resources");
			return Path.Combine(resourcePath, fileName);
		}
	}
}