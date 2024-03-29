﻿namespace YAMP
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using YAMP.Errors;

    /// <summary>
    /// Represents the syntax for a function keyword. The basic syntax is
    /// function NAME ( ARGS ) STATEMENT
    /// </summary>
    internal class FunctionKeyword : BodyKeyword
    {
        #region Fields

        BracketExpression arguments;
        SymbolExpression name;

        #endregion

        #region ctor

        public FunctionKeyword()
            : base("function")
        {
        }

        public FunctionKeyword(Int32 line, Int32 column, QueryContext query)
            : this()
        {
            Query = query;
            StartLine = line;
            StartColumn = column;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the created function.
        /// </summary>
        public String Name
        {
            get { return name.SymbolName; }
        }

        /// <summary>
        /// Gets the name of the arguments of the function.
        /// </summary>
        public String[] Arguments
        {
            get
            {
                var symbols = arguments.GetSymbols().ToArray();
                var args = new String[symbols.Length];

                for (var i = 0; i != symbols.Length; i++)
                {
                    args[i] = symbols[i].SymbolName;
                }

                return args;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Scans for a function entry.
        /// </summary>
        /// <param name="engine">The current parse engine.</param>
        /// <returns>The created expression.</returns>
        public override Expression Scan(ParseEngine engine)
        {
            var start = engine.Pointer;
            var kw = new FunctionKeyword(engine.CurrentLine, engine.CurrentColumn, engine.Query);
            engine.Advance(Token.Length).Skip();

            if (engine.Pointer == engine.Characters.Length)
            {
                kw.Length = engine.Pointer - start;
                engine.AddError(new YAMPFunctionNameMissing(engine), kw);
                return kw;
            }

            kw.name = engine.Elements.FindExpression<SymbolExpression>().Scan(engine) as SymbolExpression;

            if (kw.name == null)
            {
                engine.AddError(new YAMPFunctionNameMissing(engine), kw);
                return kw;
            }

            engine.Skip();

            if (engine.Pointer == engine.Characters.Length)
            {
                kw.Length = engine.Pointer - start;
                engine.AddError(new YAMPFunctionArgumentsMissing(engine), kw);
                return kw;
            }

            kw.arguments = engine.Elements.FindExpression<BracketExpression>().Scan(engine) as BracketExpression;

            if (engine.Pointer == engine.Characters.Length)
            {
                kw.Length = engine.Pointer - start;
                engine.AddError(new YAMPFunctionBodyMissing(engine), kw.arguments);
                return kw;
            }

            kw.Body = engine.ParseStatement();
            kw.Length = engine.Pointer - start;

            if (kw.Body.Container.Expressions.Length == 1 && kw.Body.Container.Expressions[0] is GroupExpression)
            {
                var container = (GroupExpression)kw.Body.Container.Expressions[0];
                var context = new ParseContext(engine.Context.Parent);
                var input = container.Scope.Input;
                container.Scope = new QueryContext(context, input);
            }
            else
            {
                engine.AddError(new YAMPFunctionBodyMissing(engine), kw.arguments);
                return kw;
            }
            
            if (kw.arguments == null)
            {
                engine.AddError(new YAMPFunctionArgumentsMissing(engine), kw);
                return kw;
            }
            else if (kw.arguments.HasContent && !kw.arguments.IsSymbolList)
            {
                engine.AddError(new YAMPFunctionArgumentsSymbols(engine), kw.arguments);
                return kw;
            }

            return kw;
        }

        public override Value Interpret(IDictionary<String, Value> symbols)
        {
            var f = new FunctionValue(Name, Arguments, Body.Container);
            Query.Context.AddFunction(Name, f);
            return f;
        }

        #endregion

        #region String Representations

        /// <summary>
        /// Transforms the created function expression to YAMP code.
        /// </summary>
        /// <returns>The string for creating the expression.</returns>
        public override String ToCode()
        {
            var sb = new StringBuilder();
            sb.Append(Token).Append(" ").Append(Name);
            sb.AppendLine(arguments.ToCode());
            sb.AppendLine(Body.ToCode());
            return sb.ToString();
        }

        #endregion
    }
}