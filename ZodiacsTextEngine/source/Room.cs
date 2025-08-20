using System;
using System.Collections.Generic;
using System.IO;
using ZodiacsTextEngine.Effects;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine
{
	public class Room
	{
		public readonly string name;

		public EffectGroup onEnter;
		public EffectGroup onExit;
		public List<Choice> choices = new List<Choice>();
		public Choice antiChoice;

		public Room(string id)
		{
			onEnter = null;
			name = id;
		}

		public static Room Parse(Story story, string filename, string content)
		{
			return RoomParser.Parse(story, filename, content);
		}

		public Choice GetChoice(string input)
		{
			input = input.ToUpper().Trim();
			foreach(var c in choices)
			{
				if(input.Equals(c.prompt, StringComparison.OrdinalIgnoreCase)) return c;
			}
			if(antiChoice != null) return antiChoice;
			return null;
		}

		public IEnumerable<EffectGroup> EnumerateEffectGroups()
		{
			yield return onEnter;
			if(onExit != null) yield return onExit;
			foreach(var c in choices)
			{
				yield return c;
			}
		}

		public List<Effect> ListAllEffects()
		{
			var effects = new List<Effect>();
			onEnter?.ListAllEffects(effects);
			onExit?.ListAllEffects(effects);
			foreach(var choice in choices)
			{
				choice.ListAllEffects(effects);
			}
			return effects;
		}
	}
}
