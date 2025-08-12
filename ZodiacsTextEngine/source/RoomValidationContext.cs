using System.Collections.Generic;

namespace ZodiacsTextEngine
{
	public class RoomValidationContext
	{
		public Room room;
		public List<string> definedVars;
		public List<string> definedSVars;

		public RoomValidationContext(Room room, List<string> definedVars, List<string> definedSVars)
		{
			this.room = room;
			this.definedVars = definedVars;
			this.definedSVars = definedSVars;
		}
	}
}