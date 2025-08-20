using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZodiacsTextEngine.Effects;

namespace ZodiacsTextEngine
{
	public static class TextEngine
	{
		public static ITextInterface Interface { get; private set; } = null;

		public static Story Story
		{
			get
			{
				if(!Initialized && !initializing)
				{
					throw new InvalidOperationException("TextEngine has not yet been initialized.");
				}
				return story;
			}
		}
		private static Story story;

		public static bool DebugMode { get; private set; } = false;

		public static bool Initialized { get; private set; } = false;
		private static bool initializing = false;

		public static async Task Initialize(ITextInterface textInterface, ContentLoader contentLoader, bool debugMode)
		{
			initializing = true;
			if(textInterface == null) throw new NullReferenceException("Null text interface passed to initialization call.");
			Interface = textInterface;
			DebugMode = debugMode;

			Interface.Initialize(DebugMode);
			bool success = true;
			story = contentLoader.Load(ref success);
			if(!DebugMode && !success)
			{
				Interface.OnLoadError();
				return;
			}
			Rooms.Validate(story);
			var startRoom = story.StartRoom;
			if(DebugMode)
			{
				Interface.VerticalSpace();
				ReportWordAndCharacterCounts(story);
				Interface.OnDebugInfo();
				Interface.Text("Start Room ID: " + startRoom.name);
				Interface.VerticalSpace();
				Interface.Hint("[Press any key to start]");
				await Interface.WaitForInput(false);
			}

			Initialized = true;
			initializing = false;
		}

		private static void ReportWordAndCharacterCounts(Story story)
		{
			List<ITextEffect> textEffects = story.Rooms.Values.SelectMany(r => r.EnumerateEffectGroups())
							.Where(g => g != null && g.effects != null)
							.SelectMany(g => g.effects)
							.Where(e => e is ITextEffect)
							.Cast<ITextEffect>().ToList();
			int characterCount = textEffects.SelectMany(e => e.GetTextStrings()).Sum(s => s.Length);
			Interface.Text($"Character count: {characterCount}");
			int wordCount = textEffects.SelectMany(e => e.GetTextStrings()).Sum(s => s.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length);
			Interface.Text($"Word count: {wordCount}");
		}

		public static async Task Start()
		{
			Interface.Clear();
			await Session.StartNew();
		}

		public static async Task RequestChoice()
		{
			Choice choice = await Interface.RequestChoice(Session.Current.currentRoom);
			if(choice == null) throw new NullReferenceException("Null choice was returned.");
			//Interface.Write("", true);
			await choice.Execute();
		}

		public static async Task<string> RequestInput()
		{
			var input = await Interface.ReadInput();
			Session.Current.variables.SetString("input", input);
			return input;
		}
	}
}
