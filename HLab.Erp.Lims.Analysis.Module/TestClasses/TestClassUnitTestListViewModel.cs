using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTestResults;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    public class TestClassUnitTestListViewModel : EntityListViewModel<TestClassUnitTest>, IMvvmContextProvider
    {
        [Import]
        private readonly IErpServices _erp;

        private readonly TestClass _testClass;

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

        public TestClassUnitTestListViewModel(TestClass testClass)
        {
            _testClass = testClass;
            _failedTests.CollectionChanged += FailedTests_CollectionChanged;
            _passedTests.CollectionChanged += FailedTests_CollectionChanged;

            AddAllowed=true;
            DeleteAllowed=true;

            List.AddFilter(() => u => u.TestClassId == testClass.Id && Id>=0);

            Columns.Configure(c => c
                .Column.Header("{Name}").Content(s => s.Name).Width(200)
                .Column.Id("error").Header("{Error}").Content(s => _failedTests.Contains(s.Id)?_errors[s.Id]:"OK").Width(150)
                .Icon(s => _failedTests.Contains(s.Id)?"Icons/Conformity/CheckFailed":_passedTests.Contains(s.Id)?"Icons/Conformity/CheckPassed":"Icons/Conformity/Invalid")
            );

            using (List.Suspender.Get())
            {
                DeleteAllowed = true;
                AddAllowed = true;
            }

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
