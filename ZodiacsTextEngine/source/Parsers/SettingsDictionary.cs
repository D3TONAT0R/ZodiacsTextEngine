using System.Collections.Generic;

namespace ZodiacsTextEngine.Parsers
{
	public class SettingsDictionary
	{
		public readonly Dictionary<string, string> data;

		public SettingsDictionary(Dictionary<string, string> data)
		{
			this.data = data;
		}

		public string Get(string key, string defaultValue = null)
		{
			if(data.TryGetValue(key.ToLower(), out var value))
			{
				return value;
			}
			return defaultValue;
		}

		public string[] GetArray(string key, string defaultValue = null)
		{
			if(!data.TryGetValue(key.ToLower(), out var value)) value = defaultValue;
			var parts = value.Split(',');
			for(int i = 0; i < parts.Length; i++)
			{
				parts[i] = parts[i].Trim();
			}
			return parts;
		}

		public int GetInt(string key, int defaultValue = 0)
		{
			if(data.TryGetValue(key.ToLower(), out var value))
			{
				if(int.TryParse(value, out var intValue))
				{
					return intValue;
				}
			}
			return defaultValue;
		}

		public bool GetBool(string key, bool defaultValue = false)
		{
			if(data.TryGetValue(key.ToLower(), out var value))
			{
				if(bool.TryParse(value, out var boolValue))
				{
					return boolValue;
				}
				else if(int.TryParse(value, out var intValue))
				{
					return intValue > 0;
				}
			}
			return defaultValue;
		}
	}
}