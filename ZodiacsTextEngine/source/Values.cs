using System.Linq;

namespace ZodiacsTextEngine
{
	public abstract class Value
	{
		public abstract int GetInt();

		public abstract string GetString();

		public static Value Parse(string value, bool allowStrings)
		{
			if(value.StartsWith("\""))
			{
				if(!allowStrings)
				{
					throw new System.ArgumentException("String values are not allowed in this context");
				}
				if(!value.EndsWith("\""))
				{
					throw new System.ArgumentException("Invalid string value, missing closing quote");
				}
				//Get string between first and last quote
				var str = value.Substring(1, value.Length - 2);
				return new StringConstant(str);
			}
			if(value.Length > 1 && value[0] == '-' && (char.IsLetter(value[1]) || value[1] == '_'))
			{
				//Negative variable reference
				return new VariableRef(value.Substring(1), false);
			}
			if(char.IsLetter(value[0]) || value[0] == '_')
			{
				//Parse variable reference
				return new VariableRef(value, true);
			}
			//Just return as int
			return new Constant(int.Parse(value));
		}
	}

	public class Constant : Value
	{
		public readonly int value;

		public Constant(int value)
		{
			this.value = value;
		}

		public override int GetInt() => value;

		public override string GetString() => value.ToString();

		public override string ToString()
		{
			return value.ToString();
		}
	}

	public class StringConstant : Value
	{
		public readonly string value;
		public StringConstant(string value)
		{
			this.value = value;
		}
		public override int GetInt() => 0; // Strings don't have an integer representation
		public override string GetString() => value;
		public override string ToString()
		{
			return $"\"{value}\"";
		}
	}

	public class VariableRef : Value
	{
		public readonly string variableName;
		public readonly bool sign;

		public VariableRef(string variableName, bool sign)
		{
			this.variableName = variableName;
			this.sign = sign;
		}

		public override int GetInt()
		{
			int v = Session.Current.variables.GetInt(variableName);
			if(!sign) v = -v;
			return v;
		}

		public override string GetString()
		{
			string v = Session.Current.variables.GetString(variableName);
			return v;
		}

		public override string ToString()
		{
			var vars = Session.Current?.variables;
			if(vars == null) return $"'{variableName}'";
			var v = vars.HasString(variableName) ? vars.GetString(variableName) : vars.GetInt(variableName).ToString();
			return $"'{variableName}' ({v})";
		}
	}
}