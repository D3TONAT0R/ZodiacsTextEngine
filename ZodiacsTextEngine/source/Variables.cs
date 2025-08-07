using System.Collections.Generic;

namespace ZodiacsTextEngine
{
	public class Variables
	{
		public enum ConditionalOperator
		{
			Equal,
			NotEqual,

			//Integer specific operators
			LessThan,
			LessThanOrEqual,
			GreaterThanOrEqual,
			GreaterThan,

			//String specific operators
			StringContains,
			StringNotContains,
			StringStartsWith,
			StringNotStartsWith,
			StringEndsWith,
			StringNotEndsWith
		}

		public static bool HasFixedVariableNames => fixedVariableNames != null;

		public static List<string> fixedVariableNames = null;
		private readonly Dictionary<string, int> ints = new Dictionary<string, int>();
		private readonly Dictionary<string, string> strings = new Dictionary<string, string>();

		public static void Init(List<string> fixedVariableNames)
		{
			Variables.fixedVariableNames = fixedVariableNames;
		}

		public Variables()
		{

		}

		public void Clear()
		{
			ints.Clear();
			strings.Clear();
		}

		public int GetInt(string name)
		{
			if(ints.TryGetValue(name, out var value)) return value;
			return 0;
		}

		public string GetString(string name)
		{
			if(strings.TryGetValue(name, out var value)) return value;
			return "";
		}

		public bool HasInt(string name)
		{
			return ints.ContainsKey(name);
		}

		public bool HasString(string name)
		{
			return strings.ContainsKey(name);
		}

		public void SetInt(string name, int value)
		{
			ints[name] = value;
		}

		public void SetString(string name, string value)
		{
			strings[name] = value;
		}

		public void AddInt(string name, int value)
		{
			if(ints.ContainsKey(name)) ints[name] += value;
			else ints[name] = value;
		}

		public void AddString(string name, string value)
		{
			if(strings.ContainsKey(name)) strings[name] += value;
			else strings[name] = value;
		}

		public bool CheckInt(string varName, ConditionalOperator operation, int value)
		{
			var variableValue = GetInt(varName);
			switch(operation)
			{
				case ConditionalOperator.LessThan:
					return variableValue < value;
				case ConditionalOperator.LessThanOrEqual:
					return variableValue <= value;
				case ConditionalOperator.Equal:
					return variableValue == value;
				case ConditionalOperator.GreaterThanOrEqual:
					return variableValue >= value;
				case ConditionalOperator.GreaterThan:
					return variableValue > value;
				case ConditionalOperator.NotEqual:
					return variableValue != value;
				default:
					return false;
			}
		}

		public bool CheckString(string varName, string value, ConditionalOperator comparison, bool ignoreCase = true)
		{
			string varValue = GetString(varName) ?? "";
			value = value ?? "";
			var sc = ignoreCase ? System.StringComparison.OrdinalIgnoreCase : System.StringComparison.Ordinal;
			switch(comparison)
			{
				case ConditionalOperator.Equal:
					return varValue.Equals(value, sc);
				case ConditionalOperator.NotEqual:
					return !varValue.Equals(value, sc);
				case ConditionalOperator.StringContains:
					return varValue.IndexOf(value, sc) >= 0;
				case ConditionalOperator.StringNotContains:
					return varValue.IndexOf(value, sc) < 0;
				case ConditionalOperator.StringStartsWith:
					return varValue.StartsWith(value, sc);
				case ConditionalOperator.StringNotStartsWith:
					return !varValue.StartsWith(value, sc);
				case ConditionalOperator.StringEndsWith:
					return varValue.EndsWith(value, sc);
				case ConditionalOperator.StringNotEndsWith:
					return !varValue.EndsWith(value, sc);
				default:
					return false;
			}
		}

		public static bool IsDefined(string name)
		{
			if(fixedVariableNames != null)
			{
				return fixedVariableNames.Contains(name);
			}
			else return true;
		}
	}
}
