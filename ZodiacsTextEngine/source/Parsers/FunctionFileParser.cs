using System.Collections.Generic;

namespace ZodiacsTextEngine.Parsers
{
	public struct FunctionSource
	{
		public string name;
		public string code;
		public bool async;

		public FunctionSource(string name, string code, bool async = false)
		{
			this.name = name;
			this.code = code;
			this.async = async;
		}
	}

	public static class FunctionFileParser
	{
		public static Dictionary<string, FunctionSource> ParseFile(string[] lines)
		{
			Dictionary<string, FunctionSource> functions = new Dictionary<string, FunctionSource>();
			string currentFunctionName = null;
			bool currentFunctionAsync = false;
			List<string> currentFunctionStrings = new List<string>();
			for(int i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				if(line.StartsWith("#"))
				{
					if(currentFunctionName != null)
					{
						functions.Add(currentFunctionName, new FunctionSource(currentFunctionName, string.Join("\n", currentFunctionStrings), currentFunctionAsync));
					}
					currentFunctionStrings.Clear();
					var definition = line.Substring(1).Trim();
					currentFunctionAsync = false;
					var split = definition.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
					currentFunctionName = split[0];
					if(split.Length > 1)
					{
						if(split[1].ToLower() == "async")
						{
							currentFunctionAsync = true;
						}
						else
						{
							throw new System.Exception($"Invalid function declaration '{line}' at line {i + 1}. Expected format: '# FunctionName [async]'.");
						}
					}
				}
				else
				{
					if(!string.IsNullOrWhiteSpace(line))
					{
						currentFunctionStrings.Add(line.Trim());
					}
				}
			}
			//Add the last function if it exists
			if(currentFunctionName != null)
			{
				functions.Add(currentFunctionName, new FunctionSource(currentFunctionName, string.Join("\n", currentFunctionStrings), currentFunctionAsync));
			}
			return functions;
		}
	}
}