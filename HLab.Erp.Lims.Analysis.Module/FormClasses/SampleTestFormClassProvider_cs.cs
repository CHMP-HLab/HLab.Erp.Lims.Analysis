﻿namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public partial class SampleTestFormClassProvider
    {
        private const string CsHeader = 
            "using System.Runtime;"
            + "using System.ComponentModel;"
            + "using System.Text;"
            + "using HLab.Erp.Lims.Analysis.Module.FormClasses;"
            + "using HLab.Notify.PropertyChanged;"
            + "using HLab.Notify.Annotations;" 
            + "using HLab.Erp.Conformity.Annotations;"
            + "using HLab.Notify.Wpf;" 
            + "using HLab.Base.Wpf;"
            + "using HLab.Erp.Lims.Analysis.Module.TestClasses;"
            + "using Outils;" 
            + "/*Content*/";

        private static string AddCsHeader(string cs)
        {
            cs = CsHeader.Replace("/*Content*/", cs);

            if (cs.Contains("using FM;"))
            {
                cs = cs.Replace("using FM;", "");
            }
            if (cs.Contains("void Traitement("))
            {
                cs = cs.Replace("void Traitement(", "void Process(");
            }

            return cs;
        }

        public SampleTestFormClassProvider(string xaml, string cs) : base(AddXamlHeader(xaml), AddCsHeader(cs))
        {
        }
    }
}
