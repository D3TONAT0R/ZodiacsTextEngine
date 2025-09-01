using System;

namespace ZodiacsTextEngine
{
	public class StoryMetadata
	{
		public string Title { get; private set; } = "Untitled";
		public string[] Authors { get; private set; } = Array.Empty<string>();
		public string Version { get; private set; } = "1.0";
		public string Description { get; private set; } = "";

		public static StoryMetadata FromFile(string[] lines, string rootFileName)
		{
			var dict = Parsers.SettingsFileParser.ParseFile(lines);
			var meta = new StoryMetadata
			{
				Title = dict.Get("title", rootFileName),
				Authors = dict.GetArray("authors", "Unknown"),
				Version = dict.Get("version", "1.0"),
				Description = dict.Get("description", "")
			};
			return meta;
		}

		public static StoryMetadata CreateBlank(string title)
		{
			return new StoryMetadata
			{
				Title = title
			};
		}
	}
}