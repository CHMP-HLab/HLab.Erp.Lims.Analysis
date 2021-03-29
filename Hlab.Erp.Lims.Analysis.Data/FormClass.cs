﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
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
        private readonly IProperty<string> _name = H.Property<string>();
        public byte[] Code
        {
            get => _code.Get();
            set => _code.Set(value);
        }
        private readonly IProperty<byte[]> _code = H.Property<byte[]>();
        public string Class
        {
            get => _class.Get();
            set => _class.Set(value);
        }
        private readonly IProperty<string> _class = H.Property<string>();
        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }
        private readonly IProperty<string> _iconPath = H.Property<string>();
        public string Version
        {
            get => _version.Get();
            set => _version.Set(value);
        }
        private readonly IProperty<string> _version = H.Property<string>();

        [Ignore]
        public string Caption => Name;

    }
}
