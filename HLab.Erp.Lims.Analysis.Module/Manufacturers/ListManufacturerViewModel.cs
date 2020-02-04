using System;
using System.Collections.Generic;
using System.Text;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Products;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Lims.Analysis.Module.Manufacturers
{
    class ListManufacturerViewModel: EntityListViewModel<ListManufacturerViewModel,Manufacturer>, IMvvmContextProvider
    {
        public string Title => "Manufacturers";

        private readonly IErpServices _erp;

        [Import]
        public ListManufacturerViewModel(IErpServices erp)
        {
            AddAllowed = true;
            DeleteAllowed = true;

            _erp = erp;
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
//                .Column("Ref",  s => s.Caption)
                .Column("{Name}", e => e.Name)
//                .Column("Dose",e => e.Dose)
//                .Column("Form",e => e.Form)
                .ColumnAsync("", async (s) => await _erp.Icon.GetIconAsync(s.Country?.IconPath ?? "", 25), s => s.Country.Name)
                //.Hidden("IsValid",  s => s.Validation != 2)
                ;

            using (List.Suspender.Get())
            {
                Filters.Add(new FilterTextViewModel {Title = "{Name}"}.Link(List, e => e.Name));
            }
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}
