using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Wpf.Entities;
using HLab.Erp.Base.Wpf.Entities.Customers;
using HLab.Erp.Core;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    using H = NotifyHelper<ProductsDataModule>;

    public class ProductsDataModule : ErpDataModule<ProductsDataModule,ProductsListViewModel>
    {

    }
}
