using System;
using System.Linq;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class SamplesListViewModel : EntityListViewModel<Sample>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        { }

        protected override bool CanExecuteAdd(Action<string> errorAction) => true;
        protected override bool CanExecuteDelete(Sample target,Action<string> errorAction) => Selected!=null || (SelectedIds?.Any()??false);

        public SamplesListViewModel() : base(c => c

                .Column()
                    .Header("{Reference}")
                    .Width(80)
                    .Link(s => s.Reference)
                        .Filter()
                        .IconPath("Icons/Entities/Sample")

                .Column()
                    .Header("{FileId}")
                    .Width(100)
                    .OrderBy(s => s.FileId)
                    .Link(s => s.FileId)
                        .Filter()

                .Column()
                .Header("{Reception}")
                .Width(75)
//                .OrderByOrder(0)
                .Link(s => s.ReceptionDate)
                .Filter()

                .Column(e => e.Customer).Mvvm().Width(250)

                .Column(e => e.Product).Mvvm().Width(550)

                //.PostLinkedColumn(s => s.Product?.Form, s => s.Product?.FormId)


                .Column(e => e.Manufacturer).Mvvm().Width(250)

                .Column()
                .Header("{Qty}")
                .Width(50)
                .Content(s => s.ReceivedQuantity)
                // TODO :create IntFilter                        .Filter()

                .Column()
                .Header("{Expiration}")
                .Width(75)
                .OrderBy(s => s.ExpirationDate)
                .Content(s => s.ExpirationDate?.ToString(s.ExpirationDayValid ? "dd/MM/yyyy" : "MM/yyyy"))
                .Link(s => s.ExpirationDate)
                .Filter()

                .Header("{Expiration}")
                .MinDate(DateTime.Now.AddYears(-5))
                .MaxDate(DateTime.Now.AddYears(+5))
                .IconPath("Icons/Sample/Date")
                .Link(s => s.ExpirationDate)

                .Column()
                .Header("{Notification}")
                .Width(75)
                .Content(s => s.NotificationDate?.ToString("dd/MM/yyyy") ?? "")
                .Link(s => s.NotificationDate)
                    .Filter()
                        .MinDate(DateTime.Now.AddYears(-10))
                        .MaxDate(DateTime.Now.AddYears(10))
                        .IconPath("Icons/Sample/Notification|Icons/Sample/Date")

                .Column()
                    .Header("{Validator}")
                    .Width(140)
                    .Content(s => s.Validator)

                .ProgressColumn(s => s.Progress)

                .ConformityColumn(s => s.ConformityId)

                .StageColumn(default(SampleWorkflow), s => s.StageId)

                .Column().Hidden()
                    .Header("{Pharmacopoeia}")
                    .Link(s => s.Pharmacopoeia).Mvvm()
                    .Filter()

                .Column().Hidden()
                    .Header("{Batch}")
                    .Link(s => s.Batch)
                    .Filter()
                    .IconPath("Icons/Sample/BarCode")

                .Column().Hidden()
                    .Header("{Manufacturing}")
                    .IconPath("Icons/Entities/Manufacturer|Icons/Sample/Date")
                    .Link(s => s.ManufacturingDate)
                    .Filter()
                        .MinDate(DateTime.Now.AddYears(-10))
                        .MaxDate(DateTime.Now.AddYears(10))

                .Column().Hidden()
                    .Header("{Sampling}")
                    .IconPath("Icons/Sample/Sampling|Icons/Sample/Date")
                    .Link(s => s.SamplingDate)
                    .Filter()
                        .MinDate(DateTime.Now.AddYears(-10))
                        .MaxDate(DateTime.Now.AddYears(10))

                .Column().Hidden()
                    .Header("{Origin}")
                    .Link(s => s.SamplingOrigin)
                    .Filter()
                        .IconPath("Icons/Sample/Location")

                .Column().Hidden()
                    .Header("{Commercial Name}")
                    .Link(s => s.CommercialName)
                    .Filter()
                        .IconPath("Icons/Sample/Commercial")

        )
        {
            var n = SampleWorkflow.Reception; // TODO : this is a hack to force top level static constructor
        }

        protected override void ConfigureEntity(Sample sample)
        {
            sample.Stage = SampleWorkflow.DefaultStage;
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
