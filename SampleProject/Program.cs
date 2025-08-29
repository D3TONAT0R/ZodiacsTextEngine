using ZodiacsTextEngine;

namespace SampleProject
{
	internal class Program
	{
		public class ContentContentLoader : StandardContentLoader
		{
			public ContentContentLoader() : base("content", "start")
			{
				// Set the root directory to "content" and the starting room to "start"
			}
		}

		private static async Task Main(string[] args)
		{
			await TextEngine.Initialize(new DefaultConsoleWindow(), new ContentContentLoader(), true);
			await TextEngine.Start();
		}
	}
}
