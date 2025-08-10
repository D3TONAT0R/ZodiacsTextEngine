using System;
using ZodiacsTextEngine;

namespace Tests
{
	public class EngineTests
	{
		private TestTextInterface console;

		private string BaseDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TestFiles");

		[OneTimeSetUp]
		public void Init()
		{
			console = new TestTextInterface();
		}

		[SetUp]
		public void Setup()
		{
			console.Clear();
			console.SetInputs();
		}


		#region Test cases

		[Test]
		public void BasicRoomTest()
		{
			RunTest(
				SingleFile("test_room"),
				//Inputs
				["test", "exit"],
				//Expected output
				["Test room", "A"]
			);
		}

		[Test]
		public void TestConditionals()
		{
			RunTest(
				SingleFile("conditionals"),
				//Inputs
				[
					//0 (default value)
					"3",
					"7",
					"-1",
					"8",
					"exit"
				],
				//Expected output
				[
					"x is 0", "!>5", "!=7", "0", //0
					"x is 3", "!>5", "!=7", ">1", //3
					"x is 7", ">5", "==7", ">1", //7
					"x is -1", "!>5", "!=7", "<0", //-1
					"x is 8", ">5", "!=7", ">1", //8
				]
			);
		}

		[Test]
		public void TestVariables()
		{
			RunTest(
				SingleFile("variables"),
				//Inputs
				null,
				//Expected output
				null
			);
		}

		#endregion

		public static Task<string> AssertFunc(string[] args)
		{
			string var = args[0];
			int expected = int.Parse(args[1]);
			Assert.That(GameSession.Current.variables.GetInt(var), Is.EqualTo(expected), $"Variable '{var}' should be {expected}");
			return Task.FromResult($"Assertion passed ({var} == {expected})");
		}

		private SingleFileGameDataLoader SingleFile(string path)
		{
			return new SingleFileGameDataLoader(Path.Combine(BaseDirectory, path + ".txt"));
		}

		private StandardGameDataLoader Directory(string path, string start = "_start")
		{
			return new StandardGameDataLoader(Path.Combine(BaseDirectory, path), start);
		}

		private void RunTest(GameDataLoader gameData, List<string>? inputs, List<string>? expectedOutput)
		{
			// Run synchronously
			TextEngine.Initialize(console, gameData, false).GetAwaiter().GetResult();
			if(inputs != null) console.SetInputs(inputs.ToArray());
			//expectedOutput.Insert(0, "Test start");
			TextEngine.StartGame().GetAwaiter().GetResult();
			if(expectedOutput != null) CheckOutput(expectedOutput.ToArray());
		}

		private void CheckOutput(params string[] expectedOutput)
		{
			var actual = new List<string>(console.Output);
			actual.RemoveAt(actual.Count - 1); // Remove the last empty line
			Assert.That(actual.ToArray(), Is.EqualTo(expectedOutput));
		}
	}
}