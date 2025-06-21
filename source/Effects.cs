using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static ZodiacsTextEngine.TextEngine;

namespace ZodiacsTextEngine
{
	public interface ITextEffect
	{
		IEnumerable<string> GetTextStrings();
	}

	public abstract class Effect
	{
		public ConditionalEffectBlock parent;

		public abstract Task Execute(EffectGroup g);

		public virtual LogMessage Validate(string site)
		{
			return null;
		}

		public override string ToString()
		{
			return GetType().Name;
		}
	}

	//TODO: word counter breaks
	public class ConditionalEffectBlock : Effect, ITextEffect
	{
		public Condition condition;
		//public bool inverted;

		public List<Effect> childEffects = new List<Effect>();
		public List<Effect> invertedChildEffects;

		public bool HasInvertedBlock => invertedChildEffects != null;

		public ConditionalEffectBlock(Condition condition)
		{
			this.condition = condition;
			//this.inverted = inverted;
		}

		public void AddChild(Effect effect)
		{
			childEffects.Add(effect);
			effect.parent = this;
		}

		public void BeginElseBlock()
		{
			invertedChildEffects = new List<Effect>();
		}

		public void AddInvertedChild(Effect effect)
		{
			invertedChildEffects.Add(effect);
			effect.parent = this;
		}

		public override LogMessage Validate(string site)
		{
			var msg = condition.Validate(site);
			if(msg != null) return msg;

			foreach(var e in childEffects)
			{
				msg = e.Validate(site);
				if(msg != null) return msg;
			}
			return null;
		}

		public override async Task Execute(EffectGroup g)
		{
			bool passed = condition.Check();
			if(passed)
			{
				foreach(var e in childEffects)
				{
					await e.Execute(g);
				}
			}
			else if(invertedChildEffects != null)
			{
				foreach(var e in invertedChildEffects)
				{
					await e.Execute(g);
				}
			}
		}

		public IEnumerable<string> GetTextStrings()
		{
			foreach(var e in childEffects)
			{
				if(e is ITextEffect t)
				{
					foreach(var s in t.GetTextStrings()) yield return s;
				}
			}
			if(invertedChildEffects != null)
			{
				foreach(var e in invertedChildEffects)
				{
					if(e is ITextEffect t)
					{
						foreach(var s in t.GetTextStrings()) yield return s;
					}
				}
			}
		}

		public override string ToString()
		{
			if(invertedChildEffects != null) return $"{condition} ? ({childEffects.Count} Effects) : ({invertedChildEffects?.Count} Effects)";
			else return $"{condition} ? ({childEffects.Count} Effects)";
		}
	}

	public class TextWriter : Effect, ITextEffect
	{
		public string[] lines;

		public TextWriter(params string[] lines)
		{
			this.lines = lines;
		}

		public override Task Execute(EffectGroup g)
		{
			foreach(var line in lines)
			{
				Interface.Write(line, true);
			}
			return Task.CompletedTask;
		}

		public IEnumerable<string> GetTextStrings()
		{
			yield return string.Join("\n", lines);
		}
	}

	public class RichTextWriter : Effect, ITextEffect
	{
		public RichText text;

		public RichTextWriter(RichText text)
		{
			this.text = text;
		}

		public override Task Execute(EffectGroup g)
		{
			text.Write();
			return Task.CompletedTask;
		}

		public IEnumerable<string> GetTextStrings()
		{
			yield return string.Join(" ", text.components.Select(c => c.text));
		}
	}

	public class Space : Effect
	{
		int count;

		public Space(int count)
		{
			this.count = count;
		}

		public override Task Execute(EffectGroup g)
		{
			Interface.VerticalSpace(count);
			return Task.CompletedTask;
		}
	}

	public class GoToRoom : Effect
	{
		string nextRoomName;

		public GoToRoom(string nextRoomName)
		{
			this.nextRoomName = nextRoomName;
		}

		public override async Task Execute(EffectGroup g)
		{
			await GameSession.Current.GoToRoom(nextRoomName);
		}

