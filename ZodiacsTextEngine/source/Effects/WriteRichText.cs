using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public class WriteRichText : Effect, ITextEffect
	{
		public RichText text;

		public WriteRichText(RichText text)
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
}