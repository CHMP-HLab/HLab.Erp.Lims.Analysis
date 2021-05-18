using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Workflows;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductWorkflow : Workflow<ProductWorkflow,Product>
    {
        public ProductWorkflow(Product product,IDataLocker locker):base(product,locker)
        {
            CurrentStage = Created;
            Update();
        }

        public static Stage Created = Stage.Create(c => c
            .Caption("^Reception entry").Icon("Icons/Sample/PackageOpened")
            .SetState(() => Created)
        );

        protected override Stage TargetStage { get; set; }
    }
}