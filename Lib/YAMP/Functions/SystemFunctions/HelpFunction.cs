﻿namespace YAMP
{
    using System;
    using System.Text;
    using YAMP.Help;

    [Description("HelpFunctionDescription")]
	[Kind(PopularKinds.System)]
    internal sealed class HelpFunction : SystemFunction
    {
        const String SPACING = "   ";

        public HelpFunction(ParseContext context)
            : base(context)
        {
        }

        [Description("HelpFunctionDescriptionForVoid")]
        [Example("help()", "HelpFunctionExampleForVoid1")]
        public StringValue Function()
        {
            var sb = new StringBuilder();
			var list = Documentation.Overview(Context);

			foreach (var entry in list)
			{
				sb.Append(" ").AppendLine(entry.Kind).AppendLine("--------------");

                foreach (var item in entry)
                {
                    sb.Append("--> ").AppendLine(item.Name);
                }

				sb.AppendLine();
			}

            return new StringValue(sb.ToString());
        }

        [Description("HelpFunctionDescriptionForString")]
        [Example("help(\"help\")", "HelpFunctionExampleForString1")]
        [Example("help(\"sin\")", "HelpFunctionExampleForString2")]
        public StringValue Function(StringValue topic)
        {
			var docu = Documentation.Create(Context);

            if (docu.ContainsEntry(topic.Value))
            {
                var entry = docu.Get(topic.Value);
                var sb = new StringBuilder();

                sb.Append(" ").AppendLine(entry.Name).AppendLine("--------------");

                sb.AppendLine().AppendLine("Description:").Append("\t").AppendLine(entry.Description);

                if (entry is HelpFunctionSection)
                {
                    var fe = entry as HelpFunctionSection;

                    foreach (var usage in fe.Usages)
                    {
                        var i = 1;
                        sb.AppendLine();
                        sb.AppendLine("** Usage **").Append("\t").AppendLine(usage.Usage);
                        sb.AppendLine("** Description **").Append("\t").AppendLine(usage.Description);
                        sb.AppendLine("** Arguments **").Append("\t").AppendLine(string.Join(", ", usage.Arguments.ToArray()));
                        sb.AppendLine("** Returns **");

                        foreach (var ret in usage.Returns)
                        {
                            sb.Append("\t").AppendLine(ret);
                        }

                        foreach (var example in usage.Examples)
                        {
                            sb.AppendFormat(" ({0}) Example:", i).AppendLine();
                            sb.Append("\t").Append("-> Call: ").AppendLine(example.Example);
                            sb.Append("\t").Append("-> Description: ").AppendLine(example.Description);
                            i++;
                        }
                    }
                }

                return new StringValue(sb.ToString());
            }
            else
            {
                return new StringValue(String.Format("The specified entry was not found. Did you mean {0}?", docu.ClosestEntry(topic.Value)));
            }
        }
    }
}
