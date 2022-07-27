namespace YAMP
{
    using System;

    internal abstract class RightUnaryOperator : UnaryOperator
    {
        public RightUnaryOperator(String op, Int32 level) : 
            base(op, level)
        {
            IsRightToLeft = true;
        }
    }
}
