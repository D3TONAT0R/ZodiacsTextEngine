using ZodiacsTextEngine;

namespace Tests;

public class SingleFileGameDataLoader : GameDataLoader
{
	public string path;

	public SingleFileGameDataLoader(string path)
	{
		this.path = path;
	}

	protected override void LoadContent(ref bool success)
	{
		var room = Room.FromFile(path);
		AddRoom(room);
		SetStartRoom(room.name);
		AddFunction("assert", EngineTests.AssertFunc);
		AddFunction("fail", EngineTests.FailFunc);

		AddFunction("test_func", () =>
		{
			TextEngine.Interface.Write("Test Function", true);
		});
		AddFunction("test_func_with_params", (args) =>
		{
			TextEngine.Interface.Write($"Argument 0 {args[0]}, Argument 1 {args[1]}, Argument 2 {args[2]}", true);
			return null;
		});
		AddFunction("test_func_with_params", (args) =>
		{
			TextEngine.Interface.Write($"Argument 0 {args[0]}, Argument 1 {args[1]}, Argument 2 {args[2]}", true);
			return null;
		});
	}
}
