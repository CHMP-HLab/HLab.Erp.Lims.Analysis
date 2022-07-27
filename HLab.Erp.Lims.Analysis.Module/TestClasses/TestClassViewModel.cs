using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    using H = H<TestClassViewModel>;

    public class TestClassViewModel : ListableEntityViewModel<TestClass>, IFormHelperProvider
    {
        public override string Header => _header.Get();

        readonly IProperty<string> _header = H.Property<string>(c => c
            .Set(e => $"{{Test class}}\n{e.Model.Caption}")
            .On(e => e.Model.Caption)
            .Update()
        );

        public TestClassViewModel(Injector i, Func<TestClass, TestClassUnitTestListViewModel> getUnitTests, FormHelper formHelper):base(i)
        {
            _getUnitTests = getUnitTests;
            FormHelper = formHelper;

            H.Initialize(this);
        }

        ITrigger _init = H.Trigger(c => c.On(e => e.Model).Do(async (e, f) =>
        {
            if (e.Model.Code != null)
            {
                await e.FormHelper.ExtractCodeAsync(e.Model.Code).ConfigureAwait(true);
            }
            else
            {
                e.FormHelper.Xaml = "<Grid></Grid>";
                e.FormHelper.Cs = @"
                    using System;
                    using System.Windows;
                    using System.Windows.Controls;
                    using System.Linq;
                    using System.Collections.Generic;
                    namespace Lims
                    {
                        public class Test
                        {
                            public void Process(object sender, RoutedEventArgs e)
                            {
                            }      
                        }
                    }";
            }

            await e.FormHelper.CompileAsync();
        }));

        #region Imports

        readonly Func<TestClass, TestClassUnitTestListViewModel> _getUnitTests;
        public FormHelper FormHelper { get; }

        #endregion


        #region Properties

        /// <summary>
        /// Name to be used to create a unit test
        /// </summary>
        public string NewName
        {
            get => _newName.Get();
            set => _newName.Set(value);
        }

        readonly IProperty<string> _newName = H.Property<string>();



        /// <summary>
        /// List of unit tests
        /// </summary>
        public TestClassUnitTestListViewModel UnitTests => _unitTests.Get();

        readonly IProperty<TestClassUnitTestListViewModel> _unitTests = H.Property<TestClassUnitTestListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e.SetUnitTests())
            .On(e => e.Model)
            .Update()
        );

        TestClassUnitTestListViewModel SetUnitTests()
        {
            var vm =  _getUnitTests(Model);
            vm.SetSelectAction(async r =>
            {
                var u = (r==null)?new TestClassUnitTestClone():r.Clone<TestClassUnitTestClone>();
                await LoadResultAsync(u).ConfigureAwait(false);
                NewName = u.Name;
                if(vm.DeleteCommand is INotifyCommand n) n.CheckCanExecute();
            });

            if(vm.List.Any())
                vm.Selected = vm.List.First();

            return vm;
        }

        #endregion


        #region Commands

        /// <summary>
        /// Try to recompile the form
        /// </summary>
        public ICommand TryCommand { get; } = H.Command(c => c
            .CanExecute(e => !e.FormHelper.FormUpToDate )
            .Action(async e => await e.TryAsync())
                .On(e => e.FormHelper.FormUpToDate)
            .CheckCanExecute()
        );

        async Task TryAsync()
        {
            await FormHelper.CompileAsync();
            Model.Code = await FormHelper.PackCodeAsync();
        }

        public ICommand FormatCommand { get; } = H.Command(c => c
            //.CanExecute(e => !e.FormHelper.FormUpToDate )
            .Action(async e => await e.FormHelper.FormatAsync())
        );

        /// <summary>
        /// Try to recompile the form
        /// </summary>
        public ICommand SpecificationModeCommand { get; } = H.Command(c => c.Action(
            e => e.FormHelper.Form.Mode = FormMode.Specification
        ));
        public ICommand CaptureModeCommand { get; } = H.Command(c => c.Action(
            e => e.FormHelper.Form.Mode = FormMode.Capture
        ));

        /// <summary>
        /// Add unit test form current form state
        /// </summary>
        public ICommand AddUnitTestCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Locker.IsActive)
            .Action(async e => await e.AddUnitTestAsync())
            .On(e => e.Locker.IsActive)
            .CheckCanExecute()
        );

        async Task AddUnitTestAsync()
        {
            await Injected.Data.AddAsync<TestClassUnitTest>(u =>
            {
                u.TestClass = Model;
                u.Name = NewName;

                u.TestName = FormHelper.Form.Target.TestName;
                u.Description = FormHelper.Form.Target.Description;
                u.Specification = FormHelper.Form.Target.Specification;
//                u.SpecificationsDone = FormHelper.Form.Test.;
                u.SpecificationValues = FormHelper.Form.Target.SpecificationValues;
                u.ResultValues = FormHelper.Form.Target.ResultValues;

                u.ConformityId = FormHelper.Form.Target.ConformityId;
                u.Result = FormHelper.Form.Target.Result;
                u.Conformity = FormHelper.Form.Target.Conformity;
                u.MandatoryDone = FormHelper.Form.Target.MandatoryDone;

            });

            UnitTests.List.Update();
        }

        /// <summary>
        /// Check all unit tests
        /// </summary>
        public ICommand CheckUnitTestsCommand { get; } = H.Command(c => c
            .Action(e => e.CheckUnitTestsAsync())
        );

        async Task CheckUnitTestsAsync()
        {
            foreach (var t in UnitTests.List.ToList())
            {
                var u = t.Clone<TestClassUnitTestClone>();
                await  LoadResultAsync(u).ConfigureAwait(true);
                u.SpecificationValues = FormHelper.Form.Target.SpecificationValues;
                u.ResultValues = FormHelper.Form.Target.ResultValues;

                if(!t.Check(u, out var error)) 
                    UnitTests.AddError(t.Id,error);
                else
                    UnitTests.AddPassed(t.Id);
            }
            UnitTests.RefreshColumn("error");
        }
        public async Task LoadResultAsync(IFormTarget target=null)
        {
            await FormHelper.LoadAsync(target).ConfigureAwait(true);
            FormHelper.Form.Mode = FormMode.Specification;
        }


        #endregion
    }
}
