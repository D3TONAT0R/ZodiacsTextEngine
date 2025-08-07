using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public class WriteText : Effect, ITextEffect
	{
		public string[] lines;

		public WriteText(params string[] lines)
		{
			this.lines = lines;
		}

		public override Task Execute(EffectGroup g)
		{
			foreach(var line in lines)
			{
				TextEngine.Interface.Write(line, true);
			}
			return Task.CompletedTask;
		}

		public IEnumerable<string> GetTextStrings()
		{
			yield return string.Join("\n", lines);
		}
	}
}