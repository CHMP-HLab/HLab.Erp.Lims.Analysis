using System.Linq;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;
using HLab.Notify.PropertyChanged;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class SampleWorkflow : Workflow<SampleWorkflow,Sample>
    {
        public SampleWorkflow(Sample sample, DataLocker<Sample> locker):base(sample,locker)
        {
            int id = sample.Id;
            //sample.SampleTests.UpdateAsync();
            SampleTests.AddFilter(() => e => e.SampleId == id);
                
            var task = SampleTests.UpdateAsync();
            SetState(sample.Stage);

            task.GetAwaiter().OnCompleted(Update);
        }

        [Import]
        private ObservableQuery<SampleTest> SampleTests;

        protected override string StateName
        {
            get => Target.Stage; 
            set => Target.Stage = value;
        }

        private IProperty<bool> _ = H.Property<bool>(c => c
                .On(e => e.Target.Stage)
                .Do(e => { e.SetState(e.Target.Stage); }));


        //########################################################
        // RECEPTION

        public static State Reception = State.CreateDefault(c => c
            .Caption("{Reception entry}").Icon("Icons/Sample/PackageOpened").SubIcon("Icons/Validations/Open")
            .SetState(() => Reception)
        );

        public static Action SignReception = Action.Create(c => c
            .Caption("{Sign}").Icon("Icons/Validations/Sign")
            .FromState(() => Reception)
            .ToState(() => ReceptionSigned)
            .NeedRight(()=>AnalysisRights.AnalysisReceptionSign)
        );

        public static State ReceptionSigned = State.Create(c => c
            .Caption("{Reception check}").Icon("Icons/Sample/PackageOpened").SubIcon("Icons/Validations/Sign")
            .NotWhen(w => w.Target.CustomerId == null)
            .WithMessage(w => "{Missing} : {Customer}")

            .NotWhen(w => w.Target.ProductId == null)
            .WithMessage(w => "{Missing} : {Product}")

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.Batch))
            .WithMessage(w => "{Missing} : {Batch No}")

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.Reference))
            .WithMessage(w => "{Missing} : {Reference}")

            .NotWhen(w => w.Target.ReceptionDate == null)
            .WithMessage(w => "{Missing} : {Reception date}")

            .NotWhen(w => w.Target.ReceivedQuantity == null)
            .WithMessage(w => "{Missing} : {Received quantity}")

            .SetState(() => ReceptionSigned)
        );


        public static Action ValidateReception = Action.Create(c => c
            .Caption(w => "{Validate}").Icon(w => "icons/workflow/ReceptionChecked")
            .FromState(() => ReceptionSigned)
            .ToState(() => Monograph)
            .NeedRight(()=>AnalysisRights.AnalysisReceptionValidate)
        );

        //########################################################
        // MONOGRAPH

        public static State Monograph = State.Create(c => c
            .Caption(w => "{Monograph Entry}").Icon(w => "Icons/Sample/PackageOpened")
//            .WhenStateAllowed(() => ReceptionSigned)
            .SetState(() => Monograph)
        );

        public static Action ValidateMonograph = Action.Create(c => c
           .Caption(w => "{Validate monograph}").Icon(w => "icons/workflow/MonographEdit")
           .NeedRight(()=>AnalysisRights.AnalysisReceptionValidate)
           .FromState(() => Monograph)
           .ToState(() => MonographClosed)
            );

        public static State MonographClosed = State.Create(c => c
            .Caption(w => "{Monograph validated}").Icon(w => "icons/workflow/MonographCheck")
            .SetState(() => MonographClosed)

            .NotWhen(w => w.Target.PharmacopoeiaId == null)
                .WithMessage(w => "{Missing} : {Pharmacopoeia}")

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.PharmacopoeiaVersion))
                .WithMessage(w => "{Missing} : {Pharmacopoeia version}")

            .NotWhen(w => w.SampleTests.Count == 0)
                .WithMessage(w => "{Missing} : {Tests}")

            .NotWhen(w => {
                foreach (SampleTest test in w.SampleTests)
                    if (test.Stage == SampleTestWorkflow.Specifications.Name) return true; // TODO
                return false;
            }).WithMessage(w => "{Missing} : {Test specifications}")
        );

        public static Action ValidatePlanning = Action.Create(c => c
           .Caption(w => "{Schedule}").Icon(w => "icons/workflow/Planning")
           .FromState(() => MonographClosed)
           .ToState(() => Planning)
           .NeedRight(()=>AnalysisRights.AnalysisSchedule)
        );

        //########################################################
        // PLANNING

        public static State Planning = State.Create(c => c
            .Caption(w => "{Scheduling}").Icon(w => "icons/workflow/PlanningEdit")
            .WhenStateAllowed(() => MonographClosed)
            .SetState(() => Planning)
        );

        //########################################################
        // PRODUCTION
        public static Action ValidateProduction = Action.Create(c => c
           .Caption(w => "{Put into production}").Icon(w => "icons/workflow/Production")
           .FromState(() => Planning)
           .ToState(() => Production)
           .NeedRight(()=>AnalysisRights.AnalysisSchedule)
        );

        public static State Production = State.Create(c => c
            .Caption(w => "{Production}").Icon(w => "icons/results/Running")
            .SetState(()=>Production)
            .WhenStateAllowed(() => Planning)
        );

        //########################################################
        // CERTIFICAT

        public static Action ValidateCertificate = Action.Create( c => c
            .Caption(w => "{Print Certificate}").Icon(w => "icons/workflow/Certificate")
            .FromState(() => Planning)
            .ToState(() => Production)
            .NeedRight(()=>AnalysisRights.AnalysisSchedule)
        );

        public static State Certificate = State.Create(c => c
            .Caption(w => "{Edit certificate}").Icon(w => "icons/workflow/Certificate")
            .SetState(()=>Certificate)
            .WhenStateAllowed(() => Planning)
        );

        //########################################################
        // CLOSE

        public static Action Close = Action.Create( c => c
            .Caption(w => "{Close}").Icon(w => "icons/workflow/Close")
            .FromState(() => Certificate)
            .ToState(() => Closed)
            .NeedPharmacist()
        );

        public static State Closed = State.Create(c => c
            .Caption(w => "{Closed}").Icon(w => "icons/workflow/Closed")
            .SetState(() => Closed)
            .When(w => w.Target.Invoiced).WithMessage(w => "{Not billed}")
            .When(w => w.Target.Paid).WithMessage(w => "{Not Payed}")
            .WhenStateAllowed(() => Certificate)
        );
    }
}
