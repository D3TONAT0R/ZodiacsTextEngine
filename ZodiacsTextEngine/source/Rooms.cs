using System;
using System.Collections.Generic;
using System.Linq;
using ZodiacsTextEngine.Effects;

namespace ZodiacsTextEngine
{
	public static class Rooms
	{
		public static Room GetRoom(string name)
		{
			return TextEngine.GameData.Rooms[name];
		}

		public static bool Exists(string name)
		{
			return TextEngine.GameData.Rooms.ContainsKey(name);
		}

		public static void Validate(GameData gameData)
		{
			bool headerPrinted = false;
			List<string> writeVars = new List<string>();
			List<string> writeSVars = new List<string>();
			//Gather all variables used in the room
			foreach(var room in gameData.Rooms.Values)
			{
				var effects = room.ListAllEffects();
				foreach(var effect in effects)
				{
					if(effect is ModifyIntVariable mv)
					{
						AddUnique(writeVars, mv.variableName);
					}
					else if(effect is ModifyStringVariable msv)
					{
						AddUnique(writeSVars, msv.variableName);
					}
				}
			}
			foreach(var room in gameData.Rooms.Values)
			{
				var context = new RoomValidationContext(room, writeVars, writeSVars);
				if(!RoomValidator.Validate(context, out var log))
				{
					if(!headerPrinted)
					{
						TextEngine.Interface.BackgroundColor = ConsoleColor.DarkRed;
						TextEngine.Interface.ForegroundColor = ConsoleColor.White;
						TextEngine.Interface.Text("Room validation has found some problems:");
						TextEngine.Interface.ResetColors();
						headerPrinted = true;
					}
					foreach(var msg in log)
					{
						msg.Print();
					}
					log.AddRange(log);
				}
			}
		}

		private static void AddUnique<T>(List<T> list, T value)
		{
			if(!list.Contains(value)) list.Add(value);
		}
	}
}
