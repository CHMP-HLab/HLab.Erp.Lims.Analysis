using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    class ListTestClassViewModel : EntityListViewModel<ListTestClassViewModel,TestClass>, IMvvmContextProvider
    {
        
        private readonly IErpServices _erp;

 
        [Import] public ListTestClassViewModel(IErpServices erp) 
        {
            _erp = erp;
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            _ = Columns
                .Column("", s => new IconView { Id = s.IconPath, Width = 30 })
                .Column("^Name", s => s.Name)
                .Column("^Category", s => s.Category.ToString())
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
                List.Update();
            }

        }

        public string Title => "Sample";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
