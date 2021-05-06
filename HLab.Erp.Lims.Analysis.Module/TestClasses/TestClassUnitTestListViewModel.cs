using System.Collections.Generic;
using System.Collections.ObjectModel;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    [Export(typeof(IEntityListViewModel<TestClassUnitTest>))]
    public class TestClassUnitTestListViewModel : EntityListViewModel<TestClassUnitTest>, IMvvmContextProvider
    {
        private readonly ObservableCollection<int> _failedTests = new();
        private readonly Dictionary<int, string> _errors = new();

        private readonly ObservableCollection<int> _passedTests = new();



        public void AddError(int idx, string error)
        {
            if (_errors.ContainsKey(idx))
            {
                _errors.Remove(idx);
                _failedTests.Remove(idx);
            }
            if (_passedTests.Contains(idx)) _passedTests.Remove(idx);
            _errors.Add(idx,error);
            _failedTests.Add(idx);
        }

        public void AddPassed(int idx)
        {
            if (_errors.ContainsKey(idx))
            {
                _errors.Remove(idx);
                _failedTests.Remove(idx);
            }
            if (_passedTests.Contains(idx)) return;
            _passedTests.Add(idx);
        }

        public TestClassUnitTestListViewModel(TestClass testClass) : base(c =>
        {
            var list = c.Target as TestClassUnitTestListViewModel;

            return c
                //.DeleteAllowed()
                //.AddAllowed()
                .Column()
                .Header("{Name}").Content(s => s.Name).Width(200)
                .Column()
                .Id("error").Header("{Error}").Content(s => list._failedTests.Contains(s.Id) ? list._errors[s.Id] : "OK").Width(150)
                            .Icon(s => list._failedTests.Contains(s.Id) ? "Icons/Conformity/CheckFailed" : list._passedTests.Contains(s.Id) ? "Icons/Conformity/CheckPassed" : "Icons/Conformity/Invalid");
        }
        )
        {
            _failedTests.CollectionChanged += FailedTests_CollectionChanged;
            _passedTests.CollectionChanged += FailedTests_CollectionChanged;

            AddAllowed=true;
            DeleteAllowed=true;

            List.AddFilter(() => u => u.TestClassId == testClass.Id && Id>=0);
        }

        protected override bool CanExecuteDelete()
        {
            return Selected !=null;
        }

        private void FailedTests_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}
