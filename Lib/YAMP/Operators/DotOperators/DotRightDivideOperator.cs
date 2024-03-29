namespace YAMP
{
    /// <summary>
    /// The right divide operator ./
    /// </summary>
    internal class DotDivideOperator : DotOperator
	{
		public DotDivideOperator () : base(new RightDivideOperator())
		{
		}

		public override ScalarValue Operation (ScalarValue left, ScalarValue right)
		{
			return left / right;
		}

        public override Operator Create()
        {
            return new DotDivideOperator();
        }
	}
}

