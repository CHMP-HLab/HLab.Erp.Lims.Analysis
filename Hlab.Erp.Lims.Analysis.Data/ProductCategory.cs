﻿using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<ProductCategory>;

    public class ProductCategory : Entity, ILocalCache, IListableModel
    {
        public static TestCategory DesignModel => new TestCategory
        {
            Name = "Design Category",
            Priority = 1,
            IconPath = "Icons/Default"
        };

        public ProductCategory() => H.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));
        public int? Priority
        {
            get => _priority.Get();
            set => _priority.Set(value);
        }
        private readonly IProperty<int?> _priority = H.Property<int?>();
        [Ignore]
        public string Caption => Name;
        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c.Default(""));
    }

}