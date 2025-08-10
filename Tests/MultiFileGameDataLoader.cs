using ZodiacsTextEngine;

namespace Tests;

public class MultiFileGameDataLoader : StandardGameDataLoader
{
	public MultiFileGameDataLoader(string path) : base(path, "_start")
	{

	}

	protected override void LoadContent(ref bool success)
	{
		base.LoadContent(ref success);
		AddFunction("assert", EngineTests.AssertFunc);
		AddFunction("fail", EngineTests.FailFunc);
	}
}
