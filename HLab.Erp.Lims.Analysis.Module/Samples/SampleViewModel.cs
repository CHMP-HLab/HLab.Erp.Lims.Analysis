using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using HLab.Erp.Acl;
using HLab.Erp.Base.Wpf;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
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

        public IErpServices Erp { get; }
        private readonly Func<Sample,IDataLocker<Sample>,SampleWorkflow> _getSampleWorkflow;

        internal SampleViewModel()
        {
            H.Initialize(this);
        }

        public SampleViewModel(
            IErpServices erp, 
            Func<Sample,SampleSampleTestListViewModel> getTests, 
            ObservableQuery<Packaging> packagings, 
            Func<Sample,SampleFormsListViewModel> getForms, 
            Func<FormClassesListViewModel> getFormClasses, 
            Func<int, SampleAuditTrailViewModel> getAudit, 
            Func<Sample, IDataLocker<Sample>, SampleWorkflow> getSampleWorkflow
            )
        {
            _getAudit = getAudit;
            Erp = erp;
            _getSampleWorkflow = getSampleWorkflow;
            _getTests = getTests;
            H.Initialize(this);
            Packagings = packagings;
            _getForms = getForms;
            _getFormClasses = getFormClasses;
            Packagings.Update();
        }

        public override object Header => _header.Get();
        private readonly IProperty<object> _header = H.Property<object>(c => c
            .Set(e => (object)e.Model?.Reference??"{New sample}")
            .On(e => e.Model.Reference)
            .NotNull(e => e.Model)
            .Update()
        );
        public string SubTitle => _subTitle.Get();
        private readonly IProperty<string> _subTitle = H.Property<string>(c => c
            .Set(e => e.Model?.Customer?.Name??"{Customer}" + "\n" + e.Model?.Product?.Caption??"{Product}")
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

        public Visibility CustomerVisibility => _customerVisibility.Get();
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
        
        private readonly Func<Sample,SampleSampleTestListViewModel> _getTests;
        private readonly Func<Sample, SampleFormsListViewModel> _getForms;
        private readonly Func<FormClassesListViewModel> _getFormClasses;
        private readonly Func<int,SampleAuditTrailViewModel> _getAudit;

        public SampleSampleTestListViewModel Tests => _tests.Get();
        private readonly IProperty<SampleSampleTestListViewModel> _tests = H.Property<SampleSampleTestListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set( e =>
            {
                var tests =  e._getTests(e.Model);
                tests.List.CollectionChanged += e.List_CollectionChanged;
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
            .Do( e =>  e.UpdateConformity(e.Tests.List))
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
                Erp.Data.UpdateAsync(Model, "ConformityId");
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
            .Set(e => e._getForms(e.Model))
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

        public FormClassesListViewModel FormClasses => _formClasses.Get();
        private readonly IProperty<FormClassesListViewModel> _formClasses = H.Property<FormClassesListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e._getFormClasses())
            .On(e => e.Model)
            .Update()
        );

        public ICommand AddTestCommand { get; } = H.Command(c => c
            .CanExecute(e => e.CanExecuteAddTest())
            .Action(async (e,t) => await e.AddTestAsync(t as TestClass))
            .On(e => e.Model.Stage)
            .CheckCanExecute()
        );

        public ICommand AddFormCommand { get; } = H.Command(c => c
            .CanExecute(e => e.CanExecuteAddForm())
            .Action(async (e,t) =>
            {
                if (t is not FormClass formClass) return;

                if (e.Forms.List.Any(sf => sf.FormClass.Id == formClass.Id)) return;

                try
                {
                    await e.AddFormAsync(formClass);
                }
                catch(DataException)
                { }
            })
            .On(e => e.Model.Stage)
            .On(e => e.Model.Id)
            .CheckCanExecute()
        );

        public ICommand AddOneFormCommand { get; } = H.Command(c => c
            .CanExecute(e => e.CanExecuteAddForm())
            .Action(async (e,t) => await e.AddFormsAsync())
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
            if (Model.Id < 0) return false; 
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

        private async Task AddTestAsync(TestClass testClass)
        {
            if (testClass == null) return;

            var test = await Erp.Data.AddAsync<SampleTest>(st =>
            {
                st.Sample = Model;
                st.TestClass = testClass;
                //st.Code = testClass.Code;
                st.Description = "";
                st.TestName = testClass.Name;
                st.Stage = SampleTestWorkflow.DefaultStage;
            });

            if (test != null) Tests.List.Update();
        
        }

        private async Task AddFormsAsync()
        {
            Forms.List.UpdateAsync();
            FormClasses.List.UpdateAsync();
            foreach (var formClass in FormClasses.List)
            {
                if(!Forms.List.Any(e => ReferenceEquals(e.FormClass,formClass)))
                    await AddFormAsync(formClass);
            }
        }

        private async Task AddFormAsync(FormClass formClass)
        {
            if (formClass == null) return;

            var form = await Erp.Data.AddAsync<SampleForm>(st =>
            {
                st.Sample = Model;
                st.FormClass = formClass;
            });

            if (form != null)
                 Forms.List.Update();
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


        public ICommand CertificateCommand { get; } = H.Command(c => c
            //.CanExecute(e => e._acl.IsGranted(AnalysisRights.AnalysisCertificateCreate))
            .Action(e =>
            {
                var preview = !(e.Erp.Acl.IsGranted(AnalysisRights.AnalysisCertificateCreate)
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

            var template = Erp.Data.FetchOne<Xaml>(e => e.Name == "Certificate");

            // Prépare l'impression
            var ip = new Print("Certificate",template.Page, language);


            //ip["Reference"] = Model.Reference;
            //ip["Product.Inn"] = Model.Product?.Inn;
            //ip["CommercialName"] = Model.CommercialName;
            //ip["Product.Dose"] = Model.Product?.Dose;
            //ip["Product.Form"] = Model.Product?.Form.Name;
            //ip["ReportReference"] = Model.ReportReference;
            //ip["CustomerReference"] = Model.CustomerReference;
            //ip["AnalysisMotivation.Name"] = Model.AnalysisMotivation.Name;

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


            //ip["BatchNo"] = Model.Batch;
            //ip["Customer.Name"] = Model.Customer?.Name;
            //ip["Customer.Address"] = JoinNotNull("\n", Model.Customer?.Address, Model.Customer?.Country?.Name);
            //ip["Customer.Email"] = Model.Customer?.Email;
            //ip["Customer.Phone"] = Model.Customer?.Phone;

            //ip["Manufacturer.Name"] = JoinNotNull("\n", Model.Manufacturer?.Name, Model.Manufacturer?.Address, Model.Manufacturer?.Country?.Name);
            //ip["Manufacturer.Address"] = JoinNotNull("\n", Model.Manufacturer?.Address, Model.Manufacturer?.Country?.Name);

            //ip["ReceptionDate"] = Model.ReceptionDate?.ToString("dd/MM/yyyy");
            //ip["NotificationDate"] = apercu ? "Aperçu" : Model.NotificationDate?.ToString("dd/MM/yyyy");
            //ip["Pharmacopoeia"] = Model.Pharmacopoeia?.NameFr??"" + " " + Model.PharmacopoeiaVersion;
            //ip["PrelevementDate"] = Model.SamplingOrigin;
            //ip["ReceivedQuantity"] = Model.ReceivedQuantity.ToString();// + " " + slProduit.String("Forme").ToLower());
            //ip["PrimaryPackaging"] = Model.PrimaryPackaging;// + (TB_ConditionnementSecondaire.Text.Length>0 ? " (" + TB_ConditionnementSecondaire.Text + ")":""));
            //ip["SecondaryPackaging"] = Model.SecondaryPackaging;
            //ip["Product.Name"] = Model.Aspect + "\r\n" + Model.Size;


            //ip["Conclusion"] = Model.Conclusion;

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
                    else if (language == "US" || language == "EN")
                        ip.Element["Date"] = test.EndDate?.ToString("MM/dd/yyyy") + Environment.NewLine;
                    else
                        ip.Element["Date"] = test.EndDate?.ToString("dd/MM/yyyy") + Environment.NewLine;

                    ip.Element["Description"] = test.Description + Environment.NewLine;
                    ip.Element["Reference"] = test.Pharmacopoeia?.Abbreviation??"" +" "+ test.PharmacopoeiaVersion + Environment.NewLine;
                    ip.Element["Specification"] = test.Specification + Environment.NewLine;
                    ip.Element["Result"] = test.Result?.Result??"" + Environment.NewLine;

                    switch (test.Result?.ConformityId)
                    {
                        case ConformityState.NotConform : ip.Element["Conform"] = "{FR=Non conforme}{EN=Not conform}" + Environment.NewLine;
                            conform = false;
                            break;
                        case ConformityState.Conform : ip.Element["Conform"] = "{FR=Conforme}{EN=Conform}" + Environment.NewLine;
                            break;
                        case ConformityState.Invalid : ip.Element["Conform"] = "{FR=Invalide}{EN=Invalid}" + Environment.NewLine;
                            conform = null;
                            break;
                        case ConformityState.NotChecked: ip.Element["Conform"] = "{FR=Non testé}{EN=Not tested}" + Environment.NewLine;
                            conform = null;
                            break;
                        case ConformityState.Running: ip.Element["Conform"] = "{FR=En cours}{EN=Running}" + Environment.NewLine;
                            conform = null;
                            break;
                        case null: ip.Element["Conform"] = "{FR=Non validé}{EN=Not validated}" + Environment.NewLine;
                            conform = null;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    //ip.Element["Conforme"] = test.Result?.Conformity??"" + Environment.NewLine;
                }
            }

            ip["Conformity"] = (conform == true)?"Conforme": (conform==false?"Non conforme":"Indeterminée");
            ip["XConform"] = (conform == true)?"X":" ";
            ip["XNotConform"] = (conform == false)?"X":" ";

            ip["AnalysisStart"] = DateToString(language,startDate);
            ip["AnalysisEnd"] = DateToString(language,endDate);

            // Impression du certificat d'analyse
            if(ip.Apercu("Rapport_" + Model.Reference, null, Print.Langue("{FR=Rapport d'analyse}{EN=Report of analysis} ", language) + Model.Reference))
            {
                // Log cette impression
                // TODO : Sql.Log(TypeObjet.Echantillon, IdEchantillon, ip.LogText);
            }

        }
        private string JoinNotNull(string separator, params string[] values) => String.Join(separator, values.Where(s => !String.IsNullOrWhiteSpace(s)));

        private static string DateToString(string language, DateTime date)
        {
            return (language == "EN" || language == "US") ? date.ToString("MM/dd/yyyy") : date.ToString("dd/MM/yyyy");
        }

    }


    public class SampleViewModelDesign : SampleViewModel, IViewModelDesign
    {
        public new Sample Model { get; } = Sample.DesignModel;
    }
}
