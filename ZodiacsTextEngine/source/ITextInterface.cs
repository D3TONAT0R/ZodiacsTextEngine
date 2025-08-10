using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public interface ITextInterface
	{
		ConsoleColor ForegroundColor { get; set; }

		ConsoleColor BackgroundColor { get; set; }

		void Initialize(bool debug);

		void OnLoadError();

		void OnDebugInfo();

		void Clear();

		Task<string> ReadInput();

		Task<Choice> RequestChoice(Room room);

		void ListChoices(List<Choice> choices);

		void Text(string text, ConsoleColor? color = null);

		void Write(string text, bool lineBreak, bool wordWrap = true);

		void LineBreak();

		void Header(string title);

		void Hint(string text);

		void VerticalSpace(int count = 1);

		Task WaitForInput(bool printLine);

		Task Wait(int milliseconds);

		void LogWarning(string message);

		void LogError(string message);

		void ResetColors();

		Task OnGameOver(string text);

		Task Exit();
	}
}
