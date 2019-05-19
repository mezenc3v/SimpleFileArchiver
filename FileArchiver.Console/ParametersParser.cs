using System;

namespace FileArchiver.Console
{
	public class ParametersParser
	{
		private const string IncorrectCountArgsMsg = "Неверно указаны аргументы";
		private const string IncorrectActionArgMsg = "Неверно указано действие";

		public Parameters ParseArgs(string[] args)
		{
			if (args.Length < 3)
				throw new ParseArgumentsException(IncorrectCountArgsMsg);

			var parameters = new Parameters();

			try
			{
				parameters.Action = ParseEnum<Action>(args[0]);
				parameters.InputFileName = args[1];
				parameters.OutputFileName = args[2];
			}
			catch (ArgumentException)
			{
				throw new ParseArgumentsException(IncorrectActionArgMsg);
			}

			return parameters;
		}

		private static T ParseEnum<T>(string value)
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}
	}
	public class ParseArgumentsException : Exception
	{
		public string HelpMsg => "Пример использования: GZipTest.exe compress/decompress [имя исходного файла] [имя результирующего файла]";
		public ParseArgumentsException(string message) : base(message)
		{
		}
	}
}