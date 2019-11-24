using System;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Products;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Lims.Analysis.Module
{
    class ListSampleViewModel : EntityListViewModel<ListSampleViewModel,Sample>, IMvvmContextProvider
    {
        
        private readonly IErpServices _erp;

        private string GetStateIcon(int state)
        {
            switch(state)
            {
                case 1:
                    return "icons/Results/CheckFailed";
                case 2:
                    return "icons/Results/GaugeKO";
                case 3:
                    return "icons/Results/GaugeOK";
                default:
                    return "icons/Results/Gauge";
            }
        }

        [Import] public ListSampleViewModel(IErpServices erp) 
        {
            _erp = erp;
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                .Column("{Ref}",  s => s.Reference)
                .Column("{File}",  s => s.FileId.ToString())
                .Icon("",  s => s.Customer?.Country?.IconPath??"", s => s.Customer.Country.Name)
                .Column("{Customer}",  s => s.Customer.Name)
                .Column("{Product}",  s => s.Product.Caption)
                .Icon("{Form}", (s) => s.Product?.Form?.IconPath??"", s => s.Product.Form.Name)
                .Column("{Manufacturer}",  s => s.Manufacturer.Name)
                .Column("{Qty}",  s => s.ReceivedQuantity)
                .Column("{Expiration}",  s => s.ExpirationDate?.ToString(s.ExpirationDayValid ? "dd/MM/yyyy" : "MM/yyyy"), s=> s.ExpirationDate)
                .Column("{Notification}",  s => s.NotificationDate?.ToString("dd/MM/yyyy")??"", s => s.NotificationDate)
                .Column("{Validator}",  s => s.Validator)
                .Column("{Progress}",  s => new ProgressViewModel {Value = s.Progress ?? 0}, s=> s.Progress)
                .Icon("{State}",  s => s.State != null ? GetStateIcon(s.State.Value) : "", s=> s.State)
                .Hidden("IsValid",  s => s.Validation != 2);

            //List.AddFilter(e => e.State < 3);

            // Db.Fetch<Customer>();
            using (List.Suspender.Get())
            {
                Filters.Add(new FilterTextDesignViewModel()
                {
                    Title = "{Reference}",
                }.Link(List, s => s.Reference));

                
                Filters.Add(new FilterDateViewModel()
                {
                    Title = "{Expiration}",
                    MinDate = DateTime.Now.AddYears(-5),
                    MaxDate = DateTime.Now.AddYears(+5)
                }.Link(List, s => s.ExpirationDate));

                Filters.Add(new FilterDateViewModel()
                {
                    Title = "{Notification}",
                    MinDate = DateTime.Now.AddYears(-10),
                    MaxDate = DateTime.Now.AddYears(10)
                }.Link(List,s => s.NotificationDate));

                Filters.Add(new FilterDateViewModel()
                {
                    Title = "{Manufacturing}",
                    MinDate = DateTime.Now.AddYears(-10),
                    MaxDate = DateTime.Now.AddYears(10)
                }.Link(List,s => s.ManufacturingDate));

                Filters.Add(new FilterDateViewModel()
                {
                    Title = "{Sampling}",
                    MinDate = DateTime.Now.AddYears(-10),
                    MaxDate = DateTime.Now.AddYears(10)
                }.Link(List,s => s.SamplingDate));

                //Filters.Add(new FilterEntityViewModel<Product>(new ListProductPopupViewModel())
                //    {
                //        Title = "Product",
                //        Filters = {new FilterTextViewModel{Title = "Inn"}}
                //    }.Link(List, s => s.Product)
                //);
                


    /*            var f3 = new EntityFilterViewModel
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
            }

        }

        public string Title => "{Samples}";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
