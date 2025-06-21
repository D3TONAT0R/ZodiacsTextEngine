using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public static class TextEngine
	{
		public delegate IEnumerable<RoomFile> RoomDataLoader();

		public static ITextInterface Interface { get; private set; } = null;

		public static bool DebugMode { get; private set; } = false;

		public static async Task Initialize(ITextInterface textInterface, RoomDataLoader roomDataLoader, bool debugMode, string startRoomName, List<string> fixedVariableNames)
		{
			if(textInterface == null) throw new NullReferenceException("Null text interface passed to initialization call.");
			Interface = textInterface;
			DebugMode = debugMode;

			Interface.Initialize(DebugMode);
			Variables.Init(fixedVariableNames);
			bool success = Rooms.LoadRooms(roomDataLoader);
			if(!DebugMode && !success)
			{
				Interface.OnLoadError();
				return;
			}
			Functions.CreateFunctions();
			Rooms.ValidateRooms();
			if(DebugMode)
			{
				Interface.VerticalSpace();
				ReportWordAndCharacterCounts();
				Interface.OnDebugInfo();
				Interface.VerticalSpace();
				Interface.Hint("[Press any key to start the game]");
				await Interface.WaitForInput(false);
			}
			if(Rooms.Exists(startRoomName))
			{
				Rooms.StartRoom = Rooms.GetRoom(startRoomName);
			}
			else
			{
				throw new NullReferenceException($"Start room '{startRoomName}' not found.");
			}
		}

		private static void ReportWordAndCharacterCounts()
		{
			List<ITextEffect> textEffects = Rooms.All.SelectMany(r => r.EnumerateEffectGroups())
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
