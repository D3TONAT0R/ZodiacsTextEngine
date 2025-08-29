using System;

namespace ZodiacsTextEngine
{
	public class Condition
	{
		public string variableName;
		public Variables.ConditionalOperator operation;
		//TODO: throw exception when comparing to a non-existing variable
		public Value compareValue;
		public bool Inverted { get; private set; }

		public VariableType VariableType => GetVariableTargetType(operation);

		//public Condition parentCondition;

		public bool? outcome = false;

		public Condition(string variableName, Variables.ConditionalOperator operation, Value compareValue)
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

		private static string GetOperatorString(Variables.ConditionalOperator op)
		{
			switch(op)
			{
				case Variables.ConditionalOperator.Equal: return "==";
				case Variables.ConditionalOperator.NotEqual: return "!=";
				case Variables.ConditionalOperator.GreaterThan: return ">";
				case Variables.ConditionalOperator.GreaterThanOrEqual: return ">=";
				case Variables.ConditionalOperator.LessThan: return "<";
				case Variables.ConditionalOperator.LessThanOrEqual: return "<=";
				case Variables.ConditionalOperator.StringEquals: return "IS";
				case Variables.ConditionalOperator.StringEqualsCaseSensitive: return "IS_CS";
				case Variables.ConditionalOperator.StringNotEquals: return "!IS";
				case Variables.ConditionalOperator.StringNotEqualsCaseSensitive: return "!IS_CS";
				case Variables.ConditionalOperator.StringContains: return "CONTAINS";
				case Variables.ConditionalOperator.StringContainsCaseSensitive: return "CONTAINS_CS";
				case Variables.ConditionalOperator.StringNotContains: return "!CONTAINS";
				case Variables.ConditionalOperator.StringNotContainsCaseSensitive: return "!CONTAINS_CS";
				case Variables.ConditionalOperator.StringStartsWith: return "STARTSWITH";
				case Variables.ConditionalOperator.StringStartsWithCaseSensitive: return "STARTSWITH_CS";
				case Variables.ConditionalOperator.StringNotStartsWith: return "!STARTSWITH";
				case Variables.ConditionalOperator.StringNotStartsWithCaseSensitive: return "!STARTSWITH_CS";
				case Variables.ConditionalOperator.StringEndsWith: return "ENDSWITH";
				case Variables.ConditionalOperator.StringEndsWithCaseSensitive: return "ENDSWITH_CS";
				case Variables.ConditionalOperator.StringNotEndsWith: return "!ENDSWITH";
				case Variables.ConditionalOperator.StringNotEndsWithCaseSensitive: return "!ENDSWITH_CS";
				default: return "???";
			}
		}

		private static VariableType GetVariableTargetType(Variables.ConditionalOperator op)
		{
			switch(op)
			{
				case Variables.ConditionalOperator.Equal:
				case Variables.ConditionalOperator.NotEqual:
				case Variables.ConditionalOperator.LessThan:
				case Variables.ConditionalOperator.LessThanOrEqual:
				case Variables.ConditionalOperator.GreaterThan:
				case Variables.ConditionalOperator.GreaterThanOrEqual:
					return VariableType.Int;
				case Variables.ConditionalOperator.StringEquals:
				case Variables.ConditionalOperator.StringEqualsCaseSensitive:
				case Variables.ConditionalOperator.StringNotEquals:
				case Variables.ConditionalOperator.StringNotEqualsCaseSensitive:
				case Variables.ConditionalOperator.StringContains:
				case Variables.ConditionalOperator.StringContainsCaseSensitive:
				case Variables.ConditionalOperator.StringNotContains:
				case Variables.ConditionalOperator.StringNotContainsCaseSensitive:
				case Variables.ConditionalOperator.StringStartsWith:
				case Variables.ConditionalOperator.StringStartsWithCaseSensitive:
				case Variables.ConditionalOperator.StringNotStartsWith:
				case Variables.ConditionalOperator.StringNotStartsWithCaseSensitive:
				case Variables.ConditionalOperator.StringEndsWith:
				case Variables.ConditionalOperator.StringEndsWithCaseSensitive:
				case Variables.ConditionalOperator.StringNotEndsWith:
				case Variables.ConditionalOperator.StringNotEndsWithCaseSensitive:
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
