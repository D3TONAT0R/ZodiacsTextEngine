using System;

namespace ZodiacsTextEngine
{
	public class RoomFile
	{
		public readonly string roomName;
		public readonly string data;

		public RoomFile(string roomName, string data)
		{
			if(string.IsNullOrWhiteSpace(roomName)) throw new ArgumentException("Room name cannot be null or empty.");
			foreach(char c in roomName)
			{
				if(!char.IsLetterOrDigit(c) && c != '_')
				{
					throw new ArgumentException("Room name contains invalid characters (only letters, digits and underscores are allowed).");
				}
			}
			this.roomName = roomName;
			if(data == null) throw new ArgumentNullException("Room data cannot be null.");
			this.data = data;
		}
	}
}
