using System;
using System.Collections.Generic;

namespace ZodiacsTextEngine
{
	public class Room
	{
		public readonly string name;

		public RoomEvent onEnter;
		public RoomEvent onExit;
		public List<Choice> choices = new List<Choice>();
		public Choice incorrectChoice;

		public Room(string id)
		{
			onEnter = null;
			name = id;
		}

		public Choice GetChoice(string input)
		{
			input = input.ToUpper().Trim();
			foreach(var c in choices)
			{
				if(input.Equals(c.prompt, StringComparison.OrdinalIgnoreCase)) return c;
			}
			if(incorrectChoice != null) return incorrectChoice;
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
	}
}
