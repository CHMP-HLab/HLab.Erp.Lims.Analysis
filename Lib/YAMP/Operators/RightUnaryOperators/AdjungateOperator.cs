namespace YAMP
{
    using System;
    using YAMP.Exceptions;

    /// <summary>
    /// This is the operator for adjungating a matrix.
    /// </summary>
    internal class AdjungateOperator : RightUnaryOperator
	{
        public static readonly String Symbol = OpDefinitions.AdjungateOperator;
        public static readonly int OpLevel = OpDefinitions.AdjungateOperatorLevel;

        public AdjungateOperator ()
            : base(Symbol, OpLevel)
		{
		}
		
		public override Value Perform (Value left)
		{
            if (left is ScalarValue)
            {
                return (left as ScalarValue).Conjugate();
            }
            else if (left is MatrixValue)
            {
                return (left as MatrixValue).Adjungate();
            }
			
			throw new YAMPOperationInvalidException("'", left);
		}

        public override Operator Create()
        {
            return new AdjungateOperator();
        }
	}
}

