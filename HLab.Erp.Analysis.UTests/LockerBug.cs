using System.Threading.Tasks;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.SampleTestResults;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using Xunit;

namespace HLab.Erp.Analysis.UTests
{
    class Acl : AclServiceBase
    {
        public override bool IsGranted(AclRight right, object grantedTo = null, object grantedOn = null)
        {
            return true;
        }
    }


    public class LockerBug
    {
        [Fact]
        public async void Test_TestEntityViewModel()
        {
            var acl = new Acl();

            SampleTestResultWorkflow GetSampleTestResultWorkflow(SampleTestResult s, IDataLocker<SampleTestResult> d) => new SampleTestResultWorkflow(s, d);
            FormHelper GetFormHelper() => new FormHelper();

            var vm = new TestEntityViewModel(GetFormHelper,GetSampleTestResultWorkflow,null);

            vm.Inject(
                null,
                r => new DataLocker<SampleTestResult>(r, null, acl, null, null, null), 
                acl, null

                );

            vm.Model = new SampleTestResult();
            vm.Workflow.CurrentStage = SampleTestResultWorkflow.Running;

            await vm.Locker.ActivateAsync();
            Assert.True(vm.EditMode);
        }


        class Test_SampleTestResult : SampleTestResult
        {
            public override SampleTest SampleTest { get; set; }
        }
        class Test_SampleTest : SampleTest
        {
            public override TestClass TestClass { get; set; }
            public override Sample Sample { get; set; }
        }
        class Test_Sample : Sample
        {
        }

        [WpfFact]
        public async Task Test_SampleTestResultViewModel()
        {
            var acl = new Acl();
            WorkflowAnalysisExtension.Acl = acl;

            SampleTestResultWorkflow GetSampleTestResultWorkflow(SampleTestResult s, IDataLocker<SampleTestResult> d) => new SampleTestResultWorkflow(s, d);
            FormHelper GetFormHelper() => new FormHelper();
            DataLocker<T> GetDataLocker<T>(T l)
                where T : class, IEntity<int>
                => new DataLocker<T>(l, null, acl, null, null, null);

            var vm = new SampleTestResultViewModel(GetFormHelper, GetSampleTestResultWorkflow, null, GetDataLocker, GetDataLocker);


            vm.Inject(
                null,
                r => new DataLocker<SampleTestResult>(r, null, acl, null, null, null), 
                acl, null
                );


            await vm.FormHelper.LoadDefaultFormAsync();
            var testClass = new TestClass{Code = await vm.FormHelper.PackCodeAsync()};
            vm.Model = new Test_SampleTestResult {
                SampleTest = new Test_SampleTest {
                    TestClass = testClass, 
                    Sample = new Test_Sample()
                    }
                };
            //vm.Model = new SampleTestResult {};
            vm.Workflow.CurrentStage = SampleTestResultWorkflow.Running;

            var t = await vm.Locker.ActivateAsync();

            Assert.True(vm.EditMode);

            vm.Workflow.CurrentStage = SampleTestResultWorkflow.Validated;
            Assert.False(vm.EditMode);
        }

        [WpfFact]
        public async Task Test_SampleTestViewModel()
        {
            var acl = new Acl();
            WorkflowAnalysisExtension.Acl = acl;

            SampleTestWorkflow GetSampleTestWorkflow(SampleTest s, IDataLocker<SampleTest> d) => new SampleTestWorkflow(s, d, null);

            FormHelper GetFormHelper() => new FormHelper();
            
            DataLocker<T> GetDataLocker<T>(T l) 
                where T : class, IEntity<int>
                => new DataLocker<T>(l, null, acl, null, null, null);

            var vm = new SampleTestViewModel(null, null, GetFormHelper, GetSampleTestWorkflow, GetDataLocker);

            vm.Inject(
                null,
                GetDataLocker, 
                acl, null

                );

            vm.Model = new SampleTest();
            await vm.FormHelper.LoadDefaultFormAsync().ConfigureAwait(true);
            vm.Workflow.CurrentStage = SampleTestWorkflow.Specifications;

            await vm.Locker.ActivateAsync();
            Assert.True(vm.EditMode);
        }

        [Fact]
        public async void Test_SampleViewModel()
        {
            var acl = new Acl();
            WorkflowAnalysisExtension.Acl = acl;


            SampleWorkflow GetSampleWorkflow(Sample s, IDataLocker<Sample> d) => new SampleWorkflow(s, d, null);

            var vm = new SampleViewModel(null, null, null, null, null,  GetSampleWorkflow);

            vm.Inject(
                null,
                r => new DataLocker<Sample>(r, null, acl, null, null, null), 
                acl, null
                );

            vm.Model = new Sample();

            vm.Workflow.CurrentStage = SampleWorkflow.Reception;

            await vm.Locker.ActivateAsync();
            Assert.True(vm.EditMode);

            vm.Workflow.CurrentStage = SampleWorkflow.Production;
            Assert.False(vm.EditMode);
        }
    }
}
