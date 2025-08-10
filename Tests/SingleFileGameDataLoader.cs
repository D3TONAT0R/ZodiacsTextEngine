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
		AddFunction("fail", Assert.Fail);
	}
}
