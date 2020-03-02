using System;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
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
    class SampleViewModel : EntityViewModel<SampleViewModel,Sample>
    {
        public Type ListProductType => typeof(ProductsListPopupViewModel);

        [Import] public IErpServices Erp { get; }
        
        public SampleViewModel() { }
        [Import] public SampleViewModel(Func<int, ListSampleTestViewModel> getTests, ObservableQuery<Packaging> packagings)
        {
            _getTests = getTests;
            Packagings = packagings;
            Packagings.UpdateAsync();
        }
        public override string Title => _title.Get();
        private IProperty<string> _title = H.Property<string>(c => c
            .On(e => e.Model.Reference)
            .NotNull(e => e.Model)
            .Set(e => e.Model.Reference??"{New sample}")
        );
        public string SubTitle => Model.Customer?.Name??"{New sample}" + "\n" + Model.Product?.Caption + "\n" + Model.Reference;
        private IProperty<string> _subTitle = H.Property<string>(c => c
            .On(e => e.Model.Customer.Name)
            .On(e => e.Model.Product.Caption)
            .NotNull(e => e.Model)
            .Set(e => e.Model.Customer?.Name??"{Customer}" + "\n" + e.Model.Product?.Caption??"{Product}")
        );
        public ObservableQuery<Packaging> Packagings { get; }

        [TriggerOn(nameof(Packagings),"Item","Secondary")]
        public IObservableFilter<Packaging> PrimaryPackagingList { get; }
            = H.Filter<Packaging>((e, f) => f
                .AddFilter(p => !p.Secondary)
                .Link(() => e.Packagings));

        [TriggerOn(nameof(Packagings),"Item","Secondary")]
        public IObservableFilter<Packaging> SecondaryPackagingList { get; }
            = H.Filter<Packaging>((e, f) => f
                .AddFilter(p => p.Secondary)
                .Link(() => e.Packagings)
            );

        public bool IsReadOnly => _isReadOnly.Get();
        private readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
            .On(e => e.EditMode)
            .Set(e => !e.EditMode)
        );
        public bool EditMode => _editMode.Get();
        private readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentState)
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Set(e => 
                e.Locker.IsActive 
                && e.Workflow.CurrentState == SampleWorkflow.Reception
                && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisReceptionSign)
                )
        );

        public bool IsReadOnlyMonograph => _isReadOnlyMonograph.Get();
        private readonly IProperty<bool> _isReadOnlyMonograph = H.Property<bool>(c => c
            .On(e => e.MonographMode)
            .Set(e => !e.MonographMode)
        );
        public bool MonographMode => _monographMode.Get();
        private readonly IProperty<bool> _monographMode = H.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentState)
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Set(e => 
                e.Locker.IsActive 
                && e.Workflow.CurrentState == SampleWorkflow.Monograph
                && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisMonographSign)
                )
        );

        
        private readonly Func<int, ListSampleTestViewModel> _getTests;

        public ListSampleTestViewModel Tests => _tests.Get();
        private readonly IProperty<ListSampleTestViewModel> _tests = H.Property<ListSampleTestViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getTests(e.Model.Id))
        );

        public ICommand AddTestCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Erp.Acl.IsGranted(AnalysisRights.AnalysisAddTest))
            .Action((e,t) => e.AddTest(t as TestClass))
        );

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
                st.Stage = SampleWorkflow.DefaultState.Name;
            });

            if (test != null)
                Tests.List.UpdateAsync();
        }

        public SampleWorkflow Workflow => _workflow.Get();
        private readonly IProperty<SampleWorkflow> _workflow = H.Property<SampleWorkflow>(c => c
            .On(e => e.Model)
            .On(e => e.Locker)
            .NotNull(e => e.Model)
            .NotNull(e => e.Locker)
            .Set(vm => new SampleWorkflow(vm.Model,vm.Locker))
        );

        public ICommand CertificateCommand { get; } = H.Command(c => c
                //TODO : use current language
            .Action(e => e.PrintCertificate("FR"))
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
            ip["Dci"] = Model.Product.Inn;
            ip["Nom"] = Model.CommercialName;
            ip["Dosage"] = Model.Product.Dose;
            ip["Forme"] = Model.Product.Form.Name;

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
                if (test.Validation != 2)
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
                    ip.Element["Resultat"] = test.Result + Environment.NewLine;
                    ip.Element["Conforme"] = test.Conform + Environment.NewLine;
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

    class SampleViewModelDesign : SampleViewModel, IViewModelDesign
    {
        public new Sample Model { get; } = Sample.DesignModel;
    }
}
