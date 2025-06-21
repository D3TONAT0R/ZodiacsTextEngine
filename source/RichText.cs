using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public class RichTextComponent
	{
		public string text;
		public ConsoleColor? foregroundColor;
		public ConsoleColor? backgroundColor;

		public RichTextComponent(string text, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
		{
			this.text = text;
			this.foregroundColor = foregroundColor;
			this.backgroundColor = backgroundColor;
		}

		public virtual Task Write(ConsoleColor baseForegroundColor, ConsoleColor baseBackgroundColor)
		{
			TextEngine.Interface.ForegroundColor = foregroundColor ?? baseForegroundColor;
			TextEngine.Interface.BackgroundColor = backgroundColor ?? baseBackgroundColor;
			TextEngine.Interface.Write(text, false);
			return Task.CompletedTask;
		}
	}

	public class PlayerNameComponent : RichTextComponent
	{
		public PlayerNameComponent() : base(null)
		{

		}

		public override Task Write(ConsoleColor baseForegroundColor, ConsoleColor baseBackgroundColor)
		{
			TextEngine.Interface.Write(GameSession.Current.playerName, false);
			return Task.CompletedTask;
		}
	}

	public class VariableComponent : RichTextComponent
	{
		public string variableName;
		public float multiplier = 1;
		public string format;

		public VariableComponent(string variableName, float multiplier = 1, string format = null) : base(null)
		{
			this.variableName = variableName;
			this.multiplier = multiplier;
			this.format = format;
		}

		public override Task Write(ConsoleColor baseForegroundColor, ConsoleColor baseBackgroundColor)
		{
			//Get the value of the variable and multiply it by the multiplier, with up to 2 decimal places
			float variable = GameSession.Current.variables.Get(variableName);
			//Get value as a string with the given format
			string value = (variable * multiplier).ToString(format ?? "0.##");
			TextEngine.Interface.Write(value, false);
			return Task.CompletedTask;
		}
	}

	public class FunctionComponent : RichTextComponent
	{
		public string functionName;
		public string[] arguments;

		public FunctionComponent(string functionName, string[] arguments) : base(null)
		{
			this.functionName = functionName;
			this.arguments = arguments;
		}

		public override async Task Write(ConsoleColor baseForegroundColor, ConsoleColor baseBackgroundColor)
		{
			string output = await Functions.Execute(functionName, arguments);
			if(output != null) TextEngine.Interface.Write(output, false);
		}
	}

	public class RichText
	{
		public List<RichTextComponent> components;

		public RichText(params RichTextComponent[] components)
		{
			this.components = new List<RichTextComponent>(components);
		}

		public void AddComponent(RichTextComponent component)
		{
			components.Add(component);
		}

		public void Write(bool lineBreak = true)
		{
			var baseForegroundColor = TextEngine.Interface.ForegroundColor;
			var baseBackgroundColor = TextEngine.Interface.BackgroundColor;
			foreach(var component in components)
			{
				component.Write(baseForegroundColor, baseBackgroundColor);
			}
			TextEngine.Interface.ForegroundColor = baseForegroundColor;
			TextEngine.Interface.BackgroundColor = baseBackgroundColor;
			if(lineBreak) TextEngine.Interface.LineBreak();
		}
	}
}
