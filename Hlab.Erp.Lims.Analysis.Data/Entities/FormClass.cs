﻿using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data.Entities
{
    using H = H<FormClass>;

    public class FormClass : Entity, IListableModel, IEntityWithIcon, IFormClass
    {
        public FormClass() => H.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>();
        public byte[] Code
        {
            get => _code.Get();
            set => _code.Set(value);
        }

        readonly IProperty<byte[]> _code = H.Property<byte[]>();
        public string Class
        {
            get => _class.Get();
            set => _class.Set(value);
        }

        readonly IProperty<string> _class = H.Property<string>();
        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }

        readonly IProperty<string> _iconPath = H.Property<string>();
        public string Version
        {
            get => _version.Get();
            set => _version.Set(value);
        }

        readonly IProperty<string> _version = H.Property<string>();

        public byte[] CodeHash
        {
            get => _codeHash.Get();
            set => _codeHash.Set(value);
        }

        readonly IProperty<byte[]> _codeHash = H.Property<byte[]>();

        [Ignore]
        public string Caption => _caption.Get();

        readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => string.IsNullOrWhiteSpace(e.Name)?"{New form class}":$"{{Form class}}\n{e.Name}")
        );

    }
}
