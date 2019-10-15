using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data
{
    public abstract class EntityAClTarget<T> : Entity<T>
    where T : EntityAClTarget<T>
    {
        public string AclTargetId => nameof(T) + "_" + Id;

    }

    public class Project : EntityAClTarget<Project>, IAclTarget
    {

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

        IAclTarget IAclTarget.Parent => Parent;
    }
}
