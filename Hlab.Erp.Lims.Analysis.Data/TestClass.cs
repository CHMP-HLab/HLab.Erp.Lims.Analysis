using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.IO.Compression;
using System.Text;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<TestClass>;

    public partial class TestClass : Entity, ILocalCache, IListableModel, IFormClass
        , IEntityWithIcon
        , IEntityWithColor
    {
        public TestClass() => H.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));


        public string Version
        {
            get => _version.Get();
            set => _version.Set(value);
        }
        private readonly IProperty<string> _version = H.Property<string>(c => c.Default(""));


        public byte[] Code
        {
            get => _code.Get();
            set => _code.Set(value);
        }
        private readonly IProperty<byte[]> _code = H.Property<byte[]>();


        public int? Order
        {
            get => _order.Get();
            set => _order.Set(value);
        }
        private readonly IProperty<int?> _order = H.Property<int?>();


        public int? CategoryId
        {
            get => _category.Id.Get();
            set => _category.Id.Set(value);
        }
        [Ignore]
        public TestCategory Category
        {
            get => _category.Get();
            set => _category.Set(value);
        }
        private readonly IForeign<TestCategory> _category = H.Foreign<TestCategory>();

        [Ignore]
        public virtual ICollection<SampleTest> SampleTests { get; set; }

        public int? DurationFirst
        {
            get => _durationFirst.Get();
            set => _durationFirst.Set(value);
        }

        private readonly IProperty<int?> _durationFirst = H.Property<int?>();

        public int? DurationNext
        {
            get => _durationNext.Get();
            set => _durationNext.Set(value);
        }

        private readonly IProperty<int?> _durationNext = H.Property<int?>();

        public int? DurationAdmin
        {
            get => _durationAdmin.Get();
            set => _durationAdmin.Set(value);
        }

        private readonly IProperty<int?> _durationAdmin = H.Property<int?>();


        //[Column]
        //public int? Color
        //{
        //    get => N.Get(() => (int?)null); set => N.Set(value);
        //}
        [Ignore]
        public int? Color => 0;

        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c.Bind(e => e.Name));

        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }

        private readonly IProperty<string> _iconPath = H.Property<string>();

        public static TestClass DesignModel => new TestClass
        {
            Name = "Identification",IconPath = "",Version="1.1.0"
        };
    }
}
