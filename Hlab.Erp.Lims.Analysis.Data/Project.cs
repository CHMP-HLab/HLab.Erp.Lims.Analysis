using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<Project>;
    public class Project : Entity
    {
        public Project() => H.Initialize(this);

        public int? ParentId
        {
            get => _parentId.Get();
            set => _parentId.Set(value);
        }
        private readonly IProperty<int?> _parentId = H.Property<int?>();


        public Project Parent
        {
            get => _parent.Get();
            set => _parent.Set(value);
        }
        private readonly IProperty<Project> _parent = H.Property<Project>(c => c.Foreign(e => e.ParentId));
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>();

    }
}
