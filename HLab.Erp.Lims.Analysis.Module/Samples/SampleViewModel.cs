using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.Products;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;
using Outils;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{


    using H = H<SampleViewModel>;

    public class SampleViewModel : EntityViewModel<Sample>
    {


        public Type ListProductType => typeof(ProductsListPopupViewModel);

        [Import] public IErpServices Erp { get; }

        public SampleViewModel()
        {
            H.Initialize(this);
        }

        [Import] public SampleViewModel(Func<int, ListSampleTestViewModel> getTests, ObservableQuery<Packaging> packagings, Func<int, ListSampleFormViewModel> getForms, Func<ListFormClassViewModel> getFormClasses)
        {
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
            .Set(e =>
            {
                if (e.Locker == null) return false;
                if(e.Workflow==null) return false;

                return e.Locker.IsActive
                       && e.Workflow.CurrentState == SampleWorkflow.Reception
                       && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisReceptionSign);
            })
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentState)
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
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
                       && e.Workflow.CurrentState == SampleWorkflow.Monograph
                       && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisMonographSign);
            })
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentState)
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Update()
        );

        
        private readonly Func<int, ListSampleTestViewModel> _getTests;
        private readonly Func<int, ListSampleFormViewModel> _getForms;
        private readonly Func<ListFormClassViewModel> _getFormClasses;

        public ListSampleTestViewModel Tests => _tests.Get();
        private readonly IProperty<ListSampleTestViewModel> _tests = H.Property<ListSampleTestViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getTests(e.Model.Id))
        );
        public ListSampleFormViewModel Forms => _forms.Get();
        private readonly IProperty<ListSampleFormViewModel> _forms = H.Property<ListSampleFormViewModel>(c => c
            .On(e => e.Model)
            .Set(e =>
            {
                var f = e._getForms(e.Model.Id);
                return f;

            })
        );
        public ListFormClassViewModel FormClasses => _formClasses.Get();
        private readonly IProperty<ListFormClassViewModel> _formClasses = H.Property<ListFormClassViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getFormClasses())
        );

        public ICommand AddTestCommand { get; } = H.Command(c => c
            .CanExecute(e => e.CanExecuteAddTest())
            .Action((e,t) => e.AddTest(t as TestClass))
            .On(e => e.Model.Stage).CheckCanExecute()
        );
        public ICommand AddFormCommand { get; } = H.Command(c => c
            .CanExecute(e => e.CanExecuteAddForm())
            .Action((e,t) => e.AddForm(t as FormClass))
            .On(e => e.Model.Stage).CheckCanExecute()
        );
        public ICommand AddOneFormCommand { get; } = H.Command(c => c
            .CanExecute(e => e.CanExecuteAddForm())
            .Action((e,t) => e.AddForms())
            .On(e => e.Model.Stage).CheckCanExecute()
        );

        private bool CanExecuteAddTest()
        {
            if(!Erp.Acl.IsGranted(AnalysisRights.AnalysisAddTest)) return false;
            return Workflow?.CurrentState == SampleWorkflow.Monograph;
        }
        private bool CanExecuteAddForm()
        {
            if(!Erp.Acl.IsGranted(AnalysisRights.AnalysisReceptionSign)) return false;
            return Workflow?.CurrentState == SampleWorkflow.Reception;
        }

        public List<string> Origins => _origins.Get();

        private readonly IProperty<List<string>> _origins = H.Property<List<string>>(c => c
            .On(e => e.Model.Customer)
            .Set(e => e.GetOrigins())
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
                st.Stage = SampleTestWorkflow.DefaultState.Name;
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

            ip["NumeroEchantillon"] = Model.Reference;
            ip["Dci"] = Model.Product?.Inn;
            ip["Nom"] = Model.CommercialName;
            ip["Dosage"] = Model.Product?.Dose;
            ip["Forme"] = Model.Product?.Form.Name;

            String expiry = "";
            if (Model.ExpirationDate != null)
            {
                if (!Model.ExpirationDayValid)
                    expiry = Model.ExpirationDate?.ToString("");
                else
                    expiry = Model.ExpirationDate?.ToString("dd/MM/yyyy");
            }
            ip["DatePeremption"] = expiry;

            String dateFabrication = "";
            if (Model.ManufacturingDate != null)
            {
                    if (!Model.ManufacturingDayValid)
                        dateFabrication = Model.ManufacturingDate?.ToString("");
                    else
                        dateFabrication = Model.ManufacturingDate?.ToString("dd/MM/yyyy");
            }
            ip["DateFabrication"] = dateFabrication;


            ip["Lot"] = Model.Batch;
            ip["Demandeur"] = JoinNotNull("\n", Model.Customer?.Name, Model.Customer?.Address, Model.Customer?.Country?.Name);
            ip["Fabricant"] = JoinNotNull("\n", Model.Manufacturer?.Name, Model.Manufacturer?.Address, Model.Manufacturer?.Country?.Name);
            ip["DateReception"] = Model.ReceptionDate?.ToString("dd/MM/yyyy");
            ip["DateNotification"] = apercu ? "Aperçu" : Model.NotificationDate?.ToString("dd/MM/yyyy");
            ip["Pharmacopee"] = Model.Pharmacopoeia?.NameFr??"" + " " + Model.PharmacopoeiaVersion;
            ip["Prelevement"] = Model.SamplingOrigin;
            ip["Taille"] = Model.ReceivedQuantity.ToString();// + " " + slProduit.String("Forme").ToLower());
            ip["ConditionnementPrimaire"] = Model.PrimaryPackaging;// + (TB_ConditionnementSecondaire.Text.Length>0 ? " (" + TB_ConditionnementSecondaire.Text + ")":""));
            ip["ConditionnementSecondaire"] = Model.SecondaryPackaging;
            ip["Produit"] = Model.Aspect + "\r\n" + Model.Size;
            ip["Conclusion"] = Model.Conclusion;
            ip["Validateur"] = apercu ? "Aperçu" : Model.Validator; //MainWindow._UtilisateurPrenom + " " + MainWindow._UtilisateurNom + "\r\n" + MainWindow._UtilisateurFonction);

            // Cache le bandeau d'aperçu
            if (!apercu)
                ip.Cache("Apercu");

            // Ajout des tests sur la page
            String nomTest = "";
            foreach (var test in Tests.List)
            {
                if (test.Stage != SampleTestWorkflow.InvalidatedResults.Name)
                {
                    // Ajoute la ligne pour le nom du test
                    if (test.TestName != nomTest)
                    {
                        nomTest = test.TestName;
                        ip.AjouteElement("Titre");
                        ip.Element["Titre"] = " " + nomTest;
                    }

                    // Les résultats du test
                    ip.AjouteElement("Test");

                    if (test.EndDate==null || test.EndDate == DateTime.MinValue)
                        ip.Element["Date"] = "__/ __ /_____";
                    else if (langue == "US")
                        ip.Element["Date"] = string.Format(CultureInfo.InvariantCulture, "{0:d}", test.EndDate) + Environment.NewLine;
                    else
                        ip.Element["Date"] = string.Format("{0:d}", test.EndDate) + Environment.NewLine;

                    ip.Element["Description"] = test.Description + Environment.NewLine;
                    //TODO ip.Element["Reference"] = test.Reference + Environment.NewLine;
                    ip.Element["Norme"] = test.Specification + Environment.NewLine;
                    ip.Element["Resultat"] = test.Result?.Result??"" + Environment.NewLine;
                    ip.Element["Conforme"] = test.Result?.Conformity??"" + Environment.NewLine;
                }
            }
            // Impression du certificat d'analyse
            String numero = Model.Reference;
            if(ip.Apercu("Certificat_" + numero, null, Print.Langue("{FR=Certificat d'analyse}{US=Certificate of analysis} ", langue) + Model.Reference))
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
