using System;
using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Data;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
     public partial class AssayResult : Entity<AssayResult>
//        , IEntityWithIcon
//        , IEntityWithColor
    {
        //public int? Color => _color.Get();
        //private readonly IProperty<int?> _color = H.Property<int?>(c => c.OneWayBind(e => e.SampleAssay.AssayClass.Color));

        //public string IconName => _iconName.Get();
        //private readonly IProperty<string> _iconName = H.Property<string>(c => c.OneWayBind(e => e.SampleAssay.AssayClass.IconName));

        public int? SampleAssayId
        {
            get => _sampleAssayId.Get();
            set => _sampleAssayId.Set(value);
        }

        private readonly IProperty<int?> _sampleAssayId = H.Property<int?>();

        [Ignore]
        public virtual SampleAssay SampleAssay
        {
            get => _sampleAssay.Get();
            set => SampleAssayId = value?.Id;
        }

        private readonly IProperty<SampleAssay> _sampleAssay = H.Property<SampleAssay>(c => c.Foreign(e => e.SampleAssayId));

        public int? UserId
        {
            get => _userId.Get();
            set => _userId.Set(value);
        }

        private readonly IProperty<int?> _userId = H.Property<int?>();

        public string Values
        {
            get => _values.Get();
            set => _values.Set(value);
        }

        private readonly IProperty<string> _values = H.Property<string>(c => c.Default(""));

        public string Result
        {
            get => _result.Get(); set => _result.Set(value);
        }

        readonly IProperty<string> _result = H.Property<string>(c => c.Default(""));

        public DateTime? Start
        {
            get => _start.Get(); 
            set => _start.Set(value);
        }
        readonly IProperty<DateTime?> _start = H.Property<DateTime?>();

        public DateTime? End
        {
            get => _end.Get(); 
            set => _end.Set(value);
        }
        readonly IProperty<DateTime?> _end = H.Property<DateTime?>();

        public int? StateId
        {
            get => _stateId.Get();
            set => _stateId.Set(value);
        }
        private readonly IProperty<int?> _stateId = H.Property<int?>();

    }
}
