using ZodiacsTextEngine;

namespace SampleProject
{
	internal class Program
	{
		public class MyContentLoader : StandardContentLoader
		{
			public MyContentLoader() : base("content", "start")
			{
				// Set the root directory to "content" and the starting room to "start"
			}
		}

		private static async Task Main(string[] args)
		{
			await TextEngine.Initialize(new DefaultConsoleWindow(), new MyContentLoader(), true);
			await TextEngine.Start();
		}
	}
}