		public override LogMessage Validate(string site)
		{
			if(!Rooms.Exists(nextRoomName)) return LogMessage.Error(site, "Nonexisting room referenced: " + nextRoomName);
			return null;
		}
	}

	public class GameOver : Effect
	{
		public string text;

		public GameOver(string text)
		{
			this.text = text;
		}

		public override async Task Execute(EffectGroup g)
		{
			await Interface.OnGameOver(text);
		}
	}

	public class WaitForAnyKey : Effect
	{
		public override async Task Execute(EffectGroup g)
		{
			await Interface.WaitForInput(true);
		}
	}

	public class WaitForSeconds : Effect
	{
		public float delay;

		public WaitForSeconds(float seconds)
		{
			delay = seconds;
		}

		public override async Task Execute(EffectGroup g)
		{
			await Interface.Wait((int)(delay * 1000));
		}
	}

	public class Function : Effect
	{
		public Action action;

		public Function(Action action)
		{
			this.action = action;
		}

		public override Task Execute(EffectGroup g)
		{
			action.Invoke();
			return Task.CompletedTask;
		}
	}

	public class FunctionRef : Effect
	{
		public string functionId;
		public string[] arguments;

		public FunctionRef(string functionId, params string[] arguments)
		{
			this.functionId = functionId;
			this.arguments = arguments;
		}

		public override async Task Execute(EffectGroup g)
		{
			string output = await Functions.Execute(functionId, arguments);
			if(output != null) Interface.Write(output, true);
		}

		public override LogMessage Validate(string site)
		{
			if(!Functions.Exists(functionId)) return LogMessage.Error(site, "Nonexisting function referenced: " + functionId);
			return null;
		}
	}

	public class ModifyVariable : Effect
	{
		public string variableName;
		public int value;
		public bool additive;

		public ModifyVariable(string variableName, int value, bool additive)
		{
			this.variableName = variableName;
			this.value = value;
			this.additive = additive;
		}

		public override Task Execute(EffectGroup g)
		{
			var store = GameSession.Current.variables;
			if(additive)
			{
				store.Add(variableName, value);
			}
			else
			{
				store.Set(variableName, value);
			}
			return Task.CompletedTask;
		}

		public override LogMessage Validate(string site)
		{
			if(!Variables.IsDefined(variableName))
			{
				return LogMessage.Warning(site, $"Undefined variable '{variableName}'");
			}
			return base.Validate(site);
		}
	}

	public class SetColor : Effect
	{
		public ConsoleColor color;

		public SetColor(ConsoleColor color)
		{
			this.color = color;
		}

		public override Task Execute(EffectGroup g)
		{
			Interface.ForegroundColor = color;
			return Task.CompletedTask;
		}
	}

	public class SetBackgroundColor : Effect
	{
		public ConsoleColor color;

		public SetBackgroundColor(ConsoleColor color)
		{
			this.color = color;
		}

		public override Task Execute(EffectGroup g)
		{
			Interface.BackgroundColor = color;
			return Task.CompletedTask;
		}
	}

	public class ResetColor : Effect
	{
		public override Task Execute(EffectGroup g)
		{
			Interface.ResetColors();
			return Task.CompletedTask;
		}
	}

	public class ClearOutput : Effect
	{
		public override Task Execute(EffectGroup g)
		{
			Interface.Clear();
			return Task.CompletedTask;
		}
	}

	public class Todo : Effect
	{
		public string info;

		public Todo(string info)
		{
			this.info = info;
		}

		public override Task Execute(EffectGroup g)
		{
			Interface.Text("TODO: " + info, ConsoleColor.Magenta);
			return Task.CompletedTask;
		}

		public override LogMessage Validate(string site)
		{
			return LogMessage.Warning(site, "TODO marker found: " + info);
		}
	}

	public class Breakpoint : Effect
	{
		public override Task Execute(EffectGroup g)
		{
			Debugger.Break();
			return Task.CompletedTask;
		}

		public override LogMessage Validate(string site)
		{
			return LogMessage.Warning(site, "Breakpoint marker found");
		}
	}
}
