namespace YAMP
{
    [Description("VerFunctionDescription")]
    [Kind(PopularKinds.System)]
    internal sealed class VerFunction : SystemFunction
    {
        public VerFunction(ParseContext context)
            : base(context)
        {
        }

        [Description("VerFunctionDescriptionForVoid")]
        [Example("ver()", "VerFunctionExampleForVoid1")]
        public StringValue Function()
        {
            return new StringValue(Parser.Version);
        }
    }
}
