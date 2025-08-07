using System.Collections.Generic;

namespace ZodiacsTextEngine
{
	public class Condition
	{
		public static List<Condition> AllConditions = new List<Condition>();

		public string variableName;
		public Variables.ConditionalOperator operation;
		public string compareValue;
		public bool ignoreCase = true;
		public bool Inverted { get; private set; }

		//public Condition parentCondition;

		public bool? outcome = false;

		public Condition(string variableName, Variables.ConditionalOperator operation, string compareValue)
		{
			this.variableName = variableName;
			this.operation = operation;
			this.compareValue = compareValue;
		}

		public static void CheckAll()
		{
			foreach(var c in AllConditions)
			{
				c.outcome = c.Check();
			}
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
			var vars = GameSession.Current.variables;
			bool result;
			if(type == null)
			{
				//Target can be either int or string, check string first
				result = vars.HasString(variableName)
					? vars.CheckString(variableName, compareValue, operation, ignoreCase)
					: vars.CheckInt(variableName, operation, int.Parse(compareValue));
			}
			else if(type == typeof(int))
			{
				result = vars.CheckInt(variableName, operation, int.Parse(compareValue));
			}
			else
			{
				result = vars.CheckString(variableName, compareValue, operation, ignoreCase);
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
				case Variables.ConditionalOperator.StringContains: return "**";
				case Variables.ConditionalOperator.StringNotContains: return "!*";
				case Variables.ConditionalOperator.StringStartsWith: return ".*";
				case Variables.ConditionalOperator.StringNotStartsWith: return "!.*";
				case Variables.ConditionalOperator.StringEndsWith: return "*.";
				case Variables.ConditionalOperator.StringNotEndsWith: return "!*.";
				default: return "???";
			}
		}

		private static System.Type GetVariableTargetType(Variables.ConditionalOperator op)
		{
			if(op == Variables.ConditionalOperator.Equal || op == Variables.ConditionalOperator.NotEqual) return null;
			if(op == Variables.ConditionalOperator.LessThan || op == Variables.ConditionalOperator.LessThanOrEqual ||
			   op == Variables.ConditionalOperator.GreaterThan || op == Variables.ConditionalOperator.GreaterThanOrEqual)
			{
				return typeof(int);
			}
			else
			{
				return typeof(string);
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
