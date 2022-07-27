namespace YAMP
{
    /// <summary>
    /// Gets the value for true.
    /// </summary>
    [Description("TrueConstantDescription")]
    [Kind(PopularKinds.Constant)]
    internal sealed class TrueConstant : BaseConstant
    {
        public override Value Value
        {
            get { return ScalarValue.True; }
        }
    }
}
