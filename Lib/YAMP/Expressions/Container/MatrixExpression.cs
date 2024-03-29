﻿namespace YAMP
{
    using System;
    using System.Collections.Generic;
    using YAMP.Errors;

    /// <summary>
    /// The matrix [ ... ] expression.
    /// </summary>
    internal class MatrixExpression : TreeExpression
    {
        #region ctor

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MatrixExpression()
		{
		}

        /// <summary>
        /// Creates a new instance with some parameters.
        /// </summary>
        /// <param name="line">The line where the matrix expression starts.</param>
        /// <param name="column">The column in the line where the matrix exp. starts.</param>
        /// <param name="length">The length of the matrix expression.</param>
        /// <param name="query">The associated query context.</param>
        /// <param name="child">The child containing the column and rows.</param>
        public MatrixExpression(Int32 line, Int32 column, Int32 length, QueryContext query, ContainerExpression child)
            : base(child, query, line, column)
		{
            Length = length;
		}

        #endregion

        #region Methods

        /// <summary>
        /// Begins interpreting the matrix expression.
        /// </summary>
        /// <param name="symbols">External symbols to load.</param>
        /// <returns>The evaluated matrix value.</returns>
        public override Value Interpret(IDictionary<String, Value> symbols)
        {
            return base.Interpret(symbols) ?? new MatrixValue();
        }

        /// <summary>
        /// Scans the current parse engine for a matrix expression.
        /// </summary>
        /// <param name="engine">The parse engine to use.</param>
        /// <returns>The found expression or NULL.</returns>
        public override Expression Scan(ParseEngine engine)
        {
            var column = engine.CurrentColumn;
            var line = engine.CurrentLine;
            var chars = engine.Characters;
            var start = engine.Pointer;

            if (chars[start] == '[')
            {
                engine.Advance();
                var terminated = false;
                var statement = new Statement();
                var ws = false;
                var nl = false;

                while (engine.Pointer < chars.Length && engine.IsParsing)
                {
                    if (ParseEngine.IsWhiteSpace(chars[engine.Pointer]))
                    {
                        ws = true;
                        engine.Advance();
                    }
                    else if (ParseEngine.IsNewLine(chars[engine.Pointer]))
                    {
                        nl = true;
                        engine.Advance();
                    }
                    else if (chars[engine.Pointer] == ']')
                    {
                        terminated = true;
                        engine.Advance();
                        break;
                    }
                    else if (chars[engine.Pointer] == ',')
                    {
                        ws = false;
                        nl = false;
                        statement.Push(engine, new ColumnOperator(engine));
                        engine.Advance();
                    }
                    else if (chars[engine.Pointer] == ';')
                    {
                        ws = false;
                        nl = false;
                        statement.Push(engine, new RowOperator(engine));
                        engine.Advance();
                    }
                    else if (engine.Pointer < chars.Length - 1 && ParseEngine.IsComment(chars[engine.Pointer], chars[engine.Pointer + 1]))
                    {
                        if (ParseEngine.IsLineComment(chars[engine.Pointer], chars[engine.Pointer + 1]))
                            engine.AdvanceToNextLine();
                        else
                            engine.AdvanceTo("*/");
                    }
                    else
                    {
                        engine.ParseBlock(statement, nl ? (Operator)new RowOperator(engine) : (ws ? new ColumnOperator(engine) : null));
                        ws = false;
                        nl = false;
                    }
                }

                if (!terminated)
                {
                    var err = new YAMPMatrixNotClosedError(line, column);
                    engine.AddError(err);
                }

                var container = statement.Finalize(engine).Container;
                return new MatrixExpression(line, column, engine.Pointer - start, engine.Query, container ?? new ContainerExpression());
            }

            return null;
        }

        #endregion

        #region String Representations

        /// <summary>
        /// Transforms the expression into YAMP query code.
        /// </summary>
        /// <returns>The string representation of the part of the query.</returns>
        public override String ToCode()
        {
            return "[" + base.ToCode() + "]";
        }

        #endregion
    }
}
