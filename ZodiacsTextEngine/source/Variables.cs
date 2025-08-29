using System;
using System.Collections.Generic;

namespace ZodiacsTextEngine
{
	public class Variables
	{
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
					throw new ArgumentException("Unsupported operator for integers " + operation);
			}
		}

		public bool CheckString(string varName, string value, ConditionalOperator operation)
		{
			string varValue = GetString(varName) ?? "";
			value = value ?? "";
			switch(operation)
			{
				case ConditionalOperator.StringEquals:
					return varValue.Equals(value, StringComparison.OrdinalIgnoreCase);
				case ConditionalOperator.StringEqualsCaseSensitive:
					return varValue.Equals(value, StringComparison.Ordinal);
				case ConditionalOperator.StringNotEquals:
					return !varValue.Equals(value, StringComparison.OrdinalIgnoreCase);
				case ConditionalOperator.StringNotEqualsCaseSensitive:
					return !varValue.Equals(value, StringComparison.Ordinal);
				case ConditionalOperator.StringContains:
					return varValue.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
				case ConditionalOperator.StringContainsCaseSensitive:
					return varValue.IndexOf(value, StringComparison.Ordinal) >= 0;
				case ConditionalOperator.StringNotContains:
					return varValue.IndexOf(value, StringComparison.OrdinalIgnoreCase) < 0;
				case ConditionalOperator.StringNotContainsCaseSensitive:
					return varValue.IndexOf(value, StringComparison.Ordinal) < 0;
				case ConditionalOperator.StringStartsWith:
					return varValue.StartsWith(value, StringComparison.OrdinalIgnoreCase);
				case ConditionalOperator.StringStartsWithCaseSensitive:
					return varValue.StartsWith(value, StringComparison.Ordinal);
				case ConditionalOperator.StringNotStartsWith:
					return !varValue.StartsWith(value, StringComparison.OrdinalIgnoreCase);
				case ConditionalOperator.StringNotStartsWithCaseSensitive:
					return !varValue.StartsWith(value, StringComparison.Ordinal);
				case ConditionalOperator.StringEndsWith:
					return varValue.EndsWith(value, StringComparison.OrdinalIgnoreCase);
				case ConditionalOperator.StringEndsWithCaseSensitive:
					return varValue.EndsWith(value, StringComparison.Ordinal);
				case ConditionalOperator.StringNotEndsWith:
					return !varValue.EndsWith(value, StringComparison.OrdinalIgnoreCase);
				case ConditionalOperator.StringNotEndsWithCaseSensitive:
					return !varValue.EndsWith(value, StringComparison.Ordinal);
				default:
					throw new ArgumentException("Unsupported operator for strings " + operation);
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
