using System.Windows;
using System.Windows.Controls;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Lims.Analysis.Module
{
    class ListSampleAssayViewModel : EntityListViewModel<ListSampleAssayViewModel, SampleAssay>, IMvvmContextProvider
    {
        [Import]
        private IDataService _db;
        [Import]
        private IIconService _icons;

        private object GetIcon(int state,double size)
        {
            switch(state)
            {
                case 1:
                    return _icons.GetIcon("icons/Results/Gauge", size);
                case 2:
                    return _icons.GetIcon("icons/Results/GaugeKo",size);
                case 3:
                    return _icons.GetIcon("icons/Results/GaugeOk",size);
                default:
                    return _icons.GetIcon("icons/Results/Gauge",size);
            }
        }
        private object GetCheckIcon(int state,double size)
        {
            switch (state)
            {
                case 1:
                    return _icons.GetIcon("icons/Results/Running",size);
                case 2:
                    return _icons.GetIcon("icons/Results/CheckFailed",size);
                case 3:
                    return _icons.GetIcon("icons/Results/CheckPassed",size);
                default:
                    return _icons.GetIcon("icons/Results/Running",size);
            }
        }

        public ListSampleAssayViewModel(int sampleId)
        {
            List.AddFilter(()=>e => e.SampleId == sampleId);
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                .Column("",s=>_icons.GetIcon(s.AssayClass.IconName, 25.0))
                .Column("^Assay", s => new StackPanel{
                    VerticalAlignment = VerticalAlignment.Top,
                    Children =
                    {
                        new TextBlock{Text=s.AssayName,FontWeight = FontWeights.Bold},
                        new TextBlock{Text = s.Description, FontStyle = FontStyles.Italic}
                    }})
                .Column("^Specifications", s => s.Specification)
                .Column("^Result", s => s.Result)
            //.Column("Conformity", s => s.AssayStateId);
                .Column("^State", s => GetIcon(s.AssayStateId??0,25))
                .Column("^Validation", s => GetCheckIcon(s.Validation??0,25))
                .Hidden("IsValid", s => s.Validation!=2)
                .Hidden("Group", s => s.AssayClassId);
            //List.AddFilter(e => e.State < 3);

            // Db.Fetch<Customer>();

            //Filters.Add(new DateFilterViewModel<Sample>().Configure(
            //    "ExpirationDate",
            //    "Expiration", 
            //    s => s.ExpirationDate, 
            //    List));

            //Filters.Add(new DateFilterViewModel<Sample>().Configure(
            //    "NotificationDate", 
            //    "Notification", 
            //    s => s.NotificationDate, 
            //    List));

            //Filters.Add(new EntityFilterViewModel<Sample,Customer>().Configure(
            //    "Customer",
            //    "Customer", 
            //    s => s.Customer, 
            //    List));

            //Filters.Add(new EntityFilterViewModel<Sample, Manufacturer>().Configure(
            //    "Manufacturer", 
            //    "Manufacturer", 
            //    s => s.Manufacturer, 
            //    List));

            List.Update();
        }

        public string Title => "Sample";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
