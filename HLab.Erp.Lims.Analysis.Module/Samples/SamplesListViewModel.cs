using System;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Filters;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class SamplesListViewModel : EntityListViewModel<Sample>, IMvvmContextProvider
    {
        public class SampleListBootloader : ErpDataBootloader<SamplesListViewModel>
        { }

        public SamplesListViewModel() : base(c => c
            .AddAllowed()
            .DeleteAllowed()

                .Column()
                    .Header("{Reference}")
                    .Width(80)
                    .Link(s => s.Reference)
                        .Filter()
                            .IconPath("Icons/Entities/Sample")

                .Column()
                    .Header("{FileId}")
                    .Width(100)
                    .Content(s => s.FileId?.ToString() ?? "")
                    .OrderBy(s => s.FileId)
                    .Link(s => s.FileId)
                        .Filter()

                .Column()
                    .Header("{Reception}")
                    .Width(75)
                    .OrderByOrder(0)
                    .Link(s=>s.ReceptionDate)
                        .Filter()

                .Column(e => e.Customer/*, e => e.CustomerId*/)

                .Column(e => e.Product)

                //.Column()
                //    .Header("{Product}")
                //    .Width(400)
                //    .Content(s => s.Product?.Caption)
                //    .Filter<EntityFilterNullable<Product,ProductsListPopupViewModel>>()
                //        .PostLink(s => s.ProductId)


                .PostLinkedColumn( s => s.Product?.Form, s=>s.Product.FormId)


                .Column(e => e.Manufacturer, e=> e.ManufacturerId)

                .Column()
                    .Header("{Qty}")
                    .Width(50)
                    .Content(s => s.ReceivedQuantity)
// TODO :create IntFilter                        .Filter()

                .Column()
                    .Header("{Expiration}")
                    .Width(75)
                    .Content(s => s.ExpirationDate?.ToString(s.ExpirationDayValid ? "dd/MM/yyyy" : "MM/yyyy"))
                    .OrderBy(s => s.ExpirationDate)
                    .Filter<DateFilter>()
                        .Header("{Expiration}")
                        .MinDate(DateTime.Now.AddYears(-5))
                        .MaxDate(DateTime.Now.AddYears(+5))
                        .IconPath("Icons/Sample/Date")
                        .Link(s => s.ExpirationDate)

                .Column()
                    .Header("{Notification}")
                    .Width(75)
                    .Content(s => s.NotificationDate?.ToString("dd/MM/yyyy") ?? "")
                    .OrderBy(s => s.NotificationDate)
                    .Filter<DateFilter>()
                        .Header("{Notification}")
                        .MinDate(DateTime.Now.AddYears(-10))
                        .MaxDate(DateTime.Now.AddYears(10))
                        .IconPath("Icons/Sample/Notification|Icons/Sample/Date")
                        .Link(s => s.NotificationDate)

                .Column()
                    .Header("{Validator}")
                    .Width(140)
                    .Content(s => s.Validator)

                .ProgressColumn(s => s.Progress)

                .ConformityColumn(s => s.ConformityId)

                .StageColumn(default(SampleWorkflow),s => s.Stage)

                .Filter<EntityFilterNullable<Pharmacopoeia>>()
                    .Header("{Pharmacopoeia}")
                    .Link(s => s.PharmacopoeiaId)

                .Filter<TextFilter>()
                    .Header("{Batch}")
                    .IconPath("Icons/Sample/BarCode")
                    .Link(s => s.Batch)
            
                .Filter<DateFilter>()
                    .Header("{Manufacturing}")
                    .MinDate(DateTime.Now.AddYears(-10))
                    .MaxDate(DateTime.Now.AddYears(10))
                    .IconPath("Icons/Entities/Manufacturer|Icons/Sample/Date")
                    .Link(s => s.ManufacturingDate)

                .Filter<DateFilter>()
                    .Header("{Sampling}")
                    .MinDate(DateTime.Now.AddYears(-10))
                    .MaxDate(DateTime.Now.AddYears(10))
                    .IconPath("Icons/Sample/Sampling|Icons/Sample/Date")
                    .Link(s => s.SamplingDate)

               .Filter<TextFilter>()
                    .Header("{Origin}")
                    .IconPath("Icons/Sample/Location")
                    .Link(s => s.SamplingOrigin)

                .Filter<TextFilter>()
                    .Header("{Commercial Name}")
                    .IconPath("Icons/Sample/Commercial")
                    .Link(s => s.CommercialName)

                .Filter<WorkflowFilter<SampleWorkflow>>()
                    .Header("{Stage}")
                    .IconPath("Icons/Workflow")
                    .Link(e => e.Stage)

                .Filter<ConformityFilter>()
                    .Header("{Conformity}")
                    .IconPath("Icons/Conformity")
                    .Link(e => e.ConformityId)

        )
        {
            var n = SampleWorkflow.Reception; // TODO : this is a hack to force top level static constructor
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
