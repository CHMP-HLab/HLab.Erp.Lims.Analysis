using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    using H = H<FormClassViewModel>;

    public class FormClassViewModel : EntityViewModel<FormClass>, IFormHelperProvider
    {
        public override string Header => _header.Get();
        private readonly IProperty<string> _header = H.Property<string>(c => c.Bind(e => e.Model.Name));

        public override string IconPath => Model.IconPath;

        public FormClassViewModel(FormHelper formHelper)
        {
            FormHelper = formHelper;
            H.Initialize(this);
        }
        private ITrigger _init = H.Trigger(c => c
            .On(e => e.Model)
            .Do(async (e, f) =>
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
                    using Outils;
                    using System.Linq;
                    using System.Collections.Generic;
                    namespace Lims
                    {
                        public class Form
                        {
                            public void Process(object sender, RoutedEventArgs e)
                            {
                            }      
                        }
                    }";
                }

                await e.FormHelper.CompileAsync();
            }));
        public FormHelper FormHelper { get; }

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

        private async Task TryAsync()
        {
            await FormHelper.CompileAsync();
            Model.Code = await FormHelper.PackCodeAsync();
        }

        /// <summary>
        /// Try to recompile the form
        /// </summary>
        public ICommand SpecificationModeCommand { get; } = H.Command(c => c.Action(
            e => e.FormHelper.Form.Mode = FormMode.Specification
        ));
        public ICommand CaptureModeCommand { get; } = H.Command(c => c.Action(
             e => e.FormHelper.Form.Mode = FormMode.Capture
        ));

        #endregion


    }
}
