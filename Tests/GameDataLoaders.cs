using ZodiacsTextEngine;

namespace Tests;

public abstract class TestContentLoader : StandardContentLoader
{
	protected TestContentLoader(string rootDirectory, string startRoomName) : base(rootDirectory, startRoomName)
	{

	}

	protected override IEnumerable<(string, Functions.FunctionDelegate)> LoadFunctions()
	{
		yield return Functions.CreateFunction("assert", EngineTests.AssertFunc);
		yield return Functions.CreateFunction("fail", EngineTests.FailFunc);
		yield return Functions.CreateFunction("test_func", (args) =>
			{
				return "Test Function";
			}
		);
		yield return Functions.CreateFunction("test_func_with_params", (args) =>
			{
				return $"A {args[0]}, B {args[1]}, C {args[2]}";
			}
		);
		yield return Functions.CreateFunction("my_function", (args) =>
			{
				return $"Hello {string.Join(' ', args.args)}";
			}
		);
	}
}

public class SingleFileContentLoader : TestContentLoader
{
	public string path;
	public Func<IEnumerable<(string, Functions.FunctionDelegate)>>? loadFunctions;

	public SingleFileContentLoader(string path, Func<IEnumerable<(string, Functions.FunctionDelegate)>>? loadFunctions = null) : base(null, null)
	{
		this.path = path;
		this.loadFunctions = loadFunctions;
	}

	protected override IEnumerable<(string, Functions.FunctionDelegate)> LoadFunctions()
	{
		foreach(var func in base.LoadFunctions())
		{
			yield return func;
		}
		if(loadFunctions != null)
		{
			foreach(var func in loadFunctions.Invoke())
			{
				yield return func;
			}
		}
	}

	protected override IEnumerable<Room> LoadRooms()
	{
		var room = Room.Parse(story, path, File.ReadAllText(path));
		yield return room;
		SetStartRoom(room.name);
	}
}

public class MultiFileContentLoader : StandardContentLoader
{
	public MultiFileContentLoader(string path) : base(path, "_start")
	{

	}
}
