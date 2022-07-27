using HLab.Base.Wpf;
using HLab.Erp.Conformity.Annotations;
using HLab.Icons.Wpf;
using HLab.Icons.Wpf.Icons;

using System;
using System.Windows;

namespace HLab.Erp.Lims.Analysis.Module
{
    using H = DependencyHelper<ConformityIcon>;

    public partial class ConformityIcon : IconView
    {

        public static readonly DependencyProperty ConformityProperty =
        H.Property<ConformityState>()
            .Default(ConformityState.NotChecked)
            .OnChange(e => e.Update())
            .BindsTwoWayByDefault
            .Register();

        public ConformityState Conformity
        {
            set => SetValue(ConformityProperty, value);
            get => (ConformityState)GetValue(ConformityProperty);
        }

        public static readonly DependencyProperty ShowCaptionProperty =
        H.Property<bool>()
            .Default(false)
            .OnChange(e => e.Update())
            .Register();

        public bool ShowCaption
        {
            set => SetValue(ShowCaptionProperty, value);
            get => (bool)GetValue(ShowCaptionProperty);
        }

        void Update()
        {
            Path = $"Icons/Conformity/{Conformity}";
            if(ShowCaption)
            {
                    Caption = Conformity switch
                    {
                        ConformityState.NotChecked => "{Not Started}",
                        ConformityState.Running => "{Running}",
                        ConformityState.NotConform => "{Not Conform}",
                        ConformityState.Conform => "{Conform}",
                        ConformityState.Invalid => "{Not Valid}",
                        _ => throw new NotImplementedException(),
                    };
            }

        }
    }
}
