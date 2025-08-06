using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public static class Functions
	{
		public delegate Task<string> FunctionDelegate(string[] arguments);

		public static async Task<string> Execute(string id, string[] arguments)
		{
			if(TextEngine.GameData.Functions.TryGetValue(id, out var func))
			{
				return await func.Invoke(arguments ?? Array.Empty<string>());
			}
			else
			{
				TextEngine.Interface.LogError($"Function with id '{id}' does not exist.");
				return null;
			}
		}

		public static bool Exists(string id)
		{
			return TextEngine.GameData.Functions.ContainsKey(id);
		}
	}
}
