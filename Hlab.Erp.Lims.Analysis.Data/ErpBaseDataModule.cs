using HLab.Erp.Base.Data;
using HLab.Erp.Data;

namespace HLab.Erp.Lims.Analysis.Data
{
    public class HLabErpLimsAnalysisDataUpdaterModule : DataUpdaterModule
    {
        public HLabErpLimsAnalysisDataUpdaterModule(IDataService data) : base(data)
        { }

        protected override ISqlBuilder GetSqlUpdater(string version, ISqlBuilder builder)
        {
            switch (version)
            {
                case "0.0.0.0":
                    builder
                        .Table<Form>()
                            .AddColumn(f => f.IconPath)
                        .Table<LinkedDocument>()
                            .Create()
                        .Table<AnalysisMotivation>()
                            .Create()
                        .Table<Sample>()
                            .AddColumn(s => s.AnalysisMotivation)
                            .RenameColumn("State", s => s.ConformityId)
                        .Table<SampleTestResult>()
                            .RenameColumn("StateId", s => s.ConformityId)
                        .Table<Pharmacopoeia>()
                            .RenameColumn("NameEn", s => s.Name)
                            .AddColumn(p => p.IconPath)
                        .Table<Icon>()
                            .AddColumn(i => i.Foreground)
                        .Version("2.0.0.0")
                        ;
                    break;
                case "2.0.0.0":
                    builder
                        .Table<TestClassUnitTest>()
                            .AddColumn(t => t.TestClass)
                            .RenameColumn("AssayName", t=> t.TestName)
                            .RenameColumn("Specifications", t=> t.Specification)
                            .AddColumn(t=> t.Name)
                            .AddColumn(t=> t.SpecificationValues)
                            .AddColumn(t=> t.SpecificationDone)
                            .AddColumn(t=> t.ConformityId)
                            .AddColumn(t=> t.Conformity)
                            .AddColumn(t=> t.MandatoryDone)
                        .Table<SampleForm>()
                            .RenameColumn("SpecificationsDone", t=> t.SpecificationDone)
                            .RenameColumn("SpecValues", t=> t.SpecificationValues)
                            .RenameColumn("Values", t=> t.ResultValues)
                        .Table<FormClass>()
                            .AddColumn(t=> t.CodeHash)
                        .Table<Continent>()
                            .AddColumn(c => c.Code)
                        .Version("2.1.0.0");
                    break;
                case "2.1.0.0":
                    builder
                        .Table<SampleTest>()
                        .RenameColumn("SpecificationsDone", t => t.SpecificationDone)

                        .Table<SampleForm>()
                        .RenameColumn("State", s => s.ConformityId)

                        .Table<TestClassUnitTest>()
                        .RenameColumn("Values", t => t.ResultValues);
                    break;

            }
            return base.GetSqlUpdater(version, builder);
        }
    }
}
