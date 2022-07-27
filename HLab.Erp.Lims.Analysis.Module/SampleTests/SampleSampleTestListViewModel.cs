using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Mvvm.Annotations;
using System;
using System.Windows.Input;
using HLab.Notify.PropertyChanged;
using System.Threading.Tasks;
using HLab.Erp.Lims.Analysis.Data.Entities;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    using H = H<SampleSampleTestListViewModel>;
    public class SampleSampleTestListViewModel : Core.EntityLists.EntityListViewModel<SampleTest>, IMvvmContextProvider
    {
        public Sample Sample {get;}
        public SampleSampleTestListViewModel(Injector i, Sample sample) : base(i, c => c
                //.DeleteAllowed()
                .StaticFilter(e => e.SampleId == sample.Id)

                .DescriptionColumn(s => s.TestName, s => s.Description,"Test")
                    .Header("{Test}")//.Mvvm<IDescriptionViewClass>()
                .Width(300)
                .Icon(s => s.IconPath)
                .OrderBy(s => s.Order).UpdateOn(s => s.TestName).UpdateOn(s => s.Description)

                .DescriptionColumn(s => "", s => s.Specification,"Specification")
                    .Header("{Specifications}")
                .Width(200)
                .OrderBy(s => s.Specification).UpdateOn(s => s.Specification)

                .DescriptionColumn(s => "", s => s.Result?.Result ?? "","Result")

                    .Header("{Result}").Width(200).OrderBy(s => s.Result?.Result ?? "").UpdateOn(s => s.Result.Result)

                .DescriptionColumn(s => "", s => s.Result?.Conformity ?? "","Conformity")

                    .Header("{Conformity}").Width(200).OrderBy(s => s.Result?.Conformity ?? "").UpdateOn(s => s.Result.Conformity)


                .ConformityColumnPostLinked(s => s.Result != null ? s.Result.ConformityId : ConformityState.NotChecked).UpdateOn(s => s.Result.ConformityId)

                .StageColumn(default(SampleTestWorkflow), s => s.StageId)

                .Column("IsValid").Hidden().Content(s => s.Stage != SampleTestWorkflow.InvalidatedResults)
                .Column("Group").Hidden().Content(s => s.TestClassId)

        )
        {
            var n = SampleTestWorkflow.Specifications; // TODO : this is a hack to force top level static constructor

            Sample = sample;

            H.Initialize(this);
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
        }

        public bool EditMode { get=> _editMode.Get(); set=>_editMode.Set(value); }
        IProperty<bool> _editMode = H.Property<bool>(c => c.Default(false));

        protected override bool CanExecuteDelete(SampleTest sampleTest, Action<string> errorAction)
        {
            if(!EditMode) return false;
            if (sampleTest == null) return false;
            var stage =  sampleTest.Stage.IsAny( errorAction, SampleTestWorkflow.Specifications);
            var granted = Injected.Erp.Acl.IsGranted(errorAction, AnalysisRights.AnalysisAddTest);
            return stage && granted;
        }

        public override Type AddArgumentClass => typeof(TestClass);

        readonly ITrigger _1 = H.Trigger(c => c
            .On(e => e.Sample.Stage)
            .On(e => e.Sample.Pharmacopoeia)
            .On(e => e.Sample.PharmacopoeiaVersion)
            .On(e => e.EditMode)
            .Do(e => (e.AddCommand as CommandPropertyHolder)?.CheckCanExecute())
        );

        //private readonly ITrigger _triggerConformity = H.Trigger(c => c
        //    .On(e => e.List.Item().Result.ConformityId)
        //    .On(e => e.List.Item().Result.Stage)
        //    .On(e => e.List.Item().Result.Start)
        //    .On(e => e.List.Item().Result.End)
        //    .Do(e => e.List.Refresh())
        //);

        protected override bool CanExecuteAdd(Action<string> errorAction)
        {
            if(!EditMode) return false;
            if (!Injected.Erp.Acl.IsGranted(errorAction, AnalysisRights.AnalysisAddTest)) return false;
            if (Sample.Pharmacopoeia == null)
            {
                errorAction("{Missing} : {Pharmacopoeia}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(Sample.PharmacopoeiaVersion))
            {
                errorAction("{Missing} : {Pharmacopoeia version}");
                return false;
            }
            if (! Sample.Stage.IsAny(errorAction,SampleWorkflow.Monograph))
            {
                errorAction("{requier stage} : {Monograph}");
                return false;
            }
            return true;
        }

        protected override async Task AddEntityAsync(object arg)
        {
            if (!(arg is TestClass testClass)) return;

            var test = await Injected.Erp.Data.AddAsync<SampleTest>(st =>
            {
                st.Sample = Sample;
                st.TestClass = testClass;
                st.Pharmacopoeia = Sample.Pharmacopoeia;
                st.PharmacopoeiaVersion = Sample.PharmacopoeiaVersion;
                //st.Code = testClass.Code;
                st.Description = "";
                st.TestName = testClass.Name;
                st.Stage = SampleTestWorkflow.DefaultStage;
            });

            if (test != null) List.Update();
        }


        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}