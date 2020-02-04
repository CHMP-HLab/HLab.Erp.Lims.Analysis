﻿using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    class ListTestCategoryViewModel : EntityListViewModel<ListTestCategoryViewModel,TestCategory>, IMvvmContextProvider
    {
        
        [Import] private readonly IErpServices _erp;

 
        public ListTestCategoryViewModel() 
        {
            AddAllowed = true;
            DeleteAllowed = true;

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                .Column("", s => new IconView { Path = s.IconPath, Width = 30 })
                .Column("{Name}", s => s.Name)
                //.Hidden("IsValid",  s => s.Validation != 2)
                ;
            using (List.Suspender.Get())
            {

                List.UpdateAsync();
            }
        }

        public string Title => "{Test categories}";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }


    class ListTestClassViewModel : EntityListViewModel<ListTestClassViewModel,TestClass>, IMvvmContextProvider
    {
        
        [Import] private readonly IErpServices _erp;

 
        public ListTestClassViewModel() 
        {
            AddAllowed = true;
            DeleteAllowed = true;

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                .Column("", s => new IconView { Path = s.IconPath, Width = 30 })
                .Column("{Name}", s => s.Name)
                .Column("{Category}", s => s.Category.ToString())
                //.Hidden("IsValid",  s => s.Validation != 2)
                ;
            using (List.Suspender.Get())
            {
/*
                Filters.Add(new FilterDateViewModel()
                {
                    Title = "^Expiration",
                    MinDate = DateTime.Now.AddYears(-5),
                    MaxDate = DateTime.Now.AddYears(+5)
                }.Link(List, s => s.ExpirationDate));


                var f3 = new EntityFilterViewModel
                {
                    Title = "^Customer"
                };
                List.AddFilter(s => f3.Match(s.CustomerId));
                Filters.Add(f3);

                var f4 = new EntityFilterViewModel
                {
                    Title = "^Manufacturer"
                };
                List.AddFilter(s => f4.Match(s.ManufacturerId));
                Filters.Add(f4);
    */
                List.UpdateAsync();
            }
        }

        public string Title => "Sample";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
