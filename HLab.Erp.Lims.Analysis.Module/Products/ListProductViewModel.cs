using System;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ListProductViewModel: EntityListViewModel<ListProductViewModel,Product>, IMvvmContextProvider
    {
        private readonly IErpServices _erp;
        [Import] public ListProductViewModel(IErpServices erp) 
        {
            _erp = erp;
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                .Column("Ref",  s => s.Caption)
                .Column("Inn",e => e.Inn)
                .Column("Dose",e => e.Dose)
                .Column("Form",e => e.Form)
                .Column("", async (s) => await _erp.Icon.GetIcon(s.Form?.IconPath??"",25))
                //.Hidden("IsValid",  s => s.Validation != 2)
                ;

            //List.AddFilter(e => e.State < 3);

            // Db.Fetch<Customer>();
            using (List.Suspender.Get())
            {

            }

        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}