using System;
using System.Collections.Generic;
using System.Text;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    using H = H<FormViewModel>;

    class FormViewModel: EntityViewModel<Form>
    {
        public FormViewModel() => H.Initialize(this);

        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c
        .Set(e => e.Model?.IconPath )
        .On(e => e.Model.IconPath)
        .Update()
        );

        //public string SubTitle => _subTitle.Get();
        //private readonly IProperty<string> _subTitle = H.Property<string>(c => c
        //.On(e => e.Model.Dose)
        //.On(e => e.Model.Form.Name)
        //.Set(e => e.getSubTitle ));
        //private string getSubTitle => Model?.Dose + "\n" + Model?.Form?.Name;


        public override string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
        .Set(e => e.getIconPath )
        .On(e => e.Model.IconPath).Update()
        );

        private string getIconPath => Model.IconPath??base.IconPath;

    }
    class FormViewModelDesign : FormViewModel, IViewModelDesign
    {
        public new Form Model { get; } = Form.DesignModel;

    }
}
