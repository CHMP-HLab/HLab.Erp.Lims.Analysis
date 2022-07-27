namespace YAMP
{
    /// <summary>
    /// This class represents the += operator.
    /// </summary>
    internal class PlusAssignmentOperator : AssignmentPrefixOperator
    {
        public PlusAssignmentOperator() : 
            base(new PlusOperator())
        {
        }

        public static PlusAssignmentOperator CreateWithContext(QueryContext context)
        {
            return new PlusAssignmentOperator
            {
                Query = context
            };
        }

        public override Operator Create()
        {
            return new PlusAssignmentOperator();
        }
    }
}
