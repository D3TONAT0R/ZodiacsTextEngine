using System;
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
				TextEngine.Interface.LogError($"Attempted to call unknown function '{id}'");
				if(TextEngine.DebugMode)
				{
					await TextEngine.Interface.WaitForInput(true);
				}
				return null;
			}
		}

		public static bool Exists(string id)
		{
			return TextEngine.GameData.Functions.ContainsKey(id);
		}
	}
}
