using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

namespace HLab.Erp.Lims.Analysis.Module.Prints;

/// <summary>
/// Logique d'interaction pour MiseEnForme.xaml
/// </summary>
public partial class Formatter : Popup
{
    RichTextBox _RichTextBox = null;

    public RichTextBox RichTextBox
    {
        get
        {
            return (RichTextBox)GetValue(RichTextBoxProperty);
        }
        set
        {
            //Console.WriteLine("set _RichTextBox");
            SetValue(RichTextBoxProperty, value);
           
            _RichTextBox = value;
            Placement = PlacementMode.Relative;//. PlacementMode.ac PlacementMode.MousePoint;//.Mouse;
            PlacementTarget = _RichTextBox;
            _RichTextBox.SelectionChanged += _RichTextBox_SelectionChanged;
            //_RichTextBox.PreviewMouseMove+=_RichTextBox_PreviewMouseMove;
            //_RichTextBox.PreviewMouseLeftButtonDown+=_RichTextBox_PreviewMouseLeftButtonDown;
            _RichTextBox.PreviewMouseLeftButtonUp+=_RichTextBox_PreviewMouseLeftButtonUp;

            //_RichTextBox.MouseLeftButtonUp+=_RichTextBox_MouseLeftButtonUp;

            //_RichTextBox.MouseMove+=_RichTextBox_MouseMove;
            //_RichTextBox.MouseLeave+=_RichTextBox_MouseLeave;
            //_RichTextBox.LostFocus+=_RichTextBox_LostFocus;
            //_RichTextBox.IsMouseDirectlyOverChanged+=_RichTextBox_IsMouseDirectlyOverChanged;

            _RichTextBox.LostKeyboardFocus+=_RichTextBox_LostKeyboardFocus;
            //_RichTextBox.GotKeyboardFocus+=_RichTextBox_GotKeyboardFocus;
        }
    }

    void _RichTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if(_RichTextBox.Selection.Text.Length > 0 && !IsOpen)
            IsOpen = true;
    }

    void _RichTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        //if(BT_Gras == e.NewFocus)
        //   return;

        IsOpen = false;
    }

    void _RichTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if(_RichTextBox.Selection.Text.Length > 0 && !IsOpen)
        {
            Point pt = Mouse.GetPosition(_RichTextBox);
            HorizontalOffset = pt.X - 110.0;
            VerticalOffset   = pt.Y + 20.0;
            IsOpen = true;
        }
        else
            IsOpen = false;
    }

    //void _RichTextBox_PreviewMouseMove(object sender, MouseEventArgs e)
    //{
    //   if(IsOpen && e.LeftButton == MouseButtonState.Pressed)
    //   {
    //      Point pt = Mouse.GetPosition(_RichTextBox);
    //      HorizontalOffset = pt.X - 110.0;
    //      VerticalOffset   = pt.Y + 20.0;
    //   }
    //}

    //void _RichTextBox_MouseMove(object sender, MouseEventArgs e)
    //{
    //   if(IsOpen && e.LeftButton == MouseButtonState.Pressed)
    //   {
    //      Point positionBouton = e.GetPosition(this);
    //      Console.WriteLine(positionBouton);
    //   }
    //      //Placement = PlacementMode.MousePoint;
    //}

    //void _RichTextBox_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
    //{
    //   Console.WriteLine(e.NewValue);
    //}

    //void _RichTextBox_MouseLeave(object sender, MouseEventArgs e)
    //{
    //   //Console.WriteLine(e.OriginalSource); e.Handled=true;
    //}

    //void _RichTextBox_LostFocus(object sender, RoutedEventArgs e)
    //{
    //   IsOpen = false;
    //}

    void _RichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if(_RichTextBox.Selection.Text.Length == 0)
            IsOpen = false;
    }

    public static readonly DependencyProperty RichTextBoxProperty = DependencyProperty.Register("RichTextBox", typeof(RichTextBox), typeof(Formatter), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnRichTextBoxChanged)));

    static void OnRichTextBoxChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        //Console.WriteLine("OnRichTextBoxChanged");     
    
        Formatter mef = o as Formatter;
        mef.RichTextBox = (RichTextBox)e.NewValue;
    }
   

    public Formatter()
    {
        //Console.WriteLine("MiseEnForme");
        InitializeComponent();
        StaysOpen = false;
    }

    void BT_Gras_Click(object sender, RoutedEventArgs e)
    {
        if(_RichTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty).Equals(FontWeights.Bold))
            _RichTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
        else
            _RichTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
        _RichTextBox.Focus();
    }

    void BT_Italique_Click(object sender, RoutedEventArgs e)
    {
        if(_RichTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty).Equals(FontStyles.Italic))
            _RichTextBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
        else
            _RichTextBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
        _RichTextBox.Focus();
    }

    void BT_Souligne_Click(object sender, RoutedEventArgs e)
    {
        if(_RichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty).Equals(TextDecorations.Underline))
            _RichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
        else
            _RichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
        _RichTextBox.Focus();
    }

    void BT_Barre_Click(object sender, RoutedEventArgs e)
    {
        if(_RichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty).Equals(TextDecorations.Strikethrough))
            _RichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
        else
            _RichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
        _RichTextBox.Focus();
    }

    void BT_Gauche_Click(object sender, RoutedEventArgs e)
    {
        _RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
        _RichTextBox.Focus();
    }

    void BT_Centre_Click(object sender, RoutedEventArgs e)
    {
        _RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
        _RichTextBox.Focus();
    }

    void BT_Droite_Click(object sender, RoutedEventArgs e)
    {
        _RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
        _RichTextBox.Focus();
    }

    void BT_Jutifie_Click(object sender, RoutedEventArgs e)
    {
        _RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Justify);
        _RichTextBox.Focus();
    }

    void BT_IndenteMoins_Click(object sender, RoutedEventArgs e)
    {
        object o = _RichTextBox.Selection.GetPropertyValue(Paragraph.MarginProperty);

        if(o is Thickness)
        {
            double retrait = ((Thickness)o).Left;
            retrait -= 20.0; // 20 correspond à une tabulation
            if(retrait <0.0)
                retrait = 0.0;
            _RichTextBox.Selection.ApplyPropertyValue(Paragraph.MarginProperty, new Thickness(retrait, ((Thickness)o).Top, ((Thickness)o).Right, ((Thickness)o).Bottom));
        }
        _RichTextBox.Focus();
    }

    void BT_IndentePlus_Click(object sender, RoutedEventArgs e)
    {
        object o = _RichTextBox.Selection.GetPropertyValue(Paragraph.MarginProperty);

        if(o is Thickness)
        {
            double retrait = ((Thickness)o).Left;
            retrait += 20.0; // 20 correspond à une tabulation
            _RichTextBox.Selection.ApplyPropertyValue(Paragraph.MarginProperty, new Thickness(retrait, ((Thickness)o).Top, ((Thickness)o).Right, ((Thickness)o).Bottom));
        }
        _RichTextBox.Focus();
    }

    void BT_Indice_Click(object sender, RoutedEventArgs e)
    {
        if(_RichTextBox.Selection.GetPropertyValue(Typography.VariantsProperty).Equals(FontVariants.Subscript))
            _RichTextBox.Selection.ApplyPropertyValue(Typography.VariantsProperty, FontVariants.Normal);
        else
            _RichTextBox.Selection.ApplyPropertyValue(Typography.VariantsProperty, FontVariants.Subscript);
        _RichTextBox.Focus();
    }

    void BT_Exposant_Click(object sender, RoutedEventArgs e)
    {
        if(_RichTextBox.Selection.GetPropertyValue(Typography.VariantsProperty).Equals(FontVariants.Ordinal))
            _RichTextBox.Selection.ApplyPropertyValue(Typography.VariantsProperty, FontVariants.Normal);
        else
            _RichTextBox.Selection.ApplyPropertyValue(Typography.VariantsProperty, FontVariants.Ordinal);
        _RichTextBox.Focus();
    }
}