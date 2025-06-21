using System;
using System.Collections.Generic;
using System.Text;

namespace ZodiacsTextEngine
{
	public class LogMessage
	{
		public string site;
		public bool isError;
		public string message;

		private LogMessage(bool isError, string site, string message)
		{
			this.site = site;
			this.isError = isError;
			this.message = message;
		}

		public static LogMessage Error(string site, string message)
		{
			return new LogMessage(true, site, message);
		}

		public static LogMessage Warning(string site, string message)
		{
			return new LogMessage(false, site, message);
		}

		public void Print()
		{
			var sb = new StringBuilder();
			sb.Append(site + ": ");
			sb.Append(isError ? "[ERROR] " : "[WARNING] ");
			sb.Append(message);
			TextEngine.Interface.Text(sb.ToString(), isError ? ConsoleColor.Red : ConsoleColor.DarkYellow);
			TextEngine.Interface.ResetColors();
		}
	}

	public static class RoomValidator
	{

		public static bool Validate(Room room, out List<LogMessage> log)
		{
			log = new List<LogMessage>();
			if(room.onEnter != null && room.onEnter.effects.Count > 0)
			{
				log.AddRange(ValidateEffectGroup(room.onEnter));
			}
			else
			{
				log.Add(LogMessage.Error(room.name, "Room has no onEnter event (or is empty)"));
			}
			if(room.onExit != null) log.AddRange(ValidateEffectGroup(room.onExit));

			foreach(var choice in room.choices)
			{
				log.AddRange(ValidateEffectGroup(choice));
			}

			return log.Count == 0;
		}

		public static IEnumerable<LogMessage> ValidateEffectGroup(EffectGroup effectGroup)
		{
			bool hasColor = false;
			bool ended = false;
			bool hasUnreachableEffects = false;
			for(int i = 0; i < effectGroup.effects.Count; i++)
			{
				if(ended)
				{
					//hasUnreachableEffects = true;
					//while(effectGroup.effects.Count > i) effectGroup.effects.RemoveAt(i);
					//break;
				}
				var e = effectGroup.effects[i];

				if(effectGroup.effects[i] == null)
				{
					yield return LogMessage.Error(effectGroup.identifier, "Null effect at index " + i);
				}

				//Validate effect
				var msg = e.Validate(effectGroup.identifier);
				if(msg != null) yield return msg;

				//Validate condition
				//msg = e.condition?.Validate(effectGroup.identifier);
				//if(msg != null) yield return msg;

				if(e is SetColor || e is SetBackgroundColor)
				{
					hasColor = true;
				}
				if(e is ResetColor)
				{
					hasColor = false;
				}
				if(e is GoToRoom || e is GameOver)
				{
					ended = true;
					if(hasColor)
					{
						yield return LogMessage.Warning(effectGroup.identifier, "Color is not reset before leaving the room");
						effectGroup.effects.Insert(i, new ResetColor());
						hasColor = false;
						i++;
					}
				}
			}
			if(hasUnreachableEffects)
			{
				yield return LogMessage.Warning(effectGroup.identifier, "Unreachable effects found (after GoToRoom or GameOver)");
			}
			if(hasColor)
			{
				yield return LogMessage.Warning(effectGroup.identifier, "Color is not reset at end of group");
				effectGroup.effects.Add(new ResetColor());
			}
		}
	}
}
