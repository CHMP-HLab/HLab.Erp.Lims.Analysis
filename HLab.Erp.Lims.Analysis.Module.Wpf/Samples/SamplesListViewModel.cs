using System;
using System.Linq;
using System.Threading.Tasks;
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.Wpf.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Samples;

public class SamplesListViewModel : Core.EntityLists.EntityListViewModel<Sample>, IMvvmContextProvider
{
    public class Bootloader : NestedBootloader
    { }

    protected override bool CanExecuteAdd(Action<string> errorAction) => true;
    protected override bool CanExecuteDelete(Sample target,Action<string> errorAction) => Selected!=null || (SelectedIds?.Any()??false);

    const double Height = 30;

    public SamplesListViewModel(Injector i) : base(i, c => c


            .Column("Reference")
            .Header("{Reference}")
            .Width(80).Height(Height)
            .Link(s => s.Reference)
            .OrderByDesc(1)
            .Filter()
            .IconPath("Icons/Entities/Sample")

            .Column("FieldId")
            .Header("{FileId}")
            .Width(100).Height(Height)
            .OrderBy(s => s.FileId)
            .Link(s => s.FileId)
            .Filter()

            .Column("Reception")
            .Header("{Reception}")
            .Width(100).Height(Height)
            .Date(s => s.ReceptionDate)
            .OrderByDesc(0)
            .Link(s => s.ReceptionDate)
            .Filter()

            
            .ColumnListable(e => e.Customer , "Customer").Width(250).Height(Height)
 
            .ColumnListable(e => e.Product, "Product").Width(550).Height(Height)

            .ColumnListable(e => e.Manufacturer, "Manufacturer").Width(250).Height(Height)

            .Column("Qty")
            .Header("{Qty}")
            .Width(50).Height(Height)
            .Content(s => s.ReceivedQuantity)
            // TODO :create IntFilter                        .Filter()

            .Column("Expiration")
            .Header("{Expiration}")
            .Width(100).Height(Height)
            .OrderBy(s => s.ExpirationDate)
            .Date(s => s.ExpirationDate, s => s.ExpirationDayValid)
            .Link(s => s.ExpirationDate)
            .Filter()
            .MinDate(DateTime.Now.AddYears(-5))
            .MaxDate(DateTime.Now.AddYears(+5))
            .IconPath("Icons/Sample/Date")
            //.Link(s => s.ExpirationDate)

            .Column("Notification")
            .Header("{Notification}")
            .Width(100).Height(Height)
            .Date(s => s.NotificationDate)
            .Link(s => s.NotificationDate)
            .Filter()
            .MinDate(DateTime.Now.AddYears(-10))
            .MaxDate(DateTime.Now.AddYears(10))
            .IconPath("Icons/Sample/Notification|Icons/Sample/Date")

            .Column("Validator")
            .Header("{Validator}")
            .Width(140).Height(Height)
            .Content(s => s.Validator)
            .OrderBy(e => e.Validator.Name)

            .ProgressColumn(s => s.Progress).Height(Height)

            .ConformityColumn(s => s.ConformityId).Height(Height)

            .StageColumn(default(SampleWorkflow), s => s.StageId).Height(Height)

            //.Column("Pharmacopoeia")
            //.Header("{Pharmacopoeia}")
            //.ColumnListable(s => s.Pharmacopoeia)
            //.Link(s => s.Pharmacopoeia)
            //.Filter().IconPath("Icons/Entities/Pharmacopoeia")

            .Column("Batch").Width(0).Hidden()
            .Header("{Batch}")
            .Link(s => s.Batch)
            .Filter()
            .IconPath("Icons/Sample/BarCode").Height(Height)

            .Column("Manufacturing").Width(0).Hidden()
            .Header("{Manufacturing}")
            .IconPath("Icons/Entities/Manufacturer|Icons/Sample/Date")
            .Width(100).Height(Height)
            .Date(s => s.ManufacturingDate)
            .Link(s => s.ManufacturingDate)
            .Filter()
            .MinDate(DateTime.Now.AddYears(-10))
            .MaxDate(DateTime.Now.AddYears(10))

            .Column("Sampling").Width(0).Hidden()
            .Header("{Sampling}")
            .IconPath("Icons/Sample/Sampling|Icons/Sample/Date")
            .Width(100).Height(Height)
            .Date(s => s.SamplingDate)
            .Link(s => s.SamplingDate)
            .Filter()
            .MinDate(DateTime.Now.AddYears(-10))
            .MaxDate(DateTime.Now.AddYears(10))

            .Column("Origin").Width(0).Hidden()
            .Header("{Origin}").Height(Height)
            .Link(s => s.SamplingOrigin)
            .Filter()
            .IconPath("Icons/Sample/Location")

            .Column("CommercialName").Width(0).Hidden()
            .Header("{Commercial Name}").Height(Height)
            .Link(s => s.CommercialName)
            .Filter()
            .IconPath("Icons/Sample/Commercial")

            .AddProperty("IsValid",s=>true,s=>true)
/**/
    )
    {
        var n = SampleWorkflow.Reception; // HACK : this is a hack to force top level static constructor
    }

    protected override Task ConfigureNewEntityAsync(Sample s, object arg)
    {
        s.Stage = SampleWorkflow.DefaultStage;

        return Task.CompletedTask;
    }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }
}