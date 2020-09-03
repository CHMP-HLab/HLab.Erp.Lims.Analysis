using HLab.Erp.Data;

namespace HLab.Erp.Lims.Analysis.Data
{
    public abstract class EntityAClTarget<T> : Entity
        where T : EntityAClTarget<T>
    {
        public string AclTargetId => nameof(T) + "_" + Id;

    }
}