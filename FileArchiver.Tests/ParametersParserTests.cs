using FileArchiver.Console;
using NUnit.Framework;

namespace FileArchiver.Tests
{
	[TestFixture]
	public class ParametersParserTests
	{
		[Test]
		public void ShouldBeParseArgs()
		{
			//arrange
			var testArgs = new[] {"compress", "fileName.txt", "fileName.gzip"};
			var parser = new ParametersParser();
			//act
			var parameters = parser.ParseArgs(testArgs);
			//asserts
			Assert.True(parameters.Action == Action.Compress);
			Assert.AreEqual(testArgs[1], parameters.InputFileName);
			Assert.AreEqual(testArgs[2], parameters.OutputFileName);
		}

		[Test]
		public void ShouldBeThrowParserException()
		{
			//arrange
			var testArgsWithoutOutputFileName = new[] { "compress", "fileName.txt"};
			var testArgsWithIncorrectAction = new[] { "comprress", "fileName.txt", "fileName.gzip" };
			var emptyTestArgs = new string[0];
			var parser = new ParametersParser();
			//act
			//asserts
			Assert.Throws<ParseArgumentsException>(() => parser.ParseArgs(testArgsWithoutOutputFileName));
			Assert.Throws<ParseArgumentsException>(() => parser.ParseArgs(testArgsWithIncorrectAction));
			Assert.Throws<ParseArgumentsException>(() => parser.ParseArgs(emptyTestArgs));
		}
	}
}