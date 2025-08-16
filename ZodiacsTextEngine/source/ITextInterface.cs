using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public interface ITextInterface
	{
		Color ForegroundColor { get; set; }

		Color BackgroundColor { get; set; }

		void Initialize(bool debug);

		void OnLoadError();

		void OnDebugInfo();

		void Clear();

		Task<string> ReadInput();

		Task<Choice> RequestChoice(Room room);

		void ListChoices(List<Choice> choices);

		void Text(string text, Color? color = null, Color? background = null);

		void Write(string text, bool lineBreak, bool wordWrap = true);

		void LineBreak();

		void Header(string title);

		void Hint(string text);

		void VerticalSpace(int count = 1);

		Task WaitForInput(bool printLine);

		Task Wait(int milliseconds);

		void LogWarning(string message);

		void LogError(string message);

		Task OnGameOver(string text);

		Task Exit();
	}
}
