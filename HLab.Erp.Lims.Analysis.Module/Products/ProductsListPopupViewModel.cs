using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;
using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductsListPopupViewModel : EntityListViewModel<ProductsListPopupViewModel, Product>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public override string Title => "{Products}";
        [Import]
        public ProductsListPopupViewModel()
        {
            Columns
                .Column("{Ref}", s => s.Caption)
                .Column("{Inn}", e => e.Inn)
                .Column("{Dose}", e => e.Dose)
                .Column("{Form}", e => e.Form)
                .Icon("", s => s.Form?.IconPath ?? "",  s => s.Form.Name);
            using (List.Suspender.Get())
            {
                Filters.Add(new FilterTextViewModel{Title = "{Inn}"}.Link(List,e => e.Inn));
                Filters.Add(new FilterTextViewModel{Title = "{Dose}"}.Link(List,e => e.Dose));

            }

        }
    }
}
