using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Workflows;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductWorkflow : Workflow<ProductWorkflow,Product>
    {
        public ProductWorkflow(Product product):base(product)
        {
            H.Initialize(this);

            CurrentState = Created;
            Update();
        }

        public static State Created = State.Create(c => c
            .Caption("^Reception entry").Icon("Icons/Sample/PackageOpened")
            .SetState(() => Created)
        );

    }
}