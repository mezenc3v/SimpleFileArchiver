using System;
using System.Configuration;

namespace FileArchiver.Console
{
	public class Settings
	{
		private Configuration Config { get; }

		public int BlockSize
		{
			get
			{
				var count = GetValue<int>("blockSize", 1000000);
				return count == 0 ? 100 : count;
			}
		}

		public Settings()
		{
			Config = GetConfig();
		}

		private Configuration GetConfig()
		{
			var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			var location = new Uri(executingAssembly.CodeBase).LocalPath;
			return ConfigurationManager.OpenExeConfiguration(location);
		}

		private T GetValue<T>(string name, T defaultValue)
		{
			try
			{
				var value = Config.AppSettings.Settings[name].Value;
				return (T)(Convert.ChangeType(value, typeof(T)));
			}
			catch
			{
				return defaultValue;
			}
		}
	}
}