using System;
using System.Collections.Generic;
using System.Text;
using static ZodiacsTextEngine.Functions;

namespace ZodiacsTextEngine.source
{
	public abstract class GameDataProvider
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
			if(id.Contains(" ")) throw new ArgumentException($"Function ids cannot contain whitespaces");
			if(gameData.Functions.ContainsKey(id)) throw new ArgumentException($"A function with id '{id}' already exists.");
			gameData.Functions.Add(id, func);
		}
	}
}
