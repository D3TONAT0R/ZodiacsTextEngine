using System;
using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public static class Functions
	{
		public delegate Task<string> FunctionDelegate(string[] arguments);

		public static async Task<string> Execute(string id, string[] arguments)
		{
			if(TryGet(id, out var func))
			{
				var task = func.Invoke(arguments ?? Array.Empty<string>());
				if(task != null) return await task;
				return Task.FromResult<string>(null).Result; // If the function returns null, return null
			}
			else
			{
				TextEngine.Interface.LogError($"Attempted to call unknown function '{id}'");
				if(TextEngine.DebugMode)
				{
					await TextEngine.Interface.WaitForInput(true);
				}
				return Task.FromResult<string>(null).Result; // Return null if the function does not exist
			}
		}

		public static FunctionDelegate Get(string id)
		{
			return TextEngine.Story.Functions[id.ToLower()];
		}

		public static bool TryGet(string id, out FunctionDelegate function)
		{
			return TextEngine.Story.Functions.TryGetValue(id.ToLower(), out function);
		}

		public static bool Exists(string id)
		{
			return TextEngine.Story.Functions.ContainsKey(id.ToLower());
		}

		public static (string, FunctionDelegate) CreateFunction(string id, Action func)
		{
			return (id, _ =>
					{
						func.Invoke();
						return Task.FromResult<string>(null);
					}
				);
		}

		public static(string, FunctionDelegate) CreateFunction(string id, Func<string> func)
		{
			return (id, _ => Task.FromResult(func.Invoke()));
		}

		public static (string, FunctionDelegate) CreateFunction(string id, Func<Task<string>> func)
		{
			return (id, async _ =>
					{
						await func.Invoke();
						return null;
					}
				);
		}

		public static (string, FunctionDelegate) CreateFunction(string id, Action<string[]> func)
		{
			return (id, args =>
					{
						func.Invoke(args);
						return Task.FromResult<string>(null);
					}
				);
		}

		public static (string, FunctionDelegate) CreateFunction(string id, Func<string[], string> func)
		{
			return (id, args =>
					{
						return Task.FromResult(func.Invoke(args));
					}
				);
		}

		public static  (string, FunctionDelegate) CreateFunction(string id, FunctionDelegate func)
		{
			return (id, func);
		}
	}
}
