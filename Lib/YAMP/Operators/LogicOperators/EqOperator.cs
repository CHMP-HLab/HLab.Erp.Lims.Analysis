namespace YAMP
{
    using System;

    /// <summary>
    /// The basic equals == operator.
    /// </summary>
    internal class EqOperator : LogicOperator
	{
        #region Mapping

        public static readonly String Symbol = OpDefinitions.EqOperator;
        public static readonly int OpLevel = OpDefinitions.EqOperatorLevel;
        public static readonly BinaryOperatorMappingList Mapping = new BinaryOperatorMappingList(Symbol);

        #endregion

        public EqOperator ()
            : base(Symbol, OpLevel)
		{
		}

		public override ScalarValue Compare (ScalarValue left, ScalarValue right)
		{
            return new ScalarValue(left == right);
		}

        public override Value Perform(Value left, Value right)
        {
            Value ret;
            if (!TryPerformOverFind(left, right, Mapping, out ret))
            {
                if (left is StringValue || right is StringValue)
                {
                    return new ScalarValue(left.ToString(Context) == right.ToString(Context));
                }

                return base.Perform(left, right);
            }
            return ret;
        }

        public override Operator Create()
        {
            return new EqOperator();
        }
	}
}

