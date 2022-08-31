using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Samples.SampleTests;

public class LinkedDocumentsListViewModel : Core.EntityLists.EntityListViewModel<LinkedDocument>, IMvvmContextProvider
{
    readonly SampleTestResult _result;
    public LinkedDocumentsListViewModel(Injector i,
        SampleTestResult result
    //            Expression<Func<LinkedDocument,bool>> filter,
    //            Action<LinkedDocument> createAction = null
    ) : base(i, c => c
        .HideFilters()
        .StaticFilter(e => e.SampleTestResultId == result.Id)
        //.DeleteAllowed()
        .Column("Name")
        .Header("{Name}").Width(200).Content(s => s.Name)
    )
    {
        _result = result;
    }

    protected override async Task ConfigureNewEntityAsync(LinkedDocument doc, object arg)
    {
        Microsoft.Win32.OpenFileDialog dlg = new()
        {
            DefaultExt = ".pdf",
            Filter = "PDF Files (*.pdf)|*.pdf|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"
        };

        if (!dlg.ShowDialog() ?? false) throw new DataSetterException("User cancelled");

        doc.SampleTestResult = _result;
        doc.Name = dlg.FileName.Split('\\').Last();
        doc.File = await File.ReadAllBytesAsync(dlg.FileName);
    }

    protected override bool CanExecuteDelete(LinkedDocument doc, Action<string> errorAction) => Selected != null;

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }
}