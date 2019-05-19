using System;
using System.Text;
using FileArchiver.Compression;

namespace FileArchiver.Console
{
	internal class Program
	{
		private static int Main(string[] args)
		{
			System.Console.OutputEncoding = Encoding.UTF8;
			var parser = new ParametersParser();
			var settings = new Settings();
			try
			{
				var arguments = parser.ParseArgs(args);
				switch (arguments.Action)
				{
					case Action.Compress:
						var compressor = new GZipCompressor(settings.BlockSize);
						compressor.Do(arguments.InputFileName, arguments.OutputFileName);
						break;
					case Action.Decompress:
						var decompressor = new GZipDecompressor(settings.BlockSize);
						decompressor.Do(arguments.InputFileName, arguments.OutputFileName);
						break;
				}
			}
			catch (ParseArgumentsException e)
			{
				System.Console.WriteLine(e.Message);
				System.Console.WriteLine(e.HelpMsg);
				return 1;
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
				return 1;
			}
			return 0;
		}
	}
}
