using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    using H = H<FormClassViewModel>;

    public class FormClassViewModelDesign : FormClassViewModel, IViewModelDesign
    {
        public FormClassViewModelDesign():base(null)
        {
//            Model = FormClass.DesignModel;
            FormHelper.Xaml = "<xml></xml>";
            FormHelper.Cs = "using HLab.Erp.Acl;\nusing HLab.Erp.Lims.Analysis.Data;\nusing HLab.Mvvm.Annotations;";
        }
    }

    class DummyTarget : NotifierBase, IFormTarget
    {
        public DummyTarget() => H<DummyTarget>.Initialize(this);

        public string Result { get; set; }

        public ConformityState ConformityId
        {
            get =>_conformityId.Get(); 
            set =>_conformityId.Set(value); 
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        private readonly IProperty<ConformityState> _conformityId = H<DummyTarget>.Property<ConformityState>();

        public byte[] Code => null;

        public string SpecificationValues
        {
            get => _specificationValues.Get(); 
            set => _specificationValues.Set(value);
        }
        private readonly IProperty<string> _specificationValues = H<DummyTarget>.Property<string>();

        public bool SpecificationDone
        {
            get => _specificationDone.Get(); 
            set => _specificationDone.Set(value);
        }
        private readonly IProperty<bool> _specificationDone = H<DummyTarget>.Property<bool>();

        public string ResultValues
        {
            get => _resultValues.Get(); 
            set => _resultValues.Set(value);
        }
        private readonly IProperty<string> _resultValues = H<DummyTarget>.Property<string>();

        public bool MandatoryDone
        {
            get => _mandatoryDone.Get(); 
            set => _mandatoryDone.Set(value);
        }
        private readonly IProperty<bool> _mandatoryDone = H<DummyTarget>.Property<bool>();


        public string DefaultTestName => "Dummy";
        public string TestName { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        public string Conformity { get; set; }
        IFormClass IFormTarget.FormClass { get => null; set => throw new System.NotImplementedException(); }
        string IFormTarget.Name { get => "Dummy"; set => throw new System.NotImplementedException(); }
    }

    public class FormClassViewModel : EntityViewModel<FormClass>, IFormHelperProvider
    {
        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c.Bind(e => e.Model.Name));

        public override string IconPath => Model.IconPath;

        [Import] public FormClassViewModel(FormHelper formHelper)
        {
            FormHelper = formHelper;
            H.Initialize(this);
        }

        public FormHelper FormHelper { get; }

        public ICommand TryCommand { get; } = H.Command(c => c.Action(
            async e => await e.FormHelper.Compile()
        ));
        public ICommand SpecificationModeCommand { get; } = H.Command(c => c.Action(
            e => e.FormHelper.Form.Mode = FormMode.Specification
        ));
        public ICommand CaptureModeCommand { get; } = H.Command(c => c.Action(
             e => e.FormHelper.Form.Mode = FormMode.Capture
        ));


        private ITrigger _init = H.Trigger(c => c
            .On(e => e.Model)
            .Do(async (e, f) =>
            {
                if (e.Model.Code != null)
                {
                    await e.FormHelper.LoadCodeAsync(e.Model.Code).ConfigureAwait(true);
                }
                else
                {
                    e.FormHelper.Xaml = @"
                    <Grid>
                    </Grid>";
                    e.FormHelper.Cs = @"
                    using System;
                    using System.Windows;
                    using System.Windows.Controls;
                    using Outils;
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

                await e.FormHelper.Compile();
            }));

    }
}
