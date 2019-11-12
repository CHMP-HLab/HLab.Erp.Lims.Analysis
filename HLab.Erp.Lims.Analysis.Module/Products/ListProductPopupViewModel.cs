using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;
using System;
using System.Collections.Generic;
using System.Text;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ListProductPopupViewModel : EntityListViewModel<ListProductPopupViewModel, Product>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public string Title => "Product";
        private readonly IErpServices _erp;
        [Import]
        public ListProductPopupViewModel(IErpServices erp)
        {
            _erp = erp;
            Columns
                .Column("Ref", s => s.Caption)
                .Column("Inn", e => e.Inn)
                .Column("Dose", e => e.Dose)
                .Column("Form", e => e.Form)
                .Column("", async (s) => await _erp.Icon.GetIcon(s.Form?.IconPath ?? "", 25), s => s.Form.Name);
            using (List.Suspender.Get())
            {

            }

        }
    }
}
