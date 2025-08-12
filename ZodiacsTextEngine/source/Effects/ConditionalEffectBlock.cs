using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	//TODO: word counter breaks
	public class ConditionalEffectBlock : Effect, IEffectParent
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

		public IEnumerable<Effect> GetChildEffects()
		{
			foreach(var e in childEffects)
			{
				yield return e;
			}
			if(invertedChildEffects != null)
			{
				foreach(var e in invertedChildEffects)
				{
					yield return e;
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
}