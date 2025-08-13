using ZodiacsTextEngine;

namespace Tests;

public abstract class TestGameDataLoader : StandardGameDataLoader
{
	protected TestGameDataLoader(string rootDirectory, string startRoomName) : base(rootDirectory, startRoomName)
	{

	}

	protected override IEnumerable<(string, Functions.FunctionDelegate)> LoadFunctions()
	{
		yield return CreateFunction("assert", EngineTests.AssertFunc);
		yield return CreateFunction("fail", EngineTests.FailFunc);
		yield return CreateFunction("test_func", (args) =>
			{
				return "Test Function";
			}
		);
		yield return CreateFunction("test_func_with_params", (args) =>
			{
				return $"A {args[0]}, B {args[1]}, C {args[2]}";
			}
		);
		yield return CreateFunction("my_function", (args) =>
			{
				return $"Hello {string.Join(' ', args)}";
			}
		);
	}
}

public class SingleFileGameDataLoader : TestGameDataLoader
{
	public string path;

	public SingleFileGameDataLoader(string path) : base(null, null)
	{
		this.path = path;
	}

	protected override IEnumerable<Room> LoadRooms()
	{
		var room = Room.FromFile(gameData, path);
		yield return room;
		SetStartRoom(room.name);
	}
}

public class MultiFileGameDataLoader : StandardGameDataLoader
{
	public MultiFileGameDataLoader(string path) : base(path, "_start")
	{

	}
}
