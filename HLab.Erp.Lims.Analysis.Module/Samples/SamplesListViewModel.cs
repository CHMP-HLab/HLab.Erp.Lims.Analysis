using System;
using System.Linq;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{

    public class ProductsListPopupViewModel : EntityListViewModel<Product>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public override string Title => "{Products}";
        [Import]
        public ProductsListPopupViewModel()
        {

            Columns.Configure(c => c
                .Column
                    .Header("{Inn}")
                    .Width(200)
                    .Content(p => p.Inn)
                .Column
                    .Header("{Dose}")
                    .Width(100)
                    .Content(p => p.Dose)
                .FormColumn(p => p.Form)
                );
//                .Column("{Ref}", s => s.Caption)
            using (List.Suspender.Get())
            {
                Filter<TextFilter>(f => f.Title("{Inn}")
                    .Link(List,e => e.Inn));

                Filter<TextFilter>(f => f.Title("{Dose}")
                    .Link(List,e => e.Dose));

                Filter<EntityFilter<Form>>()
                    .Link(List, e => e.FormId);
            }

        }
    }


    internal class SamplesListViewModel : EntityListViewModel<Sample>, IMvvmContextProvider
    {

        private readonly IErpServices _erp;

        [Import] public SamplesListViewModel(IErpServices erp)
        {
            var n = SampleWorkflow.Reception; // TODO : this is a hack to force top level static constructor

            AddAllowed = true;
            DeleteAllowed = true;

            _erp = erp;

            Columns.Configure(c => c
                    .Column.Header("{Ref}").Width(80).Content(s => s.Reference)
                    .Column.Header("{FileId}").Width(100).Content(s => s.FileId?.ToString() ?? "").OrderBy(s => s.FileId)
                    .Column.Header("{Reception}").Width(75).Content(s=>s.ReceptionDate).OrderByOrder(0)
                    .Column.Header("{Customer}").Width(140).Content(s => s.Customer?.Name).Icon(s => s.Customer?.Country?.IconPath ?? "").OrderBy(s => s.Customer?.Caption)
                    .Column.Header("{Product}").Width(400).Content(s => s.Product?.Caption)
                    .FormColumn(s => s.Product?.Form)//?.Name).Icon((s) => s.Product?.Form?.IconPath ?? "")
                    .Column.Header("{Manufacturer}").Width(140).Content(s => s.Manufacturer?.Name)
                    .Column.Header("{Qty}").Width(50).Content(s => s.ReceivedQuantity)
                    .Column.Header("{Expiration}").Width(75).Content(s => s.ExpirationDate?.ToString(s.ExpirationDayValid ? "dd/MM/yyyy" : "MM/yyyy")).OrderBy(s => s.ExpirationDate)
                    .Column.Header("{Notification}").Width(75).Content(s => s.NotificationDate?.ToString("dd/MM/yyyy") ?? "").OrderBy(s => s.NotificationDate)
                    .Column.Header("{Validator}").Width(140).Content(s => s.Validator)
                    .ProgressColumn(s => s.Progress)
                    .StageColumn(s => SampleWorkflow.StageFromName(s.Stage))
                );


            using (List.Suspender.Get())
            {
                Filter<EntityFilter<Product,ProductsListPopupViewModel>>(f => f.Title("{Product}"))
                    .Link(List, s => s.ProductId);

                Filter<EntityFilter<Customer>>(f => f.Title("{Customer}"))
                    .Link(List, s => s.CustomerId);

                Filter<EntityFilter<Manufacturer>>(f => f.Title("{Manufacturer}"))
                    .Link(List, s => s.ManufacturerId);

                Filter<EntityFilter<Pharmacopoeia>>(f => f.Title("{Pharmacopoeia}"))
                    .Link(List, s => s.PharmacopoeiaId);

                Filter<TextFilter>()
                    .Title("{Reference}")
                    .IconPath("Icons/Entities/Sample")
                    .Link(List, s => s.Reference);


                Filter<TextFilter>()
                    .Title("{Batch}")
                    .IconPath("Icons/Sample/BarCode")
                    .Link(List, s => s.Batch);

                Filter<DateFilter>()
                    .Title("{Expiration}")
                    .MinDate(DateTime.Now.AddYears(-5))
                    .MaxDate(DateTime.Now.AddYears(+5))
                    .IconPath("Icons/Sample/Date")
                    .Link(List, s => s.ExpirationDate);

                Filter<DateFilter>()
                    .Title("{Notification}")
                    .MinDate(DateTime.Now.AddYears(-10))
                    .MaxDate(DateTime.Now.AddYears(10))
                    .IconPath("Icons/Sample/Notification|Icons/Sample/Date")
                    .Link(List, s => s.NotificationDate);

                Filter<DateFilter>()
                    .Title("{Manufacturing}")
                    .MinDate(DateTime.Now.AddYears(-10))
                    .MaxDate(DateTime.Now.AddYears(10))
                    .IconPath("Icons/Entities/Manufacturer|Icons/Sample/Date")
                    .Link(List, s => s.ManufacturingDate);

                Filter<DateFilter>()
                    .Title("{Sampling}")
                    .MinDate(DateTime.Now.AddYears(-10))
                    .MaxDate(DateTime.Now.AddYears(10))
                    .IconPath("Icons/Sample/Sampling|Icons/Sample/Date")
                    .Link(List, s => s.SamplingDate);

                Filter<TextFilter>()
                    .Title("{Origin}")
                    .IconPath("Icons/Sample/Location")
                    .Link(List, s => s.SamplingOrigin);

                Filter<TextFilter>()
                    .Title("{Commercial Name}")
                    .IconPath("Icons/Sample/Commercial")
                    .Link(List, s => s.CommercialName);

                Filter<WorkflowFilterViewModel<SampleWorkflow>>()
                    .Title("{Stage}")
                    .IconPath("Icons/Workflow")
                    .Link(List, e => e.Stage);

            }

        }

        protected override void ConfigureEntity(Sample sample)
        {
             sample.Stage = SampleWorkflow.DefaultStage.Name;
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
