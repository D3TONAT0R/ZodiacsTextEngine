using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public static class TextEngine
	{
		public static ITextInterface Interface { get; private set; } = null;

		public static GameData GameData
		{
			get
			{
				if(!Initialized && !initializing)
				{
					throw new InvalidOperationException("TextEngine has not yet been initialized.");
				}
				return gameData;
			}
		}
		private static GameData gameData;

		public static bool DebugMode { get; private set; } = false;

		public static bool Initialized { get; private set; } = false;
		private static bool initializing = false;

		public static async Task Initialize(ITextInterface textInterface, GameDataLoader gameDataLoader, bool debugMode)
		{
			initializing = true;
			if(textInterface == null) throw new NullReferenceException("Null text interface passed to initialization call.");
			Interface = textInterface;
			DebugMode = debugMode;

			Interface.Initialize(DebugMode);
			bool success = true;
			gameData = gameDataLoader.Load(ref success);
			if(!DebugMode && !success)
			{
				Interface.OnLoadError();
				return;
			}
			Rooms.ValidateRooms(gameData);
			var startRoom = gameData.StartRoom;
			if(DebugMode)
			{
				Interface.VerticalSpace();
				ReportWordAndCharacterCounts(gameData);
				Interface.OnDebugInfo();
				Interface.Text("Start Room ID: " + startRoom.name);
				Interface.VerticalSpace();
				Interface.Hint("[Press any key to start the game]");
				await Interface.WaitForInput(false);
			}

			Initialized = true;
			initializing = false;
		}

		private static void ReportWordAndCharacterCounts(GameData gameData)
		{
			List<ITextEffect> textEffects = gameData.Rooms.Values.SelectMany(r => r.EnumerateEffectGroups())
							.Where(g => g != null && g.effects != null)
							.SelectMany(g => g.effects)
							.Where(e => e is ITextEffect)
							.Cast<ITextEffect>().ToList();
			int characterCount = textEffects.SelectMany(e => e.GetTextStrings()).Sum(s => s.Length);
			Interface.Text($"Character count: {characterCount}");
			int wordCount = textEffects.SelectMany(e => e.GetTextStrings()).Sum(s => s.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length);
			Interface.Text($"Word count: {wordCount}");
		}

		public static async Task StartGame()
		{
			Interface.Clear();
			await GameSession.StartNew();
		}

		public static async Task RequestChoice()
		{
			Choice choice = await Interface.RequestChoice(GameSession.Current.currentRoom);
			if(choice == null) throw new NullReferenceException("Null choice was returned.");
			Interface.Write("", true);
			await choice.Execute();
		}
	}
}
