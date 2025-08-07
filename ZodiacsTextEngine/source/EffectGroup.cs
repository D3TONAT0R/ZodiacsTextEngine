using System.Collections.Generic;
using System.Threading.Tasks;
using ZodiacsTextEngine.Effects;

namespace ZodiacsTextEngine
{
	public class EffectGroup
	{
		public readonly string identifier;

		public List<Effect> effects = new List<Effect>();

		public EffectGroup(Room room, string id)
		{
			identifier = room.name + "->" + id.Replace(' ', '_');
		}

		public Dictionary<Condition, bool> conditionStates = new Dictionary<Condition, bool>();

		public async Task Execute()
		{
			conditionStates.Clear();
			foreach(var e in effects)
			{
				await e.Execute(this);
			}
		}

		public bool EvaluateCondition(Condition c)
		{
			if(conditionStates.TryGetValue(c, out bool value))
			{
				return value;
			}
			else
			{
				var state = c.Check();
				conditionStates.Add(c, state);
				return state;
			}
		}
	}

	public class Choice : EffectGroup
	{
		public string prompt;

		public Choice(Room room, string prompt) : base(room, prompt.ToLower())
		{
			this.prompt = prompt.ToLower();
		}

		public Choice(Room room, string prompt, params Effect[] effects) : this(room, prompt)
		{
			this.effects.AddRange(effects);
		}
	}
}
