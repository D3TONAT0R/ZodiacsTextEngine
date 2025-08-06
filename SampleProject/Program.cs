using ZodiacsTextEngine;

namespace SampleProject
{
	internal class Program
	{
		public class ContentLoader : StandardGameDataLoader
		{
			public ContentLoader() : base("content", "start")
			{
				// Set the root directory to "content" and the starting room to "start"
			}

			protected override void LoadContent(ref bool success)
			{
				base.LoadContent(ref success);

				//Request player name
				AddFunction("get_player_name", async _ =>
				{
					GameSession.Current.playerName = await TextEngine.Interface.ReadInput();
					return null;
				});
			}
		}

		static async Task Main(string[] args)
		{
			await TextEngine.Initialize(new DefaultConsoleWindow(), new ContentLoader(), true);
			await TextEngine.StartGame();
		}
	}
}
