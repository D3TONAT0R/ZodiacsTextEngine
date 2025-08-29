using System;

namespace ZodiacsTextEngine
{
	public class Condition
	{
		public string variableName;
		public ConditionalOperator operation;
		//TODO: throw exception when comparing to a non-existing variable
		public Value compareValue;
		public bool Inverted { get; private set; }

		public VariableType VariableType => GetVariableTargetType(operation);

		//public Condition parentCondition;

		public bool? outcome = false;

		public Condition(string variableName, ConditionalOperator operation, Value compareValue)
		{
			this.variableName = variableName;
			this.operation = operation;
			this.compareValue = compareValue;
		}

		public Condition Invert()
		{
			Condition clone = new Condition(variableName, operation, compareValue)
			{
				//parentCondition = parentCondition
			};
			if(clone.Inverted) throw new System.InvalidOperationException("Condition is already inverted");
			clone.Inverted = true;
			return clone;
		}

		public bool Check()
		{
			//if(parentCondition != null && !parentCondition.Check()) return false;
			var type = GetVariableTargetType(operation);
			var vars = Session.Current.variables;
			bool result;
			if(type == VariableType.Int)
			{
				result = vars.CheckInt(variableName, operation, compareValue.GetInt());
			}
			else
			{
				result = vars.CheckString(variableName, compareValue.GetString(), operation);
			}
			return Inverted ? !result : result;
		}

		public override string ToString()
		{
			string op = Inverted ? $"!({variableName} {GetOperatorString(operation)} {compareValue})" : $"{variableName} {GetOperatorString(operation)} {compareValue}";
			//if(parentCondition != null) return parentCondition.ToString() + " && " + op;
			return op;
		}

		private static string GetOperatorString(ConditionalOperator op)
		{
			switch(op)
			{
				case ConditionalOperator.Equal: return "==";
				case ConditionalOperator.NotEqual: return "!=";
				case ConditionalOperator.GreaterThan: return ">";
				case ConditionalOperator.GreaterThanOrEqual: return ">=";
				case ConditionalOperator.LessThan: return "<";
				case ConditionalOperator.LessThanOrEqual: return "<=";
				case ConditionalOperator.StringEquals: return "IS";
				case ConditionalOperator.StringEqualsCaseSensitive: return "IS_CS";
				case ConditionalOperator.StringNotEquals: return "!IS";
				case ConditionalOperator.StringNotEqualsCaseSensitive: return "!IS_CS";
				case ConditionalOperator.StringContains: return "CONTAINS";
				case ConditionalOperator.StringContainsCaseSensitive: return "CONTAINS_CS";
				case ConditionalOperator.StringNotContains: return "!CONTAINS";
				case ConditionalOperator.StringNotContainsCaseSensitive: return "!CONTAINS_CS";
				case ConditionalOperator.StringStartsWith: return "STARTSWITH";
				case ConditionalOperator.StringStartsWithCaseSensitive: return "STARTSWITH_CS";
				case ConditionalOperator.StringNotStartsWith: return "!STARTSWITH";
				case ConditionalOperator.StringNotStartsWithCaseSensitive: return "!STARTSWITH_CS";
				case ConditionalOperator.StringEndsWith: return "ENDSWITH";
				case ConditionalOperator.StringEndsWithCaseSensitive: return "ENDSWITH_CS";
				case ConditionalOperator.StringNotEndsWith: return "!ENDSWITH";
				case ConditionalOperator.StringNotEndsWithCaseSensitive: return "!ENDSWITH_CS";
				default: return "???";
			}
		}

		private static VariableType GetVariableTargetType(ConditionalOperator op)
		{
			switch(op)
			{
				case ConditionalOperator.Equal:
				case ConditionalOperator.NotEqual:
				case ConditionalOperator.LessThan:
				case ConditionalOperator.LessThanOrEqual:
				case ConditionalOperator.GreaterThan:
				case ConditionalOperator.GreaterThanOrEqual:
					return VariableType.Int;
				case ConditionalOperator.StringEquals:
				case ConditionalOperator.StringEqualsCaseSensitive:
				case ConditionalOperator.StringNotEquals:
				case ConditionalOperator.StringNotEqualsCaseSensitive:
				case ConditionalOperator.StringContains:
				case ConditionalOperator.StringContainsCaseSensitive:
				case ConditionalOperator.StringNotContains:
				case ConditionalOperator.StringNotContainsCaseSensitive:
				case ConditionalOperator.StringStartsWith:
				case ConditionalOperator.StringStartsWithCaseSensitive:
				case ConditionalOperator.StringNotStartsWith:
				case ConditionalOperator.StringNotStartsWithCaseSensitive:
				case ConditionalOperator.StringEndsWith:
				case ConditionalOperator.StringEndsWithCaseSensitive:
				case ConditionalOperator.StringNotEndsWith:
				case ConditionalOperator.StringNotEndsWithCaseSensitive:
					return VariableType.String;
				default:
					throw new ArgumentException($"Unknown conditional operator: {op}");
			}
		}

		public LogMessage Validate(string site)
		{
			if(!Variables.IsDefined(variableName))
			{
				//Add variable to defined variables list to avoid error spam
				Variables.fixedVariableNames.Add(variableName);
				return LogMessage.Warning(site, $"Undefined variable in condition '{variableName}'");
			}
			return null;
			//return parentCondition?.Validate(site);
		}
	}
}
