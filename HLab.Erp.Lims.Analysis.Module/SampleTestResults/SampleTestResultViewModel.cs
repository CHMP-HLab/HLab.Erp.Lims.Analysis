using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xaml;
using System.Xml;
using HLab.Base.Extensions;
using HLab.Erp.Acl;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.SampleTestResults
{
    using H = H<SampleTestResultViewModel>;

    public class SampleTestResultViewModel : EntityViewModel<SampleTestResult>
    {
        private readonly Func<Sample, DataLocker<Sample>> _getSampleLocker;
        private readonly Func<SampleTest, DataLocker<SampleTest>> _getSampleTestLocker;

        public SampleTestResultViewModel(
            Func<FormHelper> getFormHelper, 
            Func<int, SampleTestResultAuditTrailViewModel> getAudit,
            Func<SampleTestResult, DataLocker<SampleTestResult>, SampleTestResultWorkflow> getWorkflow,
            Func<SampleTestResult, LinkedDocumentsListViewModel> getDocuments,
            Func<Sample, DataLocker<Sample>> getSampleLocker,
            Func<SampleTest, DataLocker<SampleTest>> getSampleTestLocker
            )
        {
            _getFormHelper = getFormHelper;
            _getAudit = getAudit;
            _getWorkflow = getWorkflow;
            _getDocuments = getDocuments;
            _getSampleLocker = getSampleLocker;
            _getSampleTestLocker = getSampleTestLocker;

            H.Initialize(this);
        }

        // Audit Trail
        private readonly Func<int, SampleTestResultAuditTrailViewModel> _getAudit;
        public SampleTestResultAuditTrailViewModel AuditTrail => _auditTrail.Get();
        private readonly IProperty<SampleTestResultAuditTrailViewModel> _auditTrail = H.Property<SampleTestResultAuditTrailViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e._getAudit?.Invoke(e.Model.Id))
            .On(e => e.Model)
            .Update()
        );

        public bool AuditDetail
        {
            get => _auditDetail.Get();
            set => _auditDetail.Set(value);
        }
        private readonly IProperty<bool> _auditDetail = H.Property<bool>();

        private ITrigger _onAuditDetail = H.Trigger(c => c
        .On(e => e.AuditDetail)
        .On(e => e.AuditTrail)
        .NotNull(e => e.AuditTrail)
        .Do(e =>
        {
            if(e.AuditDetail)
                e.AuditTrail.List.RemoveFilter("Detail");
            else
                e.AuditTrail.List.AddFilter(e => e.Motivation != null || e.Log.Contains("Stage=") || e.Log.Contains("StageId="),0,"Detail");

            e.AuditTrail.List.Update();
        }));

        private ITrigger _modelTrigger = H.Trigger(c => c
            .On(e => e.Model)
            .Do(e => e.Locker.AddDependencyLocker(
                e._getSampleLocker(e.Model.SampleTest.Sample),
                e._getSampleTestLocker(e.Model.SampleTest)
                )));

        private readonly Func<SampleTestResult, DataLocker<SampleTestResult>, SampleTestResultWorkflow> _getWorkflow;

        public SampleTestResultWorkflow Workflow => _workflow.Get();
        private readonly IProperty<SampleTestResultWorkflow> _workflow = H.Property<SampleTestResultWorkflow>(c => c
            .NotNull(e => e.Locker)
            .NotNull(e => e.Model)
            .Set(e => e._getWorkflow?.Invoke(e.Model, e.Locker))
            .On(e => e.Model)
            .On(e => e.Locker)
            .Update()
        );

        public bool IsReadOnly => _isReadOnly.Get();
        private readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
            .Set(e => !e.EditMode)
            .On(e => e.EditMode)
            .Update()
        );

        public bool EditMode => _editMode.Get();
        private readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .NotNull(e => e.Workflow)
            .NotNull(e => e.Locker)
            .Set(e =>
                e.Locker.IsActive
                && e.Workflow.CurrentStage == SampleTestResultWorkflow.Running
                && e.Model.SampleTest.Stage == SampleTestWorkflow.Running
                && e.Model.SampleTest.Sample.Stage == SampleWorkflow.Production
                && e.Acl.IsGranted(AnalysisRights.AnalysisResultEnter)
            )
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentStage)
            .Update()
        );


        public SampleTestViewModel Parent
        {
            get => _parent.Get();
            set => _parent.Set(value);
        }
        private readonly IProperty<SampleTestViewModel> _parent = H.Property<SampleTestViewModel>();


        private readonly Func<FormHelper> _getFormHelper;

        public FormHelper FormHelper => _formHelper.Get();
        private readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>(c => c
            .Set(e => e._getFormHelper?.Invoke()));

        private readonly ITrigger _ = H.Trigger(c => c
            .On(e => e.Model.Stage)
            //            .On(e => e.Model.Values)
            .On(e => e.EditMode)
            .OnNotNull(e => e.Workflow)
            .Do(async e => await e.LoadResultAsync())
        );

        public async Task LoadResultAsync()
        {
            await FormHelper.LoadAsync(Model).ConfigureAwait(true);
            FormHelper.Form.Mode = Workflow.CurrentStage == SampleTestResultWorkflow.Running ? FormMode.Capture : FormMode.ReadOnly;
        }

        // LINKED DOCUMENTS
        private readonly Func<SampleTestResult, LinkedDocumentsListViewModel> _getDocuments;

        public LinkedDocumentsListViewModel LinkedDocuments => _linkedDocuments.Get();
        private readonly IProperty<LinkedDocumentsListViewModel> _linkedDocuments = H.Property<LinkedDocumentsListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e._getDocuments?.Invoke(e.Model).FluentAction(vm => vm.SetOpenAction(d => e.OpenDocument(e.LinkedDocuments.Selected))))
            .On(e => e.Model)
            .Update()
        );

        public ICommand AddDocumentCommand { get; } = H.Command(c => c
            .CanExecute(e => e._addDocumentCanExecute())
            .Action((e, t) => e.AddDocument())
            .On(e => e.Workflow.CurrentStage).CheckCanExecute()
        );

        public ICommand OpenDocumentCommand { get; } = H.Command(c => c
            .CanExecute(e => e._addDocumentCanExecute())
            .Action((e, t) => e.OpenDocument(e.LinkedDocuments.Selected))
            .On(e => e.Workflow.CurrentStage).CheckCanExecute()
        );
        public ICommand PrintCommand { get; } = H.Command(c => c
            //.CanExecute(e => e._addDocumentCanExecute())
            .Action((e, t) => e.Print())
            .On(e => e.Workflow.CurrentStage).CheckCanExecute()
        );

        private void Print()
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                if (FormHelper.Form is Control visual)
                {
                    var f = visual.Foreground;
                    var b = visual.Background;

                    visual.Foreground = Brushes.Black;
                    visual.Background = Brushes.White;

                    var dic = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Themes/light.blue.xaml") };

                    visual.Resources.MergedDictionaries.Add(dic);
                     //           < ResourceDictionary Source = "pack://application:,,,/MahApps.Metro;component/Styles/Themes/light.blue.xaml" />

                     //string xaml = XamlServices.Save(visual);

                     //StringReader stringReader = new StringReader(xaml);
                     //XmlReader xmlReader = XmlReader.Create(stringReader);
                     //Visual visual2 = (Visual)XamlServices.Load(xmlReader);

                     printDialog.PrintVisual( visual, "My First Print Job");

                    visual.Foreground = f;
                    visual.Background = b;

                    visual.Resources.MergedDictionaries.Remove(dic);


                }
            }
        }

        private void OpenDocument(LinkedDocument selected)
        {
            var path = Path.GetTempFileName() + "_" + selected.Name;

            File.WriteAllBytes(path, selected.File);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private bool _addDocumentCanExecute()
        {
            if (!Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            if (Workflow.CurrentStage != SampleTestResultWorkflow.Running) return false;

            return true;
        }

        private void AddDocument()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "PDF Files (*.pdf)|*.pdf|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                var doc = Data.Add<LinkedDocument>(r =>
                {
                    r.Name = dlg.FileName.Split('\\').Last();
                    r.SampleTestResult = Model;
                    r.File = File.ReadAllBytes(dlg.FileName);
                });

                if (doc != null)
                    LinkedDocuments.List.Update();
            }

        }

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////
        /// </summary>




        public override string Header => _header.Get();
        private readonly IProperty<string> _header = H.Property<string>(c => c
            .Set(e => e.Model.SampleTest?.Sample?.Reference + " - " + e.Model.Name)
            .On(e => e.Model.SampleTest.Sample.Reference)
            .On(e => e.Model.Name)
            .Update()
            );

        public string SubTitle => _subTitle.Get();
        private readonly IProperty<string> _subTitle = H.Property<string>(c => c
            .Set(e => e.Model.SampleTest?.TestName + "\n" + e.Model.SampleTest?.Description.TrimEnd('\r', '\n', ' '))
            .On(e => e.Model.SampleTest.TestName)
            .On(e => e.Model.SampleTest.Description)
            .Update()
            );



    }
}
