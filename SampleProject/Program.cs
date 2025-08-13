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
				AddFunction("prompt_player_name", async _ =>
				{
					var name = await TextEngine.RequestInput();
					GameSession.Current.variables.SetString("player_name", name);
					return null;
				});

				//Quick jump to room
				AddFunction("goto_prompt", async _ =>
				{
					TextEngine.Interface.Write("Go to room: ", true);
					var nextRoomName = await TextEngine.RequestInput();
					await GameSession.Current.GoToRoom(nextRoomName);
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
