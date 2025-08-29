namespace ZodiacsTextEngine
{
	public enum ConditionalOperator
	{
		//Integer specific operators
		Equal,
		NotEqual,
		LessThan,
		LessThanOrEqual,
		GreaterThanOrEqual,
		GreaterThan,

		//String specific operators
		StringEquals,
		StringEqualsCaseSensitive,
		StringNotEquals,
		StringNotEqualsCaseSensitive,
		StringContains,
		StringContainsCaseSensitive,
		StringNotContains,
		StringNotContainsCaseSensitive,
		StringStartsWith,
		StringStartsWithCaseSensitive,
		StringNotStartsWith,
		StringNotStartsWithCaseSensitive,
		StringEndsWith,
		StringEndsWithCaseSensitive,
		StringNotEndsWith,
		StringNotEndsWithCaseSensitive
	}
}