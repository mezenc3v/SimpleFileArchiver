namespace FileArchiver.Console
{
	public enum Action
	{
		Compress,
		Decompress,
	}
	public class Parameters
	{
		public Action Action { get; set; }
		public string InputFileName { get; set; }
		public string OutputFileName { get; set; }
	}
}