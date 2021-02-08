﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.SampleTestResults
{
    using H = H<SampleTestResultViewModel>;

    public class SampleTestResultViewModelDesign : SampleTestResultViewModel, IViewModelDesign
    {
    }

    public class SampleTestResultViewModel : EntityViewModel<SampleTestResult>
    {
        public SampleTestResultViewModel()
        {
            H.Initialize(this);
            FormHelper = new FormHelper();
        }

        [Import] private IDataService _data;
        [Import] private Func<SampleTestResult, DataLocker<SampleTestResult>, SampleTestResultWorkflow> _getWorkflow;
        [Import] public IErpServices Erp { get; set; }

        public SampleTestResultWorkflow Workflow => _workflow.Get();

        private readonly IProperty<SampleTestResultWorkflow> _workflow = H.Property<SampleTestResultWorkflow>(c => c
            .Set(e => e.Locker!=null ? e._getWorkflow(e.Model,e.Locker):null)
            .On(e => e.Model)
            .On(e => e.Locker)
            .NotNull(e => e.Locker)
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
            .Set(e => 
                e.Locker != null 
                && e.Locker.IsActive 
                && e.Workflow != null
                && e.Workflow.CurrentState == SampleTestResultWorkflow.Running
                && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisResultEnter)
            )
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentState)
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Update()
        );


        public string Conformity => _conformity.Get();

        private readonly IProperty<string> _conformity = H.Property<string>(c => c
            .Set(e =>
                {
                    switch(e.Model.StateId)
                    {
                        case -1 : return "{Undefined}";
                        case 0 : return "{Not Started}";
                        case 1 : return "{Running}";
                        case 2 : return "{Not Conform}";
                        case 3 : return "{Conform}";
                        case 4 : return "{Not Valid}";
                        default: return "{error}";
                    } 
                }            
            )
            .On(e => e.Model.StateId)
            .Update()
        );

        public string ConformityIconPath => _conformityIconPath.Get();

        private readonly IProperty<string> _conformityIconPath = H.Property<string>(c => c
            .Set(e =>
                {
                    switch(e.Model.StateId)
                    {
                        case -1 : return "Icons/Validations/Error";
                        case 0 : return "Icons/Results/NotChecked";
                        case 1 : return "Icons/Results/Running";
                        case 2 : return "Icons/Results/GaugeKO";
                        case 3 : return "Icons/Results/GaugeOK";
                        case 4 : return "Icons/Results/Invalidated";
                        default: return "Icons/Validations/Error";
                    } 
                }   )         
            .On(e => e.Model.StateId)
            .Update()
        );

        public SampleTestViewModel Parent
        {
            get => _parent.Get();
            set => _parent.Set(value);
        }
        private readonly IProperty<SampleTestViewModel> _parent = H.Property<SampleTestViewModel>();

        public FormHelper FormHelper
        {
            get => _formHelper.Get();
            set => _formHelper.Set(value);
        }
        private readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>();

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

            FormHelper.Mode = Workflow.CurrentState == SampleTestResultWorkflow.Running ? TestFormMode.Capture : TestFormMode.ReadOnly;
        }



        // LINKED DOCUMENTS
        [Import] private readonly Func<int, ListLinkedDocumentViewModel> _getDocuments;
        public ListLinkedDocumentViewModel LinkedDocuments => _linkedDocuments.Get();
        private readonly IProperty<ListLinkedDocumentViewModel> _linkedDocuments = H.Property<ListLinkedDocumentViewModel>(c => c
            .Set(e =>
            {
                if (e.Model == null) return null;
                var vm =  e._getDocuments(e.Model.Id);
                vm.SetOpenAction(d => e.OpenDocument(e.LinkedDocuments.Selected));
                return vm;
            })
            .On(e => e.Model)
            .Update()
        );

        public ICommand AddDocumentCommand { get; } = H.Command(c => c
            .CanExecute(e => e._addDocumentCanExecute())
            .Action((e,t) => e.AddDocument())
            .On(e => e.Workflow.CurrentState).CheckCanExecute()
        );
        public ICommand OpenDocumentCommand { get; } = H.Command(c => c
            .CanExecute(e => e._addDocumentCanExecute())
            .Action((e,t) => e.OpenDocument(e.LinkedDocuments.Selected))
            .On(e => e.Workflow.CurrentState).CheckCanExecute()
        );

        private void OpenDocument(LinkedDocument selected)
        {
            var path = Path.GetTempFileName()+"_" + selected.Name;

            File.WriteAllBytes(path, selected.File);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            };
            Process.Start (psi);            
        }

        private bool _addDocumentCanExecute()
        {
            if(!Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            if(Workflow.CurrentState != SampleTestResultWorkflow.Running) return false;

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
                var doc = _data.Add<LinkedDocument>(r =>
                {
                    r.Name = dlg.FileName.Split('\\').Last();
                    r.SampleTestResult = Model;
                    r.File = File.ReadAllBytes(dlg.FileName);
                });

                if (doc != null)
                    LinkedDocuments.List.UpdateAsync();
            }

        }

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////
        /// </summary>




        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c
            .Set(e => e.Model.SampleTest.Sample?.Reference + " - " + e.Model.Name)
            .On(e => e.Model.SampleTest.Sample.Reference)
            .On(e => e.Model.Name)
            .Update()
            );

        public string SubTitle => _subTitle.Get();
        private readonly IProperty<string> _subTitle = H.Property<string>(c => c
            .Set(e => e.Model.SampleTest.TestName + "\n" + e.Model.SampleTest.Description)
            .On(e => e.Model.SampleTest.TestName)
            .On(e => e.Model.SampleTest.Description)
            .Update()
            );



    }
}
