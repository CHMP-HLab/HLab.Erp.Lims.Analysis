using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Erp.Workflows;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Filters
{
    using H = H<ConformityFilter>;

    public class ConformityFilter: FilterViewModel, IWorkflowFilter
    {

        private static readonly MethodInfo ContainsMethod = typeof(List<ConformityState>).GetMethod("Contains", new[] {typeof(ConformityState)});

        public class ConformityEntry : NotifierBase
        {
            public ConformityEntry() => H<ConformityEntry>.Initialize(this);

            public bool Selected
            {
                get => _selected.Get(); 
                set => _selected.Set(value);
            }
            private readonly IProperty<bool> _selected = H<ConformityEntry>.Property<bool>();

            public ConformityState State { get; set; }

            public string IconPath => State.IconPath();
            public string Caption => State.Caption();
        }

        public ReadOnlyObservableCollection<ConformityEntry> List { get; }
        public ObservableCollection<ConformityEntry> _list = new();

        public ConformityFilter()
        {
            List = new (_list);
            H<ConformityFilter>.Initialize(this);

            var values = Enum.GetValues(typeof(ConformityState)).Cast<ConformityState>();

            foreach (var state in values)
            {
                _list.Add(new ConformityEntry{State = state});
            }
        }

        private ITrigger _ = H<ConformityFilter>.Trigger(c => c
            .On(e => e.List.Item().Selected)
            .On(e => e.Enabled)
            .Do(e => e.Update?.Invoke())
        );


        public ConformityState Selected { get; set; }


        public Expression<Func<T,bool>> Match<T>(Expression<Func<T, ConformityState>> getter)
        {
            if (!Enabled) return null; 

            var entity = getter.Parameters[0];
            var value = Expression.Constant(List.Where(e => e.Selected).Select(e => e.State).ToList(),typeof(List<ConformityState>));

            var ex = Expression.Call(value,ContainsMethod,getter.Body);

            return Expression.Lambda<Func<T, bool>>(ex,entity);
        }

        public Func<T,bool> Match<T>(Func<T, ConformityState> getter)
        {
            if (!Enabled) return t => true; 

            var values = List.Where(e => e.Selected).Select(e => e.State).ToList();

            return t => values.Contains(getter(t));
        }

        public Action Update
        {
            get => _update.Get();
            set => _update.Set(value);
        }
        private readonly IProperty<Action> _update = H.Property<Action>();


        public ConformityFilter Link<T>(ObservableQuery<T> q, Expression<Func<T, ConformityState>> getter)
            where T : class, IEntity
        {
            //var entity = getter.Parameters[0];
            q.AddFilter(Title,()=> Match(getter));
            Update = q.Update;
            return this;
        }
        public ConformityFilter PostLink<T>(ObservableQuery<T> q, Func<T, ConformityState> getter)
            where T : class, IEntity
        {
            //var entity = getter.Parameters[0];
            q.AddPostFilter(Title,e=> Match(getter).Invoke(e));
            Update = q.Update;
            return this;
        }

    }
}