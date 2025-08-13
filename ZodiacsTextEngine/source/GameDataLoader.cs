using System;
using System.Threading.Tasks;
using static ZodiacsTextEngine.Functions;

namespace ZodiacsTextEngine
{
	public abstract class GameDataLoader
	{
		protected GameData gameData;

		public GameData Load(ref bool success)
		{
			gameData = new GameData();
			LoadContent(ref success);
			return gameData;
		}

		protected abstract void LoadContent(ref bool success);

		public virtual void AddRoom(Room room)
		{
			gameData.Rooms.Add(room.name, room);
		}

		public virtual void SetStartRoom(string roomName)
		{
			gameData.SetStartRoom(roomName);
		}

		public virtual void AddVariable(string name)
		{
			gameData.VariableNames.Add(name);
		}


		public virtual void AddFunction(string id, FunctionDelegate func)
		{
			if(id.Contains(" ")) throw new ArgumentException($"Function names cannot contain whitespaces.");
			foreach(var c in id)
			{
				if(!char.IsLetterOrDigit(c) && c != '_' && c != '-')
				{
					throw new ArgumentException($"Function names may only contain letters, digits, '-', and '_'.");
				}
			}
			if(gameData.Functions.ContainsKey(id)) throw new ArgumentException($"A function with name '{id}' already exists.");
			gameData.Functions.Add(id, func);
		}

		public void AddFunction(string id, Func<string> func)
		{
			AddFunction(id, _ => Task.FromResult(func.Invoke()));
		}

		public void AddFunction(string id, Action action)
		{
			AddFunction(id, _ =>
			{
				action.Invoke();
				return Task.FromResult<string>(null);
			});
		}
	}
}
