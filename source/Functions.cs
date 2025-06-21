using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public static class Functions
	{
		private static Dictionary<string, FunctionDelegate> registry = new Dictionary<string, FunctionDelegate>();

		public delegate Task<string> FunctionDelegate(string[] arguments);

		public static void CreateFunctions()
		{
			//Example function
			Register("add", (args) =>
			{
				float a = float.Parse(args[0]);
				float b = float.Parse(args[1]);
				float sum = a + b;
				return Task.FromResult(sum.ToString());
			});
		}

		public static void Register(string id, FunctionDelegate func)
		{
			if(id.Contains(" ")) throw new ArgumentException($"Function ids cannot contain whitespaces");
			if(registry.ContainsKey(id)) throw new ArgumentException($"A function with id '{id}' already exists.");
			registry.Add(id, func);
		}

		public static async Task<string> Execute(string id, string[] arguments)
		{
			if(registry.TryGetValue(id, out var func))
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
			return registry.ContainsKey(id);
		}
	}
}
