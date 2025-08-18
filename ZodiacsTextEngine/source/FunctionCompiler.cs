using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace ZodiacsTextEngine
{
	public class FunctionCompiler
	{

		private Dictionary<string, string> sourceCodes = new Dictionary<string, string>();

		public void AddFunctionSource(string functionName, string code)
		{
			if(!ValidateFunctionName(functionName))
			{
				throw new ArgumentException($"Invalid function name: '{functionName}'");
			}
			sourceCodes[functionName] = code;
		}

		public IEnumerable<(string, Functions.FunctionDelegate)> CompileFunctions()
		{
			var assembly = Compile();
			foreach(var kv in sourceCodes)
			{
				string functionName = kv.Key;
				Type type = assembly.GetType("Functions");
				MethodInfo method = type.GetMethod(functionName, BindingFlags.Public | BindingFlags.Static);
				if(method == null)
				{
					throw new InvalidOperationException($"Function '{functionName}' not found in compiled assembly.");
				}
				yield return Functions.CreateFunction(functionName, (Func<string[], string>)Delegate.CreateDelegate(typeof(Func<string[], string>), method));
			}
		}

		public Assembly Compile()
		{
			string functionsSource = "";
			foreach(var kv in sourceCodes)
			{
				string functionName = kv.Key;
				string code = kv.Value;
				functionsSource += FunctionCodeToString(functionName, code) + "\n\n";
			}
			string source = $@"
				using System;
				using System.Collections.Generic;
				using System.Threading.Tasks;
				using ZodiacsTextEngine;
				using ZodiacsTextEngine.Effects;

				public class Functions
				{{
					{functionsSource}
				}}
			";

			CSharpCodeProvider csc = new CSharpCodeProvider();
			CompilerParameters parameters = new CompilerParameters
			{
				GenerateExecutable = false,
				GenerateInMemory = true,
				ReferencedAssemblies = { "System.dll", "ZodiacsTextEngine.dll" }
			};

			var results = csc.CompileAssemblyFromSource(parameters, source);
			if(results.Errors.Count > 0)
			{
				StringBuilder errorMessages = new StringBuilder();
				foreach(CompilerError error in results.Errors)
				{
					errorMessages.AppendLine($"Error {error.ErrorNumber}: {error.ErrorText} at line {error.Line}");
				}
				throw new InvalidOperationException($"Compilation failed:\n{errorMessages}");
			}
			else
			{
				return results.CompiledAssembly;
			}
		}

		private string FunctionCodeToString(string functionName, string code)
		{
			return $@"
				public static void {functionName}(string[] args)
				{{
					{code}
				}}
			";
		}

		private bool ValidateFunctionName(string functionName)
		{
			if(string.IsNullOrWhiteSpace(functionName))
			{
				return false; // Function name cannot be empty
			}
			if(!char.IsLetter(functionName[0]))
			{
				return false; // Function name must start with a letter or underscore
			}
			foreach(var c in functionName)
			{
				if(!char.IsLetterOrDigit(c) && c != '_')
				{
					return false; // Invalid character found
				}
			}
			return true; // Function name is valid
		}
	}
}
