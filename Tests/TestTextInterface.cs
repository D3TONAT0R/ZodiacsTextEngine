using ZodiacsTextEngine;

namespace Tests;

public class TestTextInterface : ITextInterface
{
	public ConsoleColor ForegroundColor { get; set; }

	public ConsoleColor BackgroundColor { get; set; }

	public Queue<string> Inputs = new Queue<string>();

	public List<string> Output { get; } = new List<string>() { "" };

	public void Initialize(bool debug)
	{
		
	}

	public void SetInputs(params string[] inputs)
	{
		foreach (var input in inputs)
		{
			Inputs.Enqueue(input);
		}
	}

	public void OnLoadError()
	{
		throw new Exception("Load Error");
	}

	public void OnDebugInfo()
	{
		
	}

	public void Clear()
	{
		Output.Clear();
		Output.Add(string.Empty);
	}

	public Task<string> ReadInput()
	{
		if (Inputs.Count == 0)
		{
			throw new InvalidOperationException("No inputs available to read.");
		}
		return Task.FromResult(Inputs.Dequeue());
	}

	public Task<Choice> RequestChoice(Room room)
	{
		return Task.FromResult(room.GetChoice(ReadInput().Result));
	}

	public void ListChoices(List<Choice> choices)
	{
		foreach (var choice in choices)
		{
			Text(choice.identifier);
		}
	}

	public void Text(string text, ConsoleColor? color = null)
	{
		Write(text, true, false);
	}

	public void Write(string text, bool lineBreak, bool wordWrap = true)
	{
		//Write to test console
		if(Output[^1].Length == 0) TestContext.Out.Write((Output.Count - 1).ToString().PadLeft(3) + ": ");
		Output[^1] += text;
		TestContext.Out.Write(text);
		if(lineBreak)
		{
			LineBreak();
		}
	}

	public void LineBreak()
	{
		Output.Add(string.Empty);
		TestContext.Out.WriteLine();
	}

	public void Header(string title)
	{
		Text($"<{title}>", ConsoleColor.Green);
	}

	public void Hint(string text)
	{
		Text($"Hint: {text}", ConsoleColor.Cyan);
	}

	public void VerticalSpace(int count = 1)
	{
		for (int i = 0; i < count; i++)
		{
			Text(string.Empty);
		}
	}

	public Task WaitForInput(bool printLine)
	{
		if(printLine)
		{
			Text("Press any key");
		}
		return Task.CompletedTask;
	}

	public Task Wait(int milliseconds)
	{
		return Task.Delay(milliseconds);
	}

	public void LogWarning(string message)
	{
		Text($"Warning: {message}", ConsoleColor.Yellow);
	}

	public void LogError(string message)
	{
		throw new Exception(message);
	}

	public void ResetColors()
	{
		
	}

	public Task OnGameOver(string text)
	{
		Text("Game Over");
		return Task.CompletedTask;
	}

	public Task Exit()
	{
		//Pass the test if exit is called
		Assert.Pass();
		return Task.CompletedTask;
	}
}