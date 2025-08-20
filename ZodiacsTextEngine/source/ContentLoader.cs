using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ZodiacsTextEngine.Functions;

namespace ZodiacsTextEngine
{
	public abstract class ContentLoader
	{
		protected Story story;

		public Story Load(ref bool success)
		{
			story = new Story();
			Begin();
			foreach(var func in LoadFunctions())
			{
				AddFunction(func);
			}
			foreach(var room in LoadRooms())
			{
				AddRoom(room);
			}
			Complete();
			return story;
		}

		protected virtual void Begin()
		{

		}

		protected virtual IEnumerable<(string, FunctionDelegate)> LoadFunctions()
		{
			yield break;
		}

		protected abstract IEnumerable<Room> LoadRooms();

		protected virtual void Complete()
		{

		}

		public virtual void AddRoom(Room room)
		{
			story.Rooms.Add(room.name, room);
		}

		public virtual void SetStartRoom(string roomName)
		{
			story.SetStartRoom(roomName);
		}

		public virtual void AddVariable(string name)
		{
			story.VariableNames.Add(name);
		}

		public virtual void AddFunction((string, FunctionDelegate) function)
		{
			string id = function.Item1.ToLower();
			FunctionDelegate func = function.Item2;
			if(id.Contains(" ")) throw new ArgumentException($"Function names cannot contain whitespaces.");
			foreach(var c in id)
			{
				if(!char.IsLetterOrDigit(c) && c != '_' && c != '-')
				{
					throw new ArgumentException($"Function names may only contain letters, digits, '-', and '_'.");
				}
			}
			if(story.Functions.ContainsKey(id)) throw new ArgumentException($"A function with name '{id}' already exists.");
			story.Functions.Add(id, func);
		}
	}
}
