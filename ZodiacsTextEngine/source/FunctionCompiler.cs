using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine
{
	public class FunctionCompiler
	{
		private Dictionary<string, string> sourceCodes = new Dictionary<string, string>();

		public int FunctionSourceCount => sourceCodes.Count;

		public void AddFunctionSourcesFromFile(string fileContents)
		{
			var lines = fileContents.Replace("\r", "").Split('\n');
			foreach(var kv in FunctionFileParser.ParseFile(lines))
			{
				string functionName = kv.Key;
				AddFunctionSource(functionName, kv.Value.code, kv.Value.async);
			}
		}

		public void AddFunctionSource(string functionName, string code, bool async)
		{
			if(!ValidateFunctionName(functionName))
			{
				throw new ArgumentException($"Invalid function name: '{functionName}'");
			}
			string source;
			if(async)
			{
				source = $@"
public static async Task<string> {functionName}(FunctionArgs args)
{{
{code}
}}";
			}
			else
			{
				source = $@"
public static string {functionName}(FunctionArgs args)
{{
{code}
}}";
			}
			sourceCodes[functionName] = source;
		}

		public IEnumerable<(string, Functions.FunctionDelegate)> CompileFunctions()
		{
			var assembly = Compile();
			if(sourceCodes.Count == 0)
			{
				throw new InvalidOperationException("No sources were provided to compile.");
			}
			foreach(var kv in sourceCodes)
			{
				string functionName = kv.Key;
				Type type = assembly.GetType("Functions");
				MethodInfo method = type.GetMethod(functionName, BindingFlags.Public | BindingFlags.Static);
				if(method == null)
				{
					throw new InvalidOperationException($"Function '{functionName}' not found in compiled assembly.");
				}
				if(method.GetCustomAttribute<AsyncStateMachineAttribute>() != null)
				{
					// Method is async and matches the FunctionDelegate signature
					yield return Functions.CreateFunction(functionName, (Functions.FunctionDelegate)Delegate.CreateDelegate(typeof(Functions.FunctionDelegate), method));
				}
				else
				{
					// If the method is synchronous, create a delegate for it
					yield return Functions.CreateFunction(functionName, (Func<FunctionArgs, string>)Delegate.CreateDelegate(typeof(Func<FunctionArgs, string>), method));
				}
			}
		}

		public Assembly Compile()
		{
			string functionsSource = "";
			foreach(var kv in sourceCodes)
			{
				string functionName = kv.Key;
				string code = kv.Value;
				functionsSource += code + "\n\n";
			}
			string source =
$@"using System;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZodiacsTextEngine;
using ZodiacsTextEngine.Effects;

public class Functions
{{
private static Variables vars => Session.Current?.variables;
private static ITextInterface window => TextEngine.Interface;
private static Story story => TextEngine.Story;
private static Room room => Session.Current?.currentRoom;

private static async Task GoTo(string roomName) => await Session.Current.GoToRoom(roomName);
private static async Task GoTo(Room room) => await Session.Current.GoToRoom(room);

{functionsSource}
}}";

			var syntaxTree = CSharpSyntaxTree.ParseText(source);
			var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
			var references = new List<MetadataReference>();
			AddMetadataReference(references, typeof(object).Assembly.Location);
			AddMetadataReference(references, typeof(Task).Assembly.Location);
			AddMetadataReference(references, typeof(Enumerable).Assembly.Location);
			AddMetadataReference(references, typeof(TextEngine).Assembly.Location);

			var netstandardPath = Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "netstandard.dll");
			if(File.Exists(netstandardPath))
				AddMetadataReference(references, netstandardPath);
			var systemRuntimePath = Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.dll");
			if(File.Exists(systemRuntimePath))
				AddMetadataReference(references, systemRuntimePath);

			var compilation = CSharpCompilation.Create(
				"DynamicFunctions",
				new[] { syntaxTree },
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
			);

			using(var ms = new MemoryStream())
			{
				var result = compilation.Emit(ms);
				if(!result.Success)
				{
					StringBuilder errorMessages = new StringBuilder();
					foreach(var diagnostic in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
					{
						errorMessages.AppendLine($"Error: {diagnostic.GetMessage()} at {diagnostic.Location}");
					}
					throw new InvalidOperationException($"Compilation failed:\n{errorMessages}\n\nSource:\n\n{source}");
				}
				ms.Seek(0, SeekOrigin.Begin);
				return Assembly.Load(ms.ToArray());
			}
		}

		private void AddMetadataReference(List<MetadataReference> references, string assemblyPath)
		{
			if(assemblyPath == null)
			{
				throw new InvalidOperationException($"assemblyPath is null.");
			}
			if(File.Exists(assemblyPath))
			{
				references.Add(MetadataReference.CreateFromFile(assemblyPath));
			}
			else
			{
				throw new FileNotFoundException($"Assembly not found at '{assemblyPath}'.");
			}
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
