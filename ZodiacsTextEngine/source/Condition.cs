using System.Collections.Generic;

namespace ZodiacsTextEngine
{
	public class Condition
	{
		public static List<Condition> AllConditions = new List<Condition>();

		public string variableName;
		public Variables.ConditionalOperator operation;
		public int value;
		public bool Inverted { get; private set; }

		//public Condition parentCondition;

		public bool? outcome = false;

		public Condition(string variableName, Variables.ConditionalOperator operation, int value)
		{
			this.variableName = variableName;
			this.operation = operation;
			this.value = value;
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
			Condition clone = new Condition(variableName, operation, value)
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
			bool result = GameSession.Current.variables.Check(variableName, operation, value);
			return Inverted ? !result : result;
		}

		public override string ToString()
		{
			string op = Inverted ? $"!({variableName} {GetOperatorString(operation)} {value})" : $"{variableName} {GetOperatorString(operation)} {value}";
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
				default: return "???";
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
