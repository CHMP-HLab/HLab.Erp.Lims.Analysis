using System;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ListProductViewModel: EntityListViewModel<ListProductViewModel,Product>, IMvvmContextProvider
    {
        public string Title => "Products";

        public ListProductViewModel() 
        {
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                //.Column("Ref",  s => s.Caption)
                .Column("Inn",e => e.Inn)
                .Column("Dose",e => e.Dose)
                .Column("Form",e => e.Form)
                .Icon("", (s) => s.Form?.IconPath??"",s => s.Form.Name)
                //.Hidden("IsValid",  s => s.Validation != 2)
                ;

            using (List.Suspender.Get())
            {
                Filters.Add(new FilterTextViewModel{Title = "Inn"}.Link(List,e => e.Inn));
                Filters.Add(new FilterTextViewModel{Title = "Dose"}.Link(List,e => e.Dose));
            }

        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}