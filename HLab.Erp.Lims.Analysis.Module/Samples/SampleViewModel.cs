using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using HLab.Erp.Acl;
using HLab.Erp.Base.Wpf;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

using Outils;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    using H = H<SampleViewModel>;


    public class SampleViewModelDesign : SampleViewModel.Design
    {

    }

    public class SampleViewModel : EntityViewModel<Sample>
    {

        public class Design : SampleViewModel, IViewModelDesign
        {
            public new Sample Model { get; } = Sample.DesignModel;
        }

        public Type ListProductType => typeof(ProductsListPopupViewModel);

        private readonly Func<Sample, IDataLocker<Sample>, SampleWorkflow> _getSampleWorkflow;

        private SampleViewModel() { }

        public SampleViewModel(
            Func<Sample, SampleSampleTestListViewModel> getTests,
            Func<Sample, SampleFormsListViewModel> getForms,
            Func<int, SampleAuditTrailViewModel> getAudit,
            Func<Sample, IDataLocker<Sample>, SampleWorkflow> getSampleWorkflow
            )
        {
            _getAudit = getAudit;
            _getSampleWorkflow = getSampleWorkflow;
            _getTests = getTests;
            H.Initialize(this);
            _getForms = getForms;
        }

        public override object Header => _header.Get();
        private readonly IProperty<object> _header = H.Property<object>(c => c
            .Set(e => (object)e.Model?.Reference ?? "{New sample}")
            .On(e => e.Model.Reference)
            .NotNull(e => e.Model)
            .Update()
        );
        public string SubTitle => _subTitle.Get();
        private readonly IProperty<string> _subTitle = H.Property<string>(c => c
            .Set(e => e.Model?.Customer?.Name ?? "{Customer}" + "\n" + e.Model?.Product?.Caption ?? "{Product}")
            .On(e => e.Model.Customer.Name)
            .On(e => e.Model.Product.Caption)
            .NotNull(e => e.Model)
            .Update()
        );
 
        public bool IsReadOnly => _isReadOnly.Get();
        private readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
            .Set(e => !e.EditMode)
            .On(e => e.EditMode)
            .Update()
        );

        // Audit Trail
        public SampleAuditTrailViewModel AuditTrail => _auditTrail.Get();
        private readonly IProperty<SampleAuditTrailViewModel> _auditTrail = H.Property<SampleAuditTrailViewModel>(c => c
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

        public bool EditMode => _editMode.Get();
        private readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Set(e => e.Locker.IsActive
                       && e.Workflow.CurrentStage == SampleWorkflow.Reception
                       && e.Acl.IsGranted(AnalysisRights.AnalysisReceptionSign))
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentStage)
            .Update()
        );

        public Visibility CustomerVisibility => _customerVisibility.Get();
        private readonly IProperty<Visibility> _customerVisibility = H.Property<Visibility>(c => c
            .Set(e => e.Acl.IsGranted(ErpRights.ErpViewCustomer) ? Visibility.Visible : Visibility.Hidden)
            .On(e => e.Acl.Connection.User)
            .Update()
        );

        public bool IsReadOnlyMonograph => _isReadOnlyMonograph.Get();
        private readonly IProperty<bool> _isReadOnlyMonograph = H.Property<bool>(c => c
            .Set(e => !e.MonographMode)
            .On(e => e.MonographMode)
            .Update()
        );

        public bool MonographMode => _monographMode.Get();
        private readonly IProperty<bool> _monographMode = H.Property<bool>(c => c
            .Set(e =>
            {
                if (e.Locker == null) return false;
                if (e.Workflow == null) return false;
                return e.Locker.IsActive
                       && e.Workflow.CurrentStage == SampleWorkflow.Monograph
                       && e.Acl.IsGranted(AnalysisRights.AnalysisMonographSign);
            })
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentStage)
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Update()
        );

        public bool IsReadOnlyProduction => _isReadOnlyProduction.Get();
        private readonly IProperty<bool> _isReadOnlyProduction = H.Property<bool>(c => c
            .Set(e => !e.ProductionMode)
            .On(e => e.ProductionMode)
            .Update()
        );

        public bool ProductionMode => _productionMode.Get();
        private readonly IProperty<bool> _productionMode = H.Property<bool>(c => c
            .Set(e =>
            {
                if (e.Locker == null) return false;
                if (e.Workflow == null) return false;
                return e.Locker.IsActive
                       && e.Workflow.CurrentStage == SampleWorkflow.Production
                       && e.Acl.IsGranted(AnalysisRights.AnalysisCertificateCreate);
            })
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentStage)
            .Update()
        );

        private readonly Func<Sample, SampleSampleTestListViewModel> _getTests;
        private readonly Func<Sample, SampleFormsListViewModel> _getForms;
        private readonly Func<int, SampleAuditTrailViewModel> _getAudit;

        public SampleSampleTestListViewModel Tests => _tests.Get();
        private readonly IProperty<SampleSampleTestListViewModel> _tests = H.Property<SampleSampleTestListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e =>
           {
               var tests = e._getTests?.Invoke(e.Model);
               if (tests != null) tests.List.CollectionChanged += e.List_CollectionChanged;
               return tests;
           })
            .On(e => e.Model)
            .Update()
        );

        private void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is IEnumerable<SampleTest> tests)
                UpdateConformity(tests);

        }

        private ITrigger _ = H.Trigger(c => c
            .NotNull(e => e.Tests)
            .Do(e => e.UpdateConformity(e.Tests.List))
            .On(e => e.Tests)
            .Update()
        );

        public void UpdateConformity(IEnumerable<SampleTest> tests)
        {
            var conformity = ConformityState.NotChecked;

            foreach (var sampleTest in tests)
            {
                conformity = UpdateConformity(conformity, sampleTest.Result?.ConformityId ?? ConformityState.NotChecked);
            }

            if (Model.ConformityId != conformity)
            {
                Model.ConformityId = conformity;
                Data.UpdateAsync(Model, "ConformityId");
            }
        }

        public string ConformityIconPath => _conformityIconPath.Get();
        private readonly IProperty<string> _conformityIconPath = H.Property<string>(c => c
            .Set(e => e.Model.ConformityId.IconPath())
                .On(e => e.Model.ConformityId)
                .Update()
        );

        private static ConformityState UpdateConformity(ConformityState currentState, ConformityState testState)
        {
            switch (testState)
            {
                case ConformityState.NotChecked:
                    return currentState switch
                    {
                        ConformityState.NotChecked => ConformityState.NotChecked,
                        ConformityState.Running => ConformityState.Running,
                        ConformityState.NotConform => ConformityState.NotConform,
                        ConformityState.Conform => ConformityState.Running,
                        ConformityState.Invalid => ConformityState.Invalid,
                        _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
                    };
                case ConformityState.Running:
                    return currentState switch
                    {
                        ConformityState.NotChecked => ConformityState.Running,
                        ConformityState.Running => ConformityState.Running,
                        ConformityState.Conform => ConformityState.Running,
                        ConformityState.NotConform => ConformityState.NotConform,
                        ConformityState.Invalid => ConformityState.Invalid,
                        _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
                    };
                case ConformityState.NotConform:
                    return currentState switch
                    {
                        ConformityState.NotChecked => ConformityState.NotConform,
                        ConformityState.Running => ConformityState.NotConform,
                        ConformityState.Conform => ConformityState.NotConform,
                        ConformityState.NotConform => ConformityState.NotConform,
                        ConformityState.Invalid => ConformityState.NotConform,
                        _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
                    };
                case ConformityState.Conform:
                    return currentState switch
                    {
                        ConformityState.NotChecked => ConformityState.Running,
                        ConformityState.Running => ConformityState.Running,
                        ConformityState.Conform => ConformityState.Conform,
                        ConformityState.NotConform => ConformityState.NotConform,
                        ConformityState.Invalid => ConformityState.Invalid,
                        _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
                    };
                case ConformityState.Invalid:
                    return currentState switch
                    {
                        ConformityState.NotChecked => ConformityState.Invalid,
                        ConformityState.Running => ConformityState.Invalid,
                        ConformityState.Conform => ConformityState.Invalid,
                        ConformityState.Invalid => ConformityState.Invalid,
                        ConformityState.NotConform => ConformityState.NotConform,
                        _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public SampleFormsListViewModel Forms => _forms.Get();
        private readonly IProperty<SampleFormsListViewModel> _forms = H.Property<SampleFormsListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e._getForms?.Invoke(e.Model))
            .On(e => e.Model)
            .Update()
        );


        public ObservableCollection<string> CommercialNames { get; } = new();
        public ObservableCollection<string> Origins { get; } = new();
        public ObservableCollection<string> PrimaryPackagingList { get; } = new();
        public ObservableCollection<string> SecondaryPackagingList { get; } = new();
        public ObservableCollection<string> StorageConditionsList { get; } = new();


        private readonly ITrigger _1 = H.Trigger(c => c
            .On(e => e.Model.Customer)
            .On(e => e.Locker.IsActive)
            .Do(async e => await e.GetOrigins())
        );
        private readonly ITrigger _2 = H.Trigger(c => c
            .On(e => e.Model.Product)
            .On(e => e.Locker.IsActive)
            .Do(async e => await e.UpdateProductLists())
        );

        private async Task GetOrigins()
        {
            if (!Locker.IsActive) return;
            if (Model.Stage != SampleWorkflow.Reception) return;

            var list = await Data.SelectDistinctAsync<Sample, string>(s => s.CustomerId == Model.CustomerId /*&& !string.IsNullOrWhiteSpace(s.SamplingOrigin)*/,
                s => s.SamplingOrigin).ToListAsync();
            Origins.Clear();

            foreach (var s in list.Where(e => !string.IsNullOrWhiteSpace(e)).OrderBy(e => e))
            {
                Origins.Add(s);
            }
        }

        private async Task UpdateProductLists()
        {
            if (!Locker.IsActive) return;
            if (Model.Stage != SampleWorkflow.Reception) return;

            var commercialNamesList = await Data.SelectDistinctAsync<Sample, string>(s => s.ProductId == Model.ProductId /*&& !string.IsNullOrWhiteSpace(s.SamplingOrigin)*/,
                s => s.CommercialName).ToListAsync();
            CommercialNames.Clear();

            foreach (var s in commercialNamesList.Where(e => !string.IsNullOrWhiteSpace(e)).OrderBy(e => e))
            {
                CommercialNames.Add(s);
            }

            var primaryList = await Data.SelectDistinctAsync<Sample, string>(s => s.ProductId == Model.ProductId /*&& !string.IsNullOrWhiteSpace(s.SamplingOrigin)*/,
                s => s.PrimaryPackaging).ToListAsync();
            PrimaryPackagingList.Clear();

            foreach (var s in primaryList.Where(e => !string.IsNullOrWhiteSpace(e)).OrderBy(e => e))
            {
                PrimaryPackagingList.Add(s);
            }

            var secondaryList = await Data.SelectDistinctAsync<Sample, string>(s => s.ProductId == Model.ProductId /*&& !string.IsNullOrWhiteSpace(s.SamplingOrigin)*/,
                s => s.SecondaryPackaging).ToListAsync();
            SecondaryPackagingList.Clear();

            foreach (var s in secondaryList.Where(e => !string.IsNullOrWhiteSpace(e)).OrderBy(e => e))
            {
                SecondaryPackagingList.Add(s);
            }

            var storageList = await Data.SelectDistinctAsync<Sample, string>(s => s.ProductId == Model.ProductId /*&& !string.IsNullOrWhiteSpace(s.SamplingOrigin)*/,
                s => s.StorageConditions).ToListAsync();
            StorageConditionsList.Clear();

            foreach (var s in storageList.Where(e => !string.IsNullOrWhiteSpace(e)).OrderBy(e => e))
            {
                StorageConditionsList.Add(s);
            }
        }

        public SampleWorkflow Workflow => _workflow.Get();
        private readonly IProperty<SampleWorkflow> _workflow = H.Property<SampleWorkflow>(c => c
            .NotNull(e => e.Model)
            .NotNull(e => e.Locker)
            .Set(vm => vm._getSampleWorkflow?.Invoke(vm.Model, vm.Locker))
            .On(e => e.Model)
            .On(e => e.Locker)
            .Update()
        );


        public ICommand CertificateCommand { get; } = H.Command(c => c
            //.CanExecute(e => e._acl.IsGranted(AnalysisRights.AnalysisCertificateCreate))
            .Action(e =>
            {
                var preview = !(e.Acl.IsGranted(AnalysisRights.AnalysisCertificateCreate)
                                &&
                                (e.Model.Stage == SampleWorkflow.Closed || e.Model.Stage == SampleWorkflow.Certificate));

                e.PrintCertificate("FR", preview);
            }).CheckCanExecute()
        );
        public ICommand PreviewCertificateCommand { get; } = H.Command(c => c
            .Action(e =>
            {
                e.PrintCertificate("FR", true);
            })
        );


        private void PrintCertificate(String language, bool preview = false)
        {
            if (Model.Id == -1)
                return;

            var template = Data.FetchOne<Xaml>(e => e.Name == "Certificate");

            // Prépare l'impression
            var ip = new Print("Certificate", template.Page, language);

            var expiry = "";
            if (Model.ExpirationDate != null)
            {
                expiry = Model.ExpirationDate?.ToString(!Model.ExpirationDayValid ? "MM/yyyy" : "dd/MM/yyyy");
            }
            ip["ExpirationDate"] = expiry;

            var manufacturingDate = "";
            if (Model.ManufacturingDate != null)
            {
                manufacturingDate = Model.ManufacturingDate?.ToString(!Model.ManufacturingDayValid ? "MM/yyyy" : "dd/MM/yyyy");
            }
            ip["ManufacturingDate"] = manufacturingDate;


            if (Model.Validator != null)
            {
                ip["Validator.Caption"] = $"DR {Model.Validator.Caption}";
                ip["Validator.Function"] = Model.Validator.Function;
            }
            else
            {
                ip["Validator.Caption"] = $"Analyse non validée";
                ip["Validator.Function"] = "";
            }

            ip.SetData(Model);

            // Cache le bandeau d'aperçu
            if (!preview)
                ip.Cache("Apercu");

            // Ajout des tests sur la page
            var nomTest = "";

            bool? conform = true;

            var startDate = DateTime.MaxValue;
            var endDate = DateTime.MinValue;


            foreach (var test in Tests.List)
            {
                if (test.Stage != SampleTestWorkflow.InvalidatedResults)
                {
                    var testStartDate = test.StartDate ?? test.Result?.Start ?? DateTime.MaxValue;
                    var testEndDate = test.EndDate ?? test.Result?.End ?? DateTime.MinValue;

                    if (testStartDate > testEndDate) testStartDate = testEndDate;
                    if (testEndDate < testStartDate) testEndDate = testStartDate;

                    if (testStartDate < startDate) startDate = testStartDate;
                    if (testEndDate > endDate) endDate = testEndDate;

                    // Ajoute la ligne pour le nom du test
                    //if (test.TestName != nomTest)
                    //{
                    //    nomTest = test.TestName;
                    //    ip.AjouteElement("Titre");
                    //    ip.Element["Titre"] = " " + nomTest;
                    //}

                    // Les résultats du test
                    ip.AjouteElement();

                    // si même nom de test ne pas repeter
                    if(nomTest == test.TestName)
                    {
                        ip.Element.ReplaceZone("Titre","");
                    }
                    else
                    {
                        nomTest = test.TestName;
                        ip.Element["Titre"] = " " + nomTest;
                    }

                    ip.Element["Date"] = testEndDate == DateTime.MinValue
                        ? "__/ __ /_____"
                        : language is "US" or "EN"
                            ? testEndDate.ToString("MM/dd/yyyy")
                            : testEndDate.ToString("dd/MM/yyyy");

                    ip.Element["Description"] = test.Description + Environment.NewLine;
                    ip.Element["Reference"] = test.Pharmacopoeia?.Abbreviation ?? "" + " " + test.PharmacopoeiaVersion + Environment.NewLine;
                    ip.Element["Specification"] = test.Specification + Environment.NewLine;
                    ip.Element["Result"] = test.Result?.Result ?? "" + Environment.NewLine;

                    switch (test.Result?.ConformityId)
                    {
                        case ConformityState.NotConform:
                            ip.Element["Conform"] = "{FR=Non conforme}{EN=Not conform}" + Environment.NewLine;
                            conform = false;
                            break;
                        case ConformityState.Conform:
                            ip.Element["Conform"] = "{FR=Conforme}{EN=Conform}" + Environment.NewLine;
                            break;
                        case ConformityState.Invalid:
                            ip.Element["Conform"] = "{FR=Invalide}{EN=Invalid}" + Environment.NewLine;
                            conform = null;
                            break;
                        case ConformityState.NotChecked:
                            ip.Element["Conform"] = "{FR=Non testé}{EN=Not tested}" + Environment.NewLine;
                            conform = null;
                            break;
                        case ConformityState.Running:
                            ip.Element["Conform"] = "{FR=En cours}{EN=Running}" + Environment.NewLine;
                            conform = null;
                            break;
                        case null:
                            ip.Element["Conform"] = "{FR=Non validé}{EN=Not validated}" + Environment.NewLine;
                            conform = null;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            ip["Conformity"] = (conform == true) ? "Conforme" : (conform == false ? "Non conforme" : "Indeterminée");
            ip["XConform"] = (conform == true) ? "X" : " ";
            ip["XNotConform"] = (conform == false) ? "X" : " ";

            ip["AnalysisStart"] = DateToString(language, startDate);
            ip["AnalysisEnd"] = DateToString(language, endDate);

            if (string.IsNullOrWhiteSpace(Model.Conclusion))
            {
                ip.ReplaceZone("Conclusion","");
            }

            // Impression du certificat d'analyse
            if (ip.Apercu("Rapport_" + Model.Reference, null, Print.Langue("{FR=Rapport d'analyse}{EN=Report of analysis} ", language) + Model.Reference))
            {
                // Log cette impression
                // TODO : Sql.Log(TypeObjet.Echantillon, IdEchantillon, ip.LogText);
            }

        }

        private static string DateToString(string language, DateTime date)
        {
            return (language == "EN" || language == "US") ? date.ToString("MM/dd/yyyy") : date.ToString("dd/MM/yyyy");
        }

    }


}
