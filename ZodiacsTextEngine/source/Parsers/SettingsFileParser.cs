using System.Collections.Generic;

namespace ZodiacsTextEngine.Parsers
{
	public static class SettingsFileParser
	{
		public static SettingsDictionary ParseFile(string[] lines)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			for(int i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				if(string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue; // Skip empty lines and comments
				var splitIndex = line.IndexOf('=');
				if(splitIndex == -1)
				{
					throw new System.Exception($"Invalid setting declaration '{line}' at line {i + 1}. Expected format: 'Key=Value'.");
				}
				var key = line.Substring(0, splitIndex).Trim().ToLower();
				var value = line.Substring(splitIndex + 1).Trim();
				if(string.IsNullOrWhiteSpace(key))
				{
					throw new System.Exception($"Invalid setting declaration '{line}' at line {i + 1}. Key cannot be empty.");
				}
				data[key] = value;
			}
			return new SettingsDictionary(data);
		}
	}
}