using System;
using System.Collections.Generic;

namespace ZodiacsTextEngine
{
	public class EngineConfiguration
	{
		public readonly ITextInterface textInterface;
		public readonly Func<IEnumerable<string>> roomDataLoader;
		public readonly bool debug;
	}
}
