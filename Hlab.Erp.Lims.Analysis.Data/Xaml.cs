using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data
{
    public class Xaml : Entity<Xaml>, ILocalCache
    {
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));
        public string Page
        {
            get => _page.Get();
            set => _page.Set(value);
        }

        readonly IProperty<string> _page = H.Property<string>(c => c.Default(""));
    }
}
