namespace YAMP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using YAMP.Exceptions;

    /// <summary>
    /// This class represents the basis of the assignment operators
    /// as well as the simple assignment operator (=).
    /// </summary>
    internal class AssignmentOperator : BinaryOperator
    {
        #region ctor

        public AssignmentOperator () : 
            this(String.Empty)
		{
		}

		public AssignmentOperator(String prefix) : 
            base(prefix + "=", -1)
		{
		}

        #endregion

        #region Methods

        public override Operator Create()
		{
			return new AssignmentOperator();
		}

		public override Value Handle(Expression left, Expression right, IDictionary<String, Value> symbols)
		{
			var bottom = right.Interpret(symbols);
			return Assign(left, bottom, symbols);
		}
		
		public override Value Perform (Value left, Value right)
		{
			return right;
		}

		protected Value Assign(Expression left, Value value, IDictionary<String, Value> symbols)
		{
            if (left is SymbolExpression)
            {
                return Assign((SymbolExpression)left, value, symbols);
            }
            else if (left is ContainerExpression)
            {
                var tree = (ContainerExpression)left;

                if (tree.Operator == null)
                {
                    return Assign(tree.Expressions[0], value, symbols);
                }
                else if (tree.Operator is ArgsOperator)
                {
                    var ix = (ArgsOperator)tree.Operator;
                    return ix.Handle(tree.Expressions[0], value, symbols);
                }
                else if (tree.Operator is MemberOperator)
                {
                    var mo = (MemberOperator)tree.Operator;
                    return mo.Handle(tree.Expressions[0], tree.Expressions[1], value, symbols);
                }
                else if (tree.IsSymbolList)
                {
                    var vars = tree.GetSymbols().ToArray();
                    return HandleMultipleOutputs(value, vars, symbols);
                }
                else
                {
                    throw new YAMPAssignmentException(Op);
                }
            }
			
			return value;
		}

        #endregion

        #region Helpers

        Value HandleMultipleOutputs(Value value, SymbolExpression[] vars, IDictionary<String, Value> symbols)
        {
            if (value is ArgumentsValue)
            {
                var av = (ArgumentsValue)value;
                var l = Math.Min(vars.Length, av.Length);

                for (var i = 0; i != l; i++)
                {
                    Assign(vars[i], av.Values[i], symbols);
                }

                return av;
            }

            foreach (var sym in vars)
            {
                Assign(sym, value, symbols);
            }

            return value;
        }

        Value Assign(SymbolExpression left, Value value, IDictionary<String, Value> symbols)
		{
            if (symbols.ContainsKey(left.SymbolName))
            {
                symbols[left.SymbolName] = value.Copy();
            }
            else
            {
                Context.AssignVariable(left.SymbolName, value.Copy());
            }

			return value;
        }

        #endregion
    }
}

