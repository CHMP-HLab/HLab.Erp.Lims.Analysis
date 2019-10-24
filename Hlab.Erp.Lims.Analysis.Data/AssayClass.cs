using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.IO.Compression;
using System.Text;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    public partial class AssayClass : Entity<AssayClass>, ILocalCache
        , IEntityWithIcon
        , IEntityWithColor
    {
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));


        public string Version
        {
            get => _version.Get();
            set => _version.Set(value);
        }

        readonly IProperty<string> _version = H.Property<string>(c => c.Default(""));

        [Ignore]
        public string Xaml
        {
            get => _xaml.Get();
            set => _xaml.Set(value);
        }

        private readonly IProperty<string> _xaml = H.Property<string>(c => c.Default(""));

        [Ignore]
        public string CodeBehind
        {
            get => _codeBehind.Get();
            set => _codeBehind.Set(value);
        }

        private readonly IProperty<string> _codeBehind = H.Property<string>(c => c.Default(""));




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

        readonly IProperty<int?> _order = H.Property<int?>();


        public sbyte? Category
        {
            get => _category.Get();
            set => _category.Set(value);
        }

        readonly IProperty<sbyte?> _category = H.Property<sbyte?>();

        [Ignore]
        public virtual ICollection<SampleAssay> SampleAssays { get; set; }

        public int? DurationFirst
        {
            get => _durationFirst.Get();
            set => _durationFirst.Set(value);
        }

        readonly IProperty<int?> _durationFirst = H.Property<int?>();

        public int? DurationNext
        {
            get => _durationNext.Get();
            set => _durationNext.Set(value);
        }

        readonly IProperty<int?> _durationNext = H.Property<int?>();

        [System.ComponentModel.DataAnnotations.Schema.Column("DureeAdmin")]
        public int? DurationAdmin
        {
            get => _durationAdmin.Get();
            set => _durationAdmin.Set(value);
        }

        readonly IProperty<int?> _durationAdmin = H.Property<int?>();


        //[Column]
        //public int? Color
        //{
        //    get => N.Get(() => (int?)null); set => N.Set(value);
        //}
        [Ignore]
        public int? Color => 0;

        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }

        readonly IProperty<string> _iconPath = H.Property<string>();

        public static AssayClass DesignModel => new AssayClass
        {
            Name = "Identification",IconPath = "",Version="1.1.0"
        };
    }
}
