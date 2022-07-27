namespace YAMP
{
	[Description("TypeFunctionDescription")]
	[Kind(PopularKinds.System)]
    internal sealed class TypeFunction : StandardFunction
    {
        [Description("TypeFunctionDescriptionForValue")]
        [Example("type(x)", "TypeFunctionExampleForValue1")]
        public override Value Perform(Value argument)
        {
            return new StringValue(argument.Header);
        }
    }
}
