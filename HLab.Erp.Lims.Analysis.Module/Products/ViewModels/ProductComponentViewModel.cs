﻿using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using System;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels;

using H = H<ProductComponentViewModel>;

internal class ProductComponentViewModel: ListableEntityViewModel<ProductComponent>
{

    //public string SubTitle => _subTitle.Get();
    //private readonly IProperty<string> _subTitle = H.Property<string>(c => c
    //    .Set(e => e.GetSubTitle )
    //    .On(e => e.Model.Variant)
    //    .On(e => e.Model.Form.Name)
    //    .Update()
    //);
    //private string GetSubTitle => $"{Model?.Variant}\n{Model?.Form?.Name}";


    public override string IconPath => "";//_iconPath.Get();
    //private readonly IProperty<string> _iconPath = H.Property<string>(c => c
    //.Set(e => e.GetIconPath )
    //.On(e => e.Model.Form.IconPath)
    //.On(e => e.Model.IconPath)
    //.Update()
    //);

    //private string GetIconPath => Model?.Form?.IconPath??Model?.IconPath??base.IconPath;

    public ProductComponentViewModel(Injector i):base(i)
    {
        H.Initialize(this);
    }



    //public ProductWorkflow Workflow => _workflow.Get();
    //private readonly IProperty<ProductWorkflow> _workflow = H.Property<ProductWorkflow>(c => c
    //    .On(e => e.Model)
    //    .OnNotNull(e => e.Locker)
    //    .Set(vm => new ProductWorkflow(vm.Model,vm.Locker))
    //);
}

internal class ProductComponentViewModelDesign : ProductComponentViewModel, IViewModelDesign
{
    public ProductComponentViewModelDesign() : base(null)
    {
    }

    public new ProductComponent Model { get; } = ProductComponent.DesignModel;

}