﻿using System.Linq;
using System.Threading.Tasks;
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
            SampleTests.AddFilter(() => e => e.SampleId == id);
                
            var task = UpdateChildrenAsync();
            SetState(sample.Stage);
        }
        public async Task UpdateChildrenAsync()
        {
            await SampleTests.UpdateAsync();
            Update();
        }

        [Import] private ObservableQuery<SampleTest> SampleTests;

        protected override string StateName
        {
            get => Target.Stage; 
            set => Target.Stage = value;
        }

        private IProperty<bool> _ = H.Property<bool>(c => c

            .On(e => e.Target.Stage)
            .Do((a, b) =>
            {
                a.SetState(a.Target.Stage);
            })

            .On(e => e.Locker.IsActive)
            .Do(async (a, b) =>
            {
                await a.UpdateChildrenAsync();
            })
        );


        //########################################################
        // RECEPTION

        public static State Reception = State.CreateDefault(c => c
            .Caption("{Reception entry}").Icon("Icons/Sample/PackageOpened|Icons/Validations/Edit")
            .SetState(() => Reception)
        );

        public static Action SignReception = Action.Create(c => c
            .Caption("{Sign}").Icon("Icons/Validations/Sign")
            .FromState(() => Reception)
            .ToState(() => ReceptionCheck)
            .NeedRight(()=>AnalysisRights.AnalysisReceptionSign)
        );

        //########################################################
        // RECEPTION CHECK

        public static State ReceptionCheck = State.Create(c => c
            .Caption("{Reception check}").Icon("Icons/Sample/PackageOpened|Icons/Validations/Sign")

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.Batch))
            .WithMessage(w => "{Missing} : {Batch No}")
            .HighlightField(w => w.Target.Batch)

            .NotWhen(w => w.Target.ReceivedQuantity == null)
            .WithMessage(w => "{Missing} : {Received quantity}")
            .HighlightField(w => w.Target.ReceivedQuantity)

            .NotWhen(w => w.Target.ReceptionDate == null)
            .WithMessage(w => "{Missing} : {Reception date}")
            .HighlightField(w => w.Target.ReceptionDate)

            .NotWhen(w => w.Target.ProductId == null)
            .WithMessage(w => "{Missing} : {Product}")
            .HighlightField(w => w.Target.Product)

            .NotWhen(w => w.Target.CustomerId == null)
            .WithMessage(w => "{Missing} : {Customer}")
            .HighlightField(w => w.Target.Customer)

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.Reference))
            .WithMessage(w => "{Missing} : {Reference}")
            .HighlightField(w => w.Target.Reference)
        );


        public static Action CheckReception = Action.Create(c => c
            .Caption(w => "{Check}").Icon(w => "Icons/Workflows/Check")
            .FromState(() => ReceptionCheck)
            .ToState(() => Monograph)
            .NeedRight(()=>AnalysisRights.AnalysisReceptionCheck)
        );

        public static Action ReceptionAskForCorrection = Action.Create(c => c
            .Caption(w => "{Ask for correction}").Icon(w => "Icons/Workflows/Correct")
            .FromState(() => ReceptionCheck)
            .ToState(() => ReceptionCorrectionAsked)
            .NeedRight(()=>AnalysisRights.AnalysisReceptionCheck)
            .Backward()
        );

        //########################################################
        // RECEPTION CORRECTION

        public static State ReceptionCorrectionAsked = State.Create(c => c
            .Caption(w => "{Reception correction asked}").Icon(w => "Icons/Workflows/Correct")
        );

        public static Action CorrectReception = Action.Create(c => c
           .Caption(w => "{Correct reception}").Icon(w => "Icons/Workflows/Correct")
           .FromState(() => ReceptionCorrectionAsked,()=>ReceptionCheck,()=>Monograph)
           .ToState(() => Reception)
           .NeedRight(()=>AnalysisRights.AnalysisReceptionSign)
           .Backward()
        );

        //########################################################
        // MONOGRAPH

        public static State Monograph = State.Create(c => c
            .Caption(w => "{Monograph Entry}").Icon(w => "Icons/Workflows/Monograph|Icons/Validations/Edit")
            .WhenStateAllowed(() => ReceptionCheck)
        );

        public static Action ValidateMonograph = Action.Create(c => c
           .Caption(w => "{Validate monograph}").Icon(w => "Icons/Workflows/Monograph|Icons/Validations/Validated")
           .NeedRight(()=>AnalysisRights.AnalysisReceptionCheck)
           .FromState(() => Monograph)
           .ToState(() => MonographClosed)
            );

        //########################################################
        // MONOGRAPH VALIDATED

        public static State MonographClosed = State.Create(c => c
            .Caption(w => "{Monograph validated}").Icon(w => "Icons/Workflows/Monograph|Icons/Validations/Validated")

            .NotWhen(w => w.Target.PharmacopoeiaId == null)
                .WithMessage(w => "{Missing} : {Pharmacopoeia}")
                .HighlightField(w => w.Target.Pharmacopoeia)

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.PharmacopoeiaVersion))
                .WithMessage(w => "{Missing} : {Pharmacopoeia version}")
                .HighlightField(w => w.Target.PharmacopoeiaVersion)

            .NotWhen(w => w.SampleTests.Count == 0)
                .WithMessage(w => "{Missing} : {Tests}")
//                .HighlightField(w => w.Target.Pharmacopoeia)

            .When(w => {
                foreach (SampleTest test in w.SampleTests)
                {
                    if (test.Stage == SampleTestWorkflow.Specifications.Name) return false; 
                    if (test.Stage == SampleTestWorkflow.SignedSpecifications.Name) return false;
                }
                return true;
            })
                .WithMessage(w => "{Missing} : {Test specifications}")
        );

        public static Action ValidatePlanning = Action.Create(c => c
           .Caption(w => "{Schedule}").Icon(w => "Icons/Workflows/Planning|Icons/Validations/Validated")
           .FromState(() => MonographClosed)
           .ToState(() => Planning)
           .NeedRight(()=>AnalysisRights.AnalysisSchedule)
        );

        //########################################################
        // PLANNING

        public static State Planning = State.Create(c => c
            .Caption(w => "{Scheduling}").Icon(w => "icons/Workflows/Planning|Icons/Validations/Edit")
            .WhenStateAllowed(() => MonographClosed)
        );

        public static Action ValidateProduction = Action.Create(c => c
           .Caption(w => "{Put into production}").Icon(w => "Icons/Workflows/Production")
           .FromState(() => Planning)
           .ToState(() => Production)
           .NeedRight(()=>AnalysisRights.AnalysisSchedule)
        );


        //########################################################
        // PRODUCTION
        public static State Production = State.Create(c => c
            .Caption(w => "{Production}").Icon(w => "Icons/Workflows/Production")
            .WhenStateAllowed(() => MonographClosed)
        );

        //########################################################
        // CERTIFICAT

        public static Action ValidateCertificate = Action.Create( c => c
            .Caption(w => "{Print Certificate}").Icon(w => "Icons/Workflows/Certificate")
            .FromState(() => Production)
            .ToState(() => Certificate)
            .NeedRight(()=>AnalysisRights.AnalysisCertificateCreate)
            .Action(w=>{
                w.Target.Validator = w.User?.Caption;
                w.Target.ValidatorId = w.User?.Id;
                })
        );

        public static Action InvalidateSample = Action.Create( c => c
            .Caption(w => "{Invalidated}").Icon(w => "Icons/Validations/Invalidated")
            .FromState(() => Production)
            .ToState(() => Invalidated)
            .NeedRight(()=>AnalysisRights.AnalysisCertificateCreate)
            .Action(w=>{
                w.Target.Validator = w.User?.Caption;
                w.Target.ValidatorId = w.User?.Id;
                })
        );

        public static State Certificate = State.Create(c => c
            .Caption(w => "{Print certificate}").Icon(w => "Icons/Workflows/Certificate")
            .WhenStateAllowed(()=>MonographClosed)
            .When(w => {
                var validated = 0;
                var invalidated = 0;
                foreach (SampleTest test in w.SampleTests)
                {
                    if (test.Stage == SampleTestWorkflow.ValidatedResults.Name) validated++; 
                    else if (test.Stage == SampleTestWorkflow.InvalidatedResults.Name) invalidated++; 
                    else return false;
                }

                return validated>0;
            })
            .WithMessage(w=>"{Some tests not validated yet}")
        );

        public static State Invalidated = State.Create(c => c
            .Caption(w => "{Invalidated}").Icon(w => "Icons/Validations/Invalidated")
        );

        //########################################################
        // CLOSE

        public static Action Close = Action.Create( c => c
            .Caption(w => "{Close}").Icon(w => "Icons/Workflows/Close")
            .FromState(() => Certificate)
            .ToState(() => Closed)
            .NeedPharmacist()
        );

        public static State Closed = State.Create(c => c
            .Caption(w => "{Closed}").Icon(w => "Icons/Workflows/Closed")
            .SetState(() => Closed)
            //.When(w => w.Target.Invoiced).WithMessage(w => "{Not billed}")
            //.When(w => w.Target.Paid).WithMessage(w => "{Not Payed}")
            .WhenStateAllowed(() => Certificate)
        );
    }
}
