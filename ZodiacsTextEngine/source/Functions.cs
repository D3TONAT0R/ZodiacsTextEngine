using System;
using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public static class Functions
	{
		public delegate Task<string> FunctionDelegate(FunctionArgs args);

		public static async Task<string> Execute(string id, FunctionArgs args)
		{
			if(TryGet(id, out var func))
			{
				var result = await func.Invoke(args);
				return result;
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

		public static (string, FunctionDelegate) CreateFunction(string id, Func<string> func)
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

		public static (string, FunctionDelegate) CreateFunction(string id, Action<FunctionArgs> func)
		{
			return (id, args =>
				{
					func.Invoke(args);
					return Task.FromResult<string>(null);
				}
			);
		}

		public static (string, FunctionDelegate) CreateFunction(string id, Func<FunctionArgs, string> func)
		{
			return (id, args =>
				{
					return Task.FromResult(func.Invoke(args));
				}
			);
		}

		public static (string, FunctionDelegate) CreateFunction(string id, FunctionDelegate func)
		{
			return (id, func);
		}
	}
}
