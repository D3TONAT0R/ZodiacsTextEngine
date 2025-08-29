using System.Collections.Generic;
using ZodiacsTextEngine.Effects;

namespace ZodiacsTextEngine
{
	public static class Rooms
	{
		public static Room GetRoom(string name)
		{
			return TextEngine.Story.Rooms[name];
		}

		public static bool Exists(string name)
		{
			return TextEngine.Story.Rooms.ContainsKey(name);
		}

		public static void Validate(Story story)
		{
			bool headerPrinted = false;
			List<string> writeVars = new List<string>();
			List<string> writeSVars = new List<string>() { "input" }; //input is automatically set by the engine
																	  //Gather all variables used in the room
			foreach(var room in story.Rooms.Values)
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
			foreach(var room in story.Rooms.Values)
			{
				var context = new RoomValidationContext(room, writeVars, writeSVars);
				if(!RoomValidator.Validate(context, out var log))
				{
					if(!headerPrinted)
					{
						TextEngine.Interface.BackgroundColor = Color.DarkRed;
						TextEngine.Interface.ForegroundColor = Color.White;
						TextEngine.Interface.Text("Room validation has found some problems:");
						TextEngine.Interface.BackgroundColor = Color.DefaultBackground;
						TextEngine.Interface.ForegroundColor = Color.DefaultForeground;
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
