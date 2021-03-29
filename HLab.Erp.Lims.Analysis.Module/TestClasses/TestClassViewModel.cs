﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    using H = H<TestClassViewModel>;

    public class TestClassViewModel : EntityViewModel<TestClass>
    {
        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c.Bind(e => e.Model.Name));

        public override string IconPath => Model.IconPath;

        [Import] public TestClassViewModel(IDataService data, Func<TestClass, TestClassUnitTestListViewModel> getUnitTests, FormHelper formHelper)
        {
            _data = data;
            _getUnitTests = getUnitTests;
            FormHelper = formHelper;

            H.Initialize(this);
        }
        private ITrigger _init = H.Trigger(c => c.On(e => e.Model).Do(async (e, f) =>
        {
            if (e.Model.Code != null)
            {
                await e.FormHelper.ExtractCode(e.Model.Code).ConfigureAwait(true);
            }
            else
            {
                e.FormHelper.Xaml = "<Grid></Grid>";
                e.FormHelper.Cs = "public class Test\n{\n}";
            }

            await e.FormHelper.Compile();
        }));

        #region Imports

        private readonly IDataService _data;
        private readonly Func<TestClass, TestClassUnitTestListViewModel> _getUnitTests;
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
        private readonly IProperty<string> _newName = H.Property<string>();

        /// <summary>
        /// List of unit tests
        /// </summary>
        public TestClassUnitTestListViewModel UnitTests => _unitTests.Get();
        private readonly IProperty<TestClassUnitTestListViewModel> _unitTests = H.Property<TestClassUnitTestListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e.SetUnitTests())
            .On(e => e.Model)
            .Update()
        );
        private TestClassUnitTestListViewModel SetUnitTests()
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
            .CanExecute(e => e.Locker.IsActive && e.Locker.Persister.IsDirty)
            .Action(async e => await e.TryAsync())
                .On(e => e.Locker.IsActive)
                .On(e => e.Locker.Persister.IsDirty)
            .CheckCanExecute()
        );

        private async Task TryAsync()
        {
            await FormHelper.Compile();
            Model.Code = await FormHelper.SaveCodeAsync();
        }

        /// <summary>
        /// Try to recompile the form
        /// </summary>
        public ICommand SpecificationModeCommand { get; } = H.Command(c => c.Action(
            e => e.FormHelper.Mode = FormMode.Specification
        ));
        public ICommand CaptureModeCommand { get; } = H.Command(c => c.Action(
            e => e.FormHelper.Mode = FormMode.Capture
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
        private async Task AddUnitTestAsync()
        {
            await _data.AddAsync<TestClassUnitTest>(u =>
            {
                u.TestClass = Model;
                u.Name = NewName;

                u.TestName = FormHelper.Form.Target.TestName;
                u.Description = FormHelper.Form.Target.Description;
                u.Specification = FormHelper.Form.Target.Specification;
//                u.SpecificationsDone = FormHelper.Form.Test.;
                u.SpecificationValues = FormHelper.GetSpecPackedValues();
                u.ResultValues = FormHelper.GetPackedValues();

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
        private async Task CheckUnitTestsAsync()
        {
            foreach (var t in UnitTests.List.ToList())
            {
                var u = t.Clone<TestClassUnitTestClone>();
                await  LoadResultAsync(u).ConfigureAwait(true);
                u.SpecificationValues = FormHelper.GetSpecPackedValues();
                u.ResultValues = FormHelper.GetPackedValues();

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
            FormHelper.Mode = FormMode.Specification;
        }


        #endregion
    }
}
