using System.Collections.Generic;
using System.Text;
using ZodiacsTextEngine.Effects;

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
			TextEngine.Interface.Text(sb.ToString(), isError ? Color.Red : Color.DarkYellow);
			TextEngine.Interface.ForegroundColor = Color.DefaultForeground;
		}
	}

	public static class ContentValidator
	{

		public static void ValidateStory(Story story)
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
						if(!writeVars.Contains(mv.variableName)) writeVars.Add(mv.variableName);
					}
					else if(effect is ModifyStringVariable msv)
					{
						if(!writeSVars.Contains(msv.variableName)) writeSVars.Add(msv.variableName);
					}
				}
			}
			foreach(var room in story.Rooms.Values)
			{
				var context = new RoomValidationContext(room, writeVars, writeSVars);
				if(!ValidateRoom(context, out var log))
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

		public static bool ValidateRoom(RoomValidationContext ctx, out List<LogMessage> log)
		{
			log = new List<LogMessage>();
			if(ctx.room.onEnter != null && ctx.room.onEnter.effects.Count > 0)
			{
				log.AddRange(ValidateEffectGroup(ctx.room.onEnter, ctx));
			}
			else
			{
				log.Add(LogMessage.Error(ctx.room.name, "Room has no onEnter event (or is empty)"));
			}
			if(ctx.room.onExit != null) log.AddRange(ValidateEffectGroup(ctx.room.onExit, ctx));

			foreach(var choice in ctx.room.choices)
			{
				log.AddRange(ValidateEffectGroup(choice, ctx));
			}

			return log.Count == 0;
		}

		public static IEnumerable<LogMessage> ValidateEffectGroup(EffectGroup effectGroup, RoomValidationContext ctx)
		{
			bool hasColor = false;
			bool ended = false;
			bool hasUnreachableEffects = false;
			var list = new List<Effect>();
			effectGroup.ListAllEffects(list);
			foreach(var e in list)
			{
				if(ended)
				{
					//hasUnreachableEffects = true;
					//while(effectGroup.effects.Count > i) effectGroup.effects.RemoveAt(i);
					//break;
				}

				if(e == null)
				{
					yield return LogMessage.Error(effectGroup.identifier, "Null effect found in effect group " + effectGroup.identifier);
				}

				//Check for unused variables
				if(e is WriteRichText w)
				{
					foreach(var comp in w.text.components)
					{
						if(comp is VariableComponent vr)
						{
							if(!ctx.definedVars.Contains(vr.variableName))
							{
								yield return LogMessage.Error(effectGroup.identifier, $"Use of undefined variable '{vr.variableName}' in Rich Text");
							}
						}
						else if(comp is StringVariableComponent sv)
						{
							if(!ctx.definedSVars.Contains(sv.variableName))
							{
								yield return LogMessage.Error(effectGroup.identifier, $"Use of undefined string variable '{sv.variableName}' in Rich Text");
							}
						}
					}
				}
				if(e is ModifyIntVariable mv)
				{
					if(mv.value is VariableRef vr && !ctx.definedVars.Contains(vr.variableName))
					{
						yield return LogMessage.Error(effectGroup.identifier, $"Use of undefined variable '{mv.variableName}' in VAR Effect");
					}
				}
				else if(e is ModifyStringVariable msv)
				{
					if(msv.value is VariableRef vr && !ctx.definedSVars.Contains(vr.variableName))
					{
						yield return LogMessage.Error(effectGroup.identifier, $"Use of undefined string variable '{msv.variableName}' in SVAR Effect");
					}
				}
				if(e is ConditionalEffectBlock conditional)
				{
					var cond = conditional.condition;
					if(cond.VariableType == VariableType.Int)
					{
						//Check if int variable is defined
						if(!ctx.definedVars.Contains(cond.variableName))
						{
							yield return LogMessage.Error(effectGroup.identifier, $"Use of undefined variable '{cond.variableName}' in condition");
						}
						if(cond.compareValue is VariableRef vr && !ctx.definedVars.Contains(vr.variableName))
						{
							yield return LogMessage.Error(effectGroup.identifier, $"Use of undefined variable '{vr.variableName}' in condition");
						}
					}
					else if(cond.VariableType == VariableType.String)
					{
						//Check if string variable is defined
						if(!ctx.definedSVars.Contains(cond.variableName))
						{
							yield return LogMessage.Error(effectGroup.identifier, $"Use of undefined string variable '{cond.variableName}' in condition");
						}
						if(cond.compareValue is VariableRef sv && !ctx.definedSVars.Contains(sv.variableName))
						{
							yield return LogMessage.Error(effectGroup.identifier, $"Use of undefined string variable '{sv.variableName}' in condition");
						}
					}
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
						//TODO: Reintroduce automatic color reset fix?
						//effectGroup.effects.Insert(i, new ResetColor());
						//i++;
						hasColor = false;
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
