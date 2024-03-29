namespace YAMP
{
    /// <summary>
    /// The .\ operator.
    /// </summary>
    internal class DotLeftDivideOperator : DotOperator
    {
        public DotLeftDivideOperator() : base(new LeftDivideOperator())
        {
        }

        public override ScalarValue Operation(ScalarValue left, ScalarValue right)
        {
            return right / left;
        }

        public override Operator Create()
        {
            return new DotLeftDivideOperator();
        }
    }
}

