using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Wpf;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

using Outils;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    using H = H<SampleViewModel>;

    public class SampleViewModel : EntityViewModel<Sample>
    {
        public Type ListProductType => typeof(ProductsListPopupViewModel);

        [Import] public IErpServices Erp { get; }

        public SampleViewModel() => H.Initialize(this);

        [Import] public SampleViewModel(Func<int, SampleSampleTestListViewModel> getTests, ObservableQuery<Packaging> packagings, Func<int, ListSampleFormViewModel> getForms, Func<ListFormClassViewModel> getFormClasses, Func<int,SampleAuditTrailViewModel> getAudit )
        {
            _getAudit = getAudit;
            _getTests = getTests;
            H.Initialize(this);
            Packagings = packagings;
            _getForms = getForms;
            _getFormClasses = getFormClasses;
            Packagings.UpdateAsync();
        }
        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c
            .Set(e => e.Model?.Reference??"{New sample}")
            .On(e => e.Model.Reference)
            .NotNull(e => e.Model)
            .Update()
        );
        public string SubTitle => Model.Customer?.Name??"{New sample}" + "\n" + Model.Product?.Caption + "\n" + Model.Reference;
        private IProperty<string> _subTitle = H.Property<string>(c => c
            .Set(e => e.Model?.Customer?.Name??"{Customer}" + "\n" + e.Model.Product?.Caption??"{Product}")
            .On(e => e.Model.Customer.Name)
            .On(e => e.Model.Product.Caption)
            .NotNull(e => e.Model)
            .Update()
        );
        public ObservableQuery<Packaging> Packagings { get; }

        [TriggerOn(nameof(Packagings),"Item","Secondary")]
        public IObservableFilter<Packaging> PrimaryPackagingList { get; }
            = H.Filter<Packaging>(c => c
                .AddFilter(p => !p.Secondary)
                .Link(e => e.Packagings)
            );

        [TriggerOn(nameof(Packagings),"Item","Secondary")]
        public IObservableFilter<Packaging> SecondaryPackagingList { get; }
            = H.Filter<Packaging>(c => c
                .AddFilter(p => p.Secondary)
                .Link(e => e.Packagings)
            );

        public bool IsReadOnly => _isReadOnly.Get();
        private readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
            .Set(e => !e.EditMode)
            .On(e => e.EditMode)
            .Update()
        );

        public bool EditMode => _editMode.Get();
        private readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Set(e => e.Locker.IsActive
                       && e.Workflow.CurrentStage == SampleWorkflow.Reception
                       && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisReceptionSign))
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentStage)
            .Update()
        );

        public Visibility CutomerVisibility => _customerVisibility.Get();
        private readonly IProperty<Visibility> _customerVisibility = H.Property<Visibility>(c => c
            .Set(e => e.Erp.Acl.IsGranted(ErpRights.ErpViewCustomer)?Visibility.Visible:Visibility.Hidden)
            .On(e => e.Erp.Acl.Connection.User)
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
                if (e.Locker==null) return false;
                if (e.Workflow == null) return false;
                return e.Locker.IsActive
                       && e.Workflow.CurrentStage == SampleWorkflow.Monograph
                       && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisMonographSign);
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
                if (e.Locker==null) return false;
                if (e.Workflow == null) return false;
                return e.Locker.IsActive
                       && e.Workflow.CurrentStage == SampleWorkflow.Production
                       && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisCertificateCreate);
            })
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentStage)
            .Update()
        );
        
        private readonly Func<int, SampleSampleTestListViewModel> _getTests;
        private readonly Func<int, ListSampleFormViewModel> _getForms;
        private readonly Func<ListFormClassViewModel> _getFormClasses;
        private readonly Func<int, SampleAuditTrailViewModel> _getAudit;

        public SampleSampleTestListViewModel Tests => _tests.Get();
        private readonly IProperty<SampleSampleTestListViewModel> _tests = H.Property<SampleSampleTestListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e._getTests(e.Model.Id))
            .On(e => e.Model)
            .Update()
        );

        public ListSampleFormViewModel Forms => _forms.Get();
        private readonly IProperty<ListSampleFormViewModel> _forms = H.Property<ListSampleFormViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e._getForms(e.Model.Id))
            .On(e => e.Model)
            .Update()
        );

        public SampleAuditTrailViewModel AuditTrail => _auditTrail.Get();
        private readonly IProperty<SampleAuditTrailViewModel> _auditTrail = H.Property<SampleAuditTrailViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e._getAudit(e.Model.Id))
            .On(e => e.Model)
            .Update()
        );

        public ListFormClassViewModel FormClasses => _formClasses.Get();
        private readonly IProperty<ListFormClassViewModel> _formClasses = H.Property<ListFormClassViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e._getFormClasses())
            .On(e => e.Model)
            .Update()
        );

        public ICommand AddTestCommand { get; } = H.Command(c => c
            .CanExecute(e => e.CanExecuteAddTest())
            .Action((e,t) => e.AddTest(t as TestClass))
            .On(e => e.Model.Stage)
            .CheckCanExecute()
        );

        public ICommand AddFormCommand { get; } = H.Command(c => c
            .CanExecute(e => e.CanExecuteAddForm())
            .Action((e,t) => e.AddForm(t as FormClass))
            .On(e => e.Model.Stage)
            .CheckCanExecute()
        );

        public ICommand AddOneFormCommand { get; } = H.Command(c => c
            .CanExecute(e => e.CanExecuteAddForm())
            .Action((e,t) => e.AddForms())
            .On(e => e.Model.Stage).CheckCanExecute()
        );

        private bool CanExecuteAddTest()
        {
            if(!Erp.Acl.IsGranted(AnalysisRights.AnalysisAddTest)) return false;
            return Workflow?.CurrentStage == SampleWorkflow.Monograph;
        }

        private bool CanExecuteAddForm()
        {
            if(!Erp.Acl.IsGranted(AnalysisRights.AnalysisReceptionSign)) return false;
            return Workflow?.CurrentStage == SampleWorkflow.Reception;
        }

        public List<string> Origins => _origins.Get();
        private readonly IProperty<List<string>> _origins = H.Property<List<string>>(c => c
            .NotNull(e => e.Model)
            .Set(async e => await e.GetOrigins())
            .On(e => e.Model.Customer)
            .Update()
        );

        private async Task<List<string>> GetOrigins()
        {
            var list =  await Erp.Data.SelectDistinctAsync<Sample, string>(s => s.CustomerId == Model.CustomerId && !string.IsNullOrWhiteSpace(s.SamplingOrigin),
                s => s.SamplingOrigin).ToListAsync();
            return list;
        }

        private void AddTest(TestClass testClass)
        {
            if (testClass == null) return;

            var test = _data.Add<SampleTest>(st =>
            {
                st.Sample = Model;
                st.TestClass = testClass;
                st.Code = testClass.Code;
                st.Description = "";
                st.TestName = testClass.Name;
                st.Stage = SampleTestWorkflow.DefaultStage.Name;
            });

            if (test != null)
                Tests.List.UpdateAsync();
        
        }

        private void AddForms()
        {
            Forms.List.FluentUpdateAsync();
            FormClasses.List.FluentUpdateAsync();
            foreach (var formClass in FormClasses.List)
            {
                if(!Forms.List.Any(e => ReferenceEquals(e.FormClass,formClass)))
                    AddForm(formClass);
            }
        }

        private void AddForm(FormClass formClass)
        {
            if (formClass == null) return;

            var form = _data.Add<SampleForm>(st =>
            {
                st.Sample = Model;
                st.FormClass = formClass;
            });

            if (form != null)
                Forms.List.UpdateAsync();
        }

        public SampleWorkflow Workflow => _workflow.Get();
        private readonly IProperty<SampleWorkflow> _workflow = H.Property<SampleWorkflow>(c => c
            .Set(vm =>
            {
                if (vm.Model == null || vm.Locker == null) return null;
                return vm._getSampleWorkflow(vm.Model, vm.Locker);
            })
            .On(e => e.Model)
            .On(e => e.Locker)
            .NotNull(e => e.Model)
            .NotNull(e => e.Locker)
            .Update()
        );

        [Import] private Func<Sample,DataLocker<Sample>,SampleWorkflow> _getSampleWorkflow;

        [Import] private IAclService _acl;

        public ICommand CertificateCommand { get; } = H.Command(c => c
            //.CanExecute(e => e._acl.IsGranted(AnalysisRights.AnalysisCertificateCreate))
            .Action(e =>
            {
                var preview = true;
                if (e._acl.IsGranted(AnalysisRights.AnalysisCertificateCreate)
                    && 
                    (e.Model.Stage == SampleWorkflow.Closed.Name || e.Model.Stage == SampleWorkflow.Certificate.Name))
                    preview = false;
                e.PrintCertificate("FR");
            }).CheckCanExecute()
        );
        public ICommand PreviewCertificateCommand { get; } = H.Command(c => c
            .Action(e =>
            {
                e.PrintCertificate("FR", true);
            })
        );

        [Import]
        private IDataService _data;

        private void PrintCertificate(String langue, bool apercu = false)
        {
            if (Model.Id == -1)
                return;

            var template = _data.FetchOne<Xaml>(e => e.Name == "Certificate");

            // Prépare l'impression
            var ip = new Print("Certificate",template.Page, langue);

            ip["Reference"] = Model.Reference;
            ip["Product.Inn"] = Model.Product?.Inn;
            ip["CommercialName"] = Model.CommercialName;
            ip["Product.Dose"] = Model.Product?.Dose;
            ip["Product.Form"] = Model.Product?.Form.Name;
            ip["ReportReference"] = Model.ReportReference;
            ip["CustomerReference"] = Model.CustomerReference;

            String expiry = "";
            if (Model.ExpirationDate != null)
            {
                if (!Model.ExpirationDayValid)
                    expiry = Model.ExpirationDate?.ToString("MM/yyyy");
                else
                    expiry = Model.ExpirationDate?.ToString("dd/MM/yyyy");
            }
            ip["ExpirationDate"] = expiry;

            String dateFabrication = "";
            if (Model.ManufacturingDate != null)
            {
                    if (!Model.ManufacturingDayValid)
                        dateFabrication = Model.ManufacturingDate?.ToString("MM/yyyy");
                    else
                        dateFabrication = Model.ManufacturingDate?.ToString("dd/MM/yyyy");
            }
            ip["ManufacturingDate"] = dateFabrication;


            ip["BatchNo"] = Model.Batch;
            ip["Customer.Name"] = Model.Customer?.Name;
            ip["Customer.Address"] = JoinNotNull("\n", Model.Customer?.Address, Model.Customer?.Country?.Name);
            ip["Customer.Email"] = Model.Customer?.Email;
            ip["Customer.Phone"] = Model.Customer?.Phone;

            ip["Manufacturer.Name"] = JoinNotNull("\n", Model.Manufacturer?.Name, Model.Manufacturer?.Address, Model.Manufacturer?.Country?.Name);
            ip["Manufacturer.Address"] = JoinNotNull("\n", Model.Manufacturer?.Address, Model.Manufacturer?.Country?.Name);

            ip["ReceptionDate"] = Model.ReceptionDate?.ToString("dd/MM/yyyy");
            ip["NotificationDate"] = apercu ? "Aperçu" : Model.NotificationDate?.ToString("dd/MM/yyyy");
            ip["Pharmacopoeia"] = Model.Pharmacopoeia?.NameFr??"" + " " + Model.PharmacopoeiaVersion;
            ip["PrelevementDate"] = Model.SamplingOrigin;
            ip["ReceivedQuantity"] = Model.ReceivedQuantity.ToString();// + " " + slProduit.String("Forme").ToLower());
            ip["PrimaryPackaging"] = Model.PrimaryPackaging;// + (TB_ConditionnementSecondaire.Text.Length>0 ? " (" + TB_ConditionnementSecondaire.Text + ")":""));
            ip["SecondaryPackaging"] = Model.SecondaryPackaging;
            ip["Product.Name"] = Model.Aspect + "\r\n" + Model.Size;


            ip["Conclusion"] = Model.Conclusion;

            if(Model.Validator!=null)
            {
                ip["Validator.Caption"] = $"DR {Model.Validator.Caption}"; 
                ip["Validator.Function"] = Model.Validator.Function; 
            }
            else
            {
                ip["Validator.Caption"] = $"Analyse non validée";
                ip["Validator.Function"] = ""; 
            }


            // Cache le bandeau d'aperçu
            if (!apercu)
                ip.Cache("Apercu");

            // Ajout des tests sur la page
            String nomTest = "";

            bool conform = true;

            var startDate = DateTime.MaxValue;
            var endDate = DateTime.MinValue;


            foreach (var test in Tests.List)
            {
                if (test.Stage != SampleTestWorkflow.InvalidatedResults.Name)
                {
                    if ((test.StartDate??DateTime.MaxValue)<startDate) startDate = test.StartDate??DateTime.MaxValue;
                    if ((test.EndDate??DateTime.MinValue)>endDate) endDate = test.EndDate??DateTime.MinValue;

                    // Ajoute la ligne pour le nom du test
                    if (test.TestName != nomTest)
                    {
                        nomTest = test.TestName;
                        ip.AjouteElement("Titre");
                        ip.Element["Titre"] = " " + nomTest;
                    }

                    // Les résultats du test
                    ip.AjouteElement("Test");

                    if (test.EndDate == null || test.EndDate == DateTime.MinValue)
                        ip.Element["Date"] = "__/ __ /_____";
                    else if (langue == "US")
                        ip.Element["Date"] = test.EndDate?.ToString("MM/dd/yyyy") + Environment.NewLine;
                    else
                        ip.Element["Date"] = test.EndDate?.ToString("dd/MM/yyyy") + Environment.NewLine;

                    ip.Element["Description"] = test.Description + Environment.NewLine;
                    ip.Element["Reference"] = test.Pharmacopoeia?.Abbreviation??"" +" "+ test.PharmacopoeiaVersion + Environment.NewLine;
                    ip.Element["Specification"] = test.Specification + Environment.NewLine;
                    ip.Element["Result"] = test.Result?.Result??"" + Environment.NewLine;

                    switch (test.Result?.StateId)
                    {
                        case 2 : ip.Element["Conform"] = "Non Conforme" + Environment.NewLine;
                            conform = false;
                            break;
                        case 3 : ip.Element["Conform"] = "Conforme" + Environment.NewLine;
                            break;
                        default:
                            ip.Element["Conform"] = "En cours" + Environment.NewLine;
                            break;
                    }
                    //ip.Element["Conforme"] = test.Result?.Conformity??"" + Environment.NewLine;
                }
            }

            ip["AnalysisStart"] = startDate;
            ip["AnalysisEnd"] = endDate;


            if(conform)
            {
                ip["XConform"]="X"; 
                ip["XNotConform"]=" ";
            }
            else
            {
                ip["XConform"]=" "; 
                ip["XNotConform"]="X";
            }

            if(Model.Motivation=="Enregistrement")
            {
                ip["XAMM"]="X"; 
            }
            else
            {
                ip["XRoutine"]=" "; 
            }

            if(Model.Motivation=="Routine")
            {
                ip["XRoutine"]="X"; 
            }
            else
            {
                ip["XAMM"]=" "; 
            }
            // Impression du certificat d'analyse
            String numero = Model.Reference;
            if(ip.Apercu("Certificat_" + numero, null, Print.Langue("{FR=Rapport d'analyse}{US=Report of analysis} ", langue) + Model.Reference))
            {
                // Log cette impression
                // TODO : Sql.Log(TypeObjet.Echantillon, IdEchantillon, ip.LogText);
            }

        }
        private string JoinNotNull(string separator, params string[] values) => String.Join(separator, values.Where(s => !String.IsNullOrWhiteSpace(s)));


    }

    public class SampleViewModelDesign : SampleViewModel, IViewModelDesign
    {
        public new Sample Model { get; } = Sample.DesignModel;
    }
}
