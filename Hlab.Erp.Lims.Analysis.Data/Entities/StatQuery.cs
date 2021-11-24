﻿using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = H<StatQuery>;

    public partial class StatQuery : Entity, IListableModel, ILocalCache
    {
        public StatQuery() => H.Initialize(this);

        public override string ToString() => Nom;

        public string Nom
        {
            get => _nom.Get(); set => _nom.Set(value);
        }
        private readonly IProperty<string> _nom = H.Property<string>(c => c.Default(""));
        public string P1
        {
            get => _p1.Get(); set => _p1.Set(value);
        }
        private readonly IProperty<string> _p1 = H.Property<string>(c => c.Default(""));
        public string P2
        {
            get => _p2.Get(); set => _p2.Set(value);
        }
        private readonly IProperty<string> _p2 = H.Property<string>(c => c.Default(""));
        public string P3
        {
            get => _p3.Get(); set => _p3.Set(value);
        }
        private readonly IProperty<string> _p3 = H.Property<string>(c => c.Default(""));
        public string P4
        {
            get => _p4.Get(); set => _p4.Set(value);
        }
        private readonly IProperty<string> _p4 = H.Property<string>(c => c.Default(""));
        public int T1
        {
            get => _t1.Get(); set => _t1.Set(value);
        }
        private readonly IProperty<int> _t1 = H.Property<int>();
        public int T2
        {
            get => _t2.Get(); set => _t2.Set(value);
        }
        private readonly IProperty<int> _t2 = H.Property<int>();
        public int T3
        {
            get => _t3.Get(); set => _t3.Set(value);
        }
        private readonly IProperty<int> _t3 = H.Property<int>();
        public int T4
        {
            get => _t4.Get(); set => _t4.Set(value);
        }
        private readonly IProperty<int> _t4 = H.Property<int>();
        public string Query
        {
            get => _query.Get(); set => _query.Set(value);
        }
        private readonly IProperty<string> _query = H.Property<string>(c => c.Default(""));
        public string Parametres
        {
            get => _parametres.Get(); set => _parametres.Set(value);
        }
        private readonly IProperty<string> _parametres = H.Property<string>(c => c.Default(""));
        public string TaillesColonnes
        {
            get => _taillesColonnes.Get(); set => _taillesColonnes.Set(value);
        }
        private readonly IProperty<string> _taillesColonnes = H.Property<string>(c => c.Default(""));
        //public string Droit
        //{
        //    get => _droit.Get(); set => _droit.Set(value);
        //}
        //private readonly IProperty<string> _droit = H.Property<string>(c => c.Default(""));
        //public string Cache
        //{
        //    get => _cache.Get(); set => _cache.Set(value);
        //}
        //private readonly IProperty<string> _cache = H.Property<string>(c => c.Default(""));

        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Nom)
            .Set(e => string.IsNullOrWhiteSpace(e.Nom)?"{New motivation}":$"{{Motivation}}\n{e.Nom}")
        );

        [Ignore]
        public string IconPath => "";

    }
}