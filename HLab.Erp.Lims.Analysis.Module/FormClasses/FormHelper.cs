using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using HLab.Base;
using HLab.Erp.Forms.Annotations;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Notify.PropertyChanged;
using MySqlX.XDevAPI.Common;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    using H = H<FormHelper>;

    public enum FormMode
    {
        NotSet = 0,
        Specification,
        Capture,
        ReadOnly
    }


    public interface IForm
    {
        IFormTarget Target { get; set; }
        void Connect(int connectionId, object target);
        void Process(object sender, RoutedEventArgs e);
    }

    public class DummyForm : UserControl, IForm
    {
        public IFormTarget Target { get; set; }

        public void Connect(int connectionId, object target)
        { }

        public void Process(object sender, RoutedEventArgs e)
        { }
    }

    public class FormHelper :NotifierBase
    {
        public FormHelper() => H.Initialize(this);
        public FormMode Mode
        {
            get => _mode.Get();
            set
            {
                if(_mode.Set(value))
                    SetFormMode(value);
            }
        }
        private readonly IProperty<FormMode> _mode = H.Property<FormMode>();

        public IForm Form
        {
            get => _form.Get();
            set => _form.Set(value);
        }
        private readonly IProperty<IForm> _form = H.Property<IForm>();


        public string Xaml
        {
            get => _xaml.Get();
            set => _xaml.Set(value);
        }
        private readonly IProperty<string> _xaml = H.Property<string>();
        public string Cs
        {
            get => _cs.Get();
            set => _cs.Set(value);
        }
        private readonly IProperty<string> _cs = H.Property<string>();

        public string XamlMessage
        {
            get => _xamlMessage.Get();
            set => _xamlMessage.Set(value);
        }
        private readonly IProperty<string> _xamlMessage = H.Property<string>();
        public string CsMessage
        {
            get => _csMessage.Get();
            set => _csMessage.Set(value);
        }
        private readonly IProperty<string> _csMessage = H.Property<string>();
        public int XamlErrorLine
        {
            get => _xamlErrorLine.Get();
            set => _xamlErrorLine.Set(value);
        }
        private readonly IProperty<int> _xamlErrorLine = H.Property<int>();
        public int XamlErrorPos
        {
            get => _xamlErrorPos.Get();
            set => _xamlErrorPos.Set(value);
        }
        private readonly IProperty<int> _xamlErrorPos = H.Property<int>();


        private readonly Dictionary<string,Action<string>> _setValue = new Dictionary<string, Action<string>>(); 


        private Action<StringBuilder> _getSpecPackedValues;
        private Action<StringBuilder> _getPackedValues;

        public string GetPackedValues()
        {
            if (_getPackedValues == null) return "";

            var sb = new StringBuilder();
            _getPackedValues(sb);
            return sb.ToString();
        }
        public string GetSpecPackedValues()
        {
            if (_getSpecPackedValues == null) return "";

            var sb = new StringBuilder();
            _getSpecPackedValues(sb);
            return sb.ToString();
        }


        public async Task LoadFormAsync(IFormTarget target)
        {
            CsMessage = "";
            XamlMessage = "";

            const string header = @"
            <UserControl 
            xmlns = ""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            xmlns:x = ""http://schemas.microsoft.com/winfx/2006/xaml""
            xmlns:mc = ""http://schemas.openxmlformats.org/markup-compatibility/2006""
            xmlns:d = ""http://schemas.microsoft.com/expression/blend/2008""
            xmlns:o = ""clr-namespace:HLab.Base;assembly=HLab.Base.Wpf""
            UseLayoutRounding = ""True"" >
                <UserControl.Resources><ResourceDictionary><ResourceDictionary.MergedDictionaries>
                    <ResourceDictionary Source = ""pack://application:,,,/HLab.Erp.Lims.Analysis.Module;component/FormClasses/FormsDictionary.xaml"" />          
                </ResourceDictionary.MergedDictionaries></ResourceDictionary></UserControl.Resources >
         
                <Grid>
                <Grid.LayoutTransform><ScaleTransform ScaleX=""{Binding Scale,FallbackValue=4}"" ScaleY=""{Binding Scale,FallbackValue=4}""/></Grid.LayoutTransform>
                <!--Content-->
                </Grid>
            </UserControl >
            ";

            var xaml = header.Replace("<!--Content-->", Xaml, StringComparison.InvariantCulture);

            //chose language
            xaml = await ApplyLanguage(xaml);

            // for theme compatibility
            xaml = xaml
                .Replace("\"Black\"", "\"{DynamicResource MahApps.Brushes.ThemeForeground}\"")
                .Replace("\"White\"", "\"{DynamicResource MahApps.Brushes.ThemeBackground}\"")
                ;

            UserControl form;
            try
            {
                form = (UserControl)XamlReader.Parse(xaml);
                XamlMessage = "XAML OK";
                XamlErrorLine = -1;
                XamlErrorPos = -1;
            }
            catch (Exception ex)
            {
                var headerLength = LineCount(header.Substring(0, header.IndexOf("<!--Content-->", StringComparison.InvariantCulture)))-1;

                XamlMessage = "Error " + Environment.NewLine;
                if (ex is XamlParseException parseEx)
                {
                    XamlErrorLine = parseEx.LineNumber - headerLength;
                    XamlErrorPos = parseEx.LinePosition;

                    var oldPos = $"Line {parseEx.LineNumber}, position {parseEx.LinePosition}.";
                    var newPos = $"Line {XamlErrorLine}, position {XamlErrorPos}.";

                        XamlMessage +=
                            ex.GetType().Name
                            + Environment.NewLine
                            + parseEx.Message.Replace(oldPos, newPos);
                }
                else
                    XamlMessage += "Error XAML :" + Environment.NewLine + ex.Message;

                CsMessage = "Compilation C# impossible car erreur XAML";
                return;
            }

            // Loading C#
            if (String.IsNullOrWhiteSpace(Cs))
            {
                CsMessage = "Code was empty";
                return;
            }

            var cs = @"
                using System.Runtime;
                using System.ComponentModel;
                using HLab.Erp.Lims.Analysis.Module.FormClasses;
                using HLab.Notify.PropertyChanged;
                using HLab.Notify.Annotations;
                using HLab.Notify.Wpf;
                using HLab.Erp.Forms.Annotations;

            " + Cs;

            //Remove old FM lib
            if (cs.Contains("using FM;"))
            {
                cs = cs.Replace("using FM;", "");
                CsMessage += "using FM is obsolete !\r\n";
            }


            int index = cs.IndexOf("public class", StringComparison.InvariantCulture)+12;
            int idx2 = cs.IndexOf("\r\n",index, StringComparison.Ordinal);
            if(idx2<0) idx2 = cs.IndexOf("\n",index, StringComparison.Ordinal);

            var classname = cs.Substring(index,idx2-index);

            index = cs.IndexOf("public class", StringComparison.InvariantCulture);
            if(index > -1)
                cs = cs.Insert(index, $"using H = H<{classname}>;\n");


            // Ajout des dérivations de classes
            index = cs.IndexOf("public class", StringComparison.InvariantCulture) + 12;
            if (index > -1)
            {
                var i = cs.IndexOf("\r\n", index, StringComparison.InvariantCulture);
                if (i < 0) i = cs.IndexOf("\n", index, StringComparison.InvariantCulture);
                index = i;
            }


            cs = cs.Insert(index, " : UserControlNotifier, IForm");

            // Ajout des déclarations des objects du formulaire à lier à la classe et de la fonction Connect pour la liaison une fois instanciée
            var declarations = "";
            var connection = @"
                public void Connect(int connectionId, object target)
                {
                    switch (connectionId)
                    {";
            var elements = new List<FrameworkElement>();
            var n = 0;

            _getSpecPackedValues = null;
            _getPackedValues = null;

            _setValue.Clear();

            foreach (var fe in FindLogicalChildren<FrameworkElement>(form))
            {
                if (string.IsNullOrEmpty(fe.Name)) continue;

                elements.Add(fe);
                declarations += $"public {fe.GetType().Name} {fe.Name};\n";
                connection += $"case {n} : this.{fe.Name} = (({fe.GetType().Name})(target)); return;\n";

                if (fe is Control c)
                {
                    var isSpec = false;
                    if(fe.Tag is string tag)
                    {
                        tag = tag.ToLower();
                        if (tag.Contains("spec") || tag.Contains("norme") ) isSpec = true;
                    }

                    Action<StringBuilder> getValues = null;

                    switch (c)
                    {
                        case TextBoxEx tbe:
                            getValues += sb =>
                            {
                                sb.Append(c.Name).Append("=").Append(tbe.Double).Append("■");// Le séparateur est un ALT + 254
                            };
                            _setValue.Add(c.Name,s => tbe.Double = CSD(s));
                            break;

                        case TextBox tb:
                            getValues += sb =>
                            {
                                sb.Append(c.Name).Append("=").Append(tb.Text.Replace("■", "")).Append("■");
                            };
                            _setValue.Add(c.Name,s => tb.Text = s);
                            break;
                        case CheckBox cb:
                            var idx = cb.Name.IndexOf("__", StringComparison.Ordinal);
                            if (idx>=0)
                            {
                                var cbValue = c.Name.Replace("__", "=") + "■";
                                getValues += sb =>
                                {
                                    if (cb.IsChecked == true) sb.Append(cbValue);
                                };

                                var name = cb.Name.Substring(0, idx);
                                var thisValue = cb.Name.Substring(idx + 2);

                                void Setter(string s) => cb.IsChecked = (thisValue == s);

                                if (_setValue.TryGetValue(name, out var oldSetter))
                                {
                                    oldSetter += Setter;
                                }
                                else _setValue.Add(name,Setter);
                            }
                            else
                            {
                                getValues += sb =>
                                {
                                    sb.Append(c.Name);
                                    sb.Append(cb.IsChecked switch
                                    {
                                        null => "=N■",
                                        false => "=0■",
                                        true => "=1■",
                                    });
                                };
                            }

                            break;
                    }
                    if (isSpec) _getSpecPackedValues += getValues;
                    else _getPackedValues += getValues;
                }


                n++;
            }
            //declarations += "public event PropertyChangedEventHandler PropertyChanged;";
            declarations += @"
                public IFormTarget Target 
                {
                    get => _target.Get();
                    set => _target.Set(value);
                }
                private IProperty<IFormTarget> _target = H.Property<IFormTarget>();";
            declarations += "public " + classname + "() => H.Initialize(this);";
            cs = cs.Insert(cs.IndexOf('{', index) + 1, declarations + connection + "}\r\n}\r\n");

            var compiler = new Compiler { SourceCode = cs };

            if (!compiler.Compile())
            {
                Form = new DummyForm { Content = form };
                CsMessage = compiler.CsMessage;
                return;
            }

            var module = (IForm) compiler.Module;
            module.Target = target;

            if (module is UserControl uc)
                uc.Content = form;

            var checkboxes = new Dictionary<string, List<CheckBox>>();

            // Liaisons entre les objets Xaml et C#
            for (int i = 0; i < n; i++)
            {
                module.Connect(i, elements[i]);

                switch (elements[i])
                {
                    case TextBoxEx textBoxEx:
                        textBoxEx.DoubleChange += (sender, args) =>
                        {
                            module.Process(sender, args);
                            SetFormMode(Mode);
                        };
                        break;

                    case TextBox textBox:
                        textBox.TextChanged += (sender, args) =>
                        {
                            module.Process(sender, args);
                            SetFormMode(Mode);
                        };
                        break;

                    case CheckBox checkBox:
                        if (checkBox.Name.Contains("__"))
                        {
                            var pos = checkBox.Name.LastIndexOf("__", StringComparison.InvariantCulture);
                            var name = checkBox.Name.Substring(0, pos);

                            if (!checkboxes.TryGetValue(name, out var chk))
                            {
                                chk = new List<CheckBox>();
                                checkboxes.Add(name, chk);
                            }

                            chk.Add(checkBox);

                            checkBox.PreviewMouseDown += (sender, args) =>
                            {
                                args.Handled = true;
                                TestLegacyHelper.CheckGroup(sender, chk.ToArray());
                                module.Process(sender, args);
                                SetFormMode(Mode);
                            };
                        }
                        else
                            checkBox.PreviewMouseDown += (sender, args) =>
                            {
                                //args.Handled = true;
                                module.Process(sender, args);
                                SetFormMode(Mode);
                            };

                        break;

                    case Button button:
                        button.Click += (sender, args) =>
                        {
                            module.Process(sender, args);
                            SetFormMode(Mode);
                        };
                        break;

                }
            }

            CsMessage = "C# Ok";
            Form = module;
        }

        private static readonly Brush _normalBrush = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF));
        private static readonly Brush _specificationNeededBrush = Brushes.MediumSpringGreen;
        private static readonly Brush _specificationDoneBrush = Brushes.DarkGreen;
        private static readonly Brush _mandatoryBrush = Brushes.PaleVioletRed;
        private static readonly Brush _hiddenBrush = Brushes.Black;


        private bool SetFormMode(FormMode mode)
        {
            if (Form == null) return false;

            var isSpecMode = (mode == FormMode.Specification);
            var isCaptureMode = (mode == FormMode.Capture);
            var isReadMode = (mode == FormMode.ReadOnly);


            var specificationNeeded = 0;
            var mandatoryNeeded = 0;
            var optionalEmpty = 0;

            var specificationDone = 0;
            var mandatoryDone = 0;
            var optionalDone = 0;


            if (Form is FrameworkElement form)
            {
                foreach (var c in FindLogicalChildren<Control>(form))
                {
                    var spec = IsSpec(c);
                    var mandatory = IsMandatory(c);

                    var doneBrush = 
                        isSpecMode
                        ?spec?_specificationDoneBrush:_hiddenBrush
                        :spec?_specificationDoneBrush:_normalBrush                        
                        ;

                    var todoBrush = 
                        isSpecMode
                        ?spec?_specificationNeededBrush:_hiddenBrush
                        :mandatory?_mandatoryBrush:_normalBrush;


                    Action todo;
                    if (spec) todo = () => specificationNeeded++;
                    else if(mandatory) todo = () => mandatoryNeeded++;
                    else todo = () => optionalEmpty++;

                    Action done;
                    if (spec) done = () => specificationDone++;
                    else if(mandatory) done = () => mandatoryDone++;
                    else done = () => optionalDone++;

                    var enabled =
                        (spec && isSpecMode)
                        || (!spec && isCaptureMode);

                    switch (c)
                    {
                        // TextBoxEx
                        case TextBoxEx tb when Math.Abs(tb.Double) > double.Epsilon:
                            c.Background = doneBrush;
                            c.IsEnabled = enabled;
                            done();
                            break;

                        case TextBoxEx tb:
                            c.Background = todoBrush;
                            c.IsEnabled = enabled;
                            todo();
                            break;

                        // TextBox
                        case TextBox tb when tb.Text.Length > 0:
                            c.Background = doneBrush;
                            c.IsEnabled = enabled;
                            done();
                            break;

                        case TextBox tb:
                            c.Background = todoBrush;
                            c.IsEnabled = enabled;
                            todo();
                            break;

                        // CheckBox
                        case CheckBox cb when cb.IsChecked != null:
                            c.Background = doneBrush;
                            c.IsEnabled = enabled;
                            done();
                            break;

                        case CheckBox cb:
                            c.Background = todoBrush;
                            c.IsEnabled = enabled;
                            todo();
                            break;
                    }
                }

            }

            if (isSpecMode && Form?.Target != null)
            {
                Form.Target.SpecValues = GetSpecPackedValues();
                if (specificationNeeded > 0)
                {
                    if(Form.Target!=null)
                        Form.Target.State = FormState.NotStarted;

                    Form.Target.SpecificationsDone = false;
                }
                else Form.Target.SpecificationsDone = true;
            }

            if (isCaptureMode && Form?.Target != null)
            {
                Form.Target.Values = GetPackedValues();
                if (mandatoryNeeded > 0)
                {
                    if(Form.Target!=null)
                        Form.Target.State = mandatoryDone > 0 ? FormState.Running : FormState.NotStarted;
                    Form.Target.MandatoryDone = false;
                }
                else Form.Target.MandatoryDone = true;
            }


            if(Form.Target !=null && Form.Target.State>FormState.Running)
            {
                if (specificationNeeded > 0) Form.Target.State = FormState.NotStarted;
                if (mandatoryNeeded > 0) Form.Target.State = FormState.Running;
            }

            return mandatoryNeeded>0 || specificationNeeded>0;
        }

        private static bool IsMandatory(Control c) => IsTagged(c, "mand", "obli");
        private static bool IsSpec(Control c) => IsTagged(c, "spec", "norme");

        private static bool IsTagged(Control c,params string[] values)
        {
            if (c.Tag is string tag)
            {
                tag = tag.ToLower();
                foreach (var value in values)
                {
                    if (tag.Contains(value)) return true;
                }

            }
            return false;
        }

        public void LoadValues([NotNull]string values)
        {
            if(values==null) return;

            var dict = new Dictionary<string, string>();

            foreach (var value in values.Split('■'))// Le séparateur est un ALT + 254
            {
                var v = value.Split("=");
                if (v.Length > 1)
                {
                    dict.TryAdd(v[0],v[1]);
                }
                else if (v.Length == 1)
                {
                    dict.TryAdd(v[0],"");
                }

            }

            LoadValues(dict);
        }

        public void LoadValues(Dictionary<string,string> values)
        {
            if (!(Form is FrameworkElement form)) return;

            foreach (var c in FindLogicalChildren<Control>(form))
            {
                var name = c.Name;
                var idx = name.IndexOf("__", StringComparison.Ordinal);
                var isBool = idx >= 0;
                if (isBool && c is CheckBox chk) name = name.Substring(0, idx);

                if(values.TryGetValue(name,out var value))
                    switch (c)
                    {
                        case TextBoxEx tbe:
                            tbe.Double = CSD(value);
                            break;
                        case TextBox tb:
                            tb.Text = value;
                            break;
                        case CheckBox cb:
                            if (isBool)
                            {
                                cb.IsChecked = (c.Name == name + "__" + value);
                            }
                            else
                                cb.IsChecked = value switch
                                {
                                    "N" => null,
                                    "0" => false,
                                    "1" => true,
                                    _ => cb.IsChecked
                                };

                            break;
                    }
            }
        }


        public async Task LoadCodeAsync(byte[] code)
        {
            if(code==null) return;

            var sCode = Encoding.UTF8.GetString(await GzipToBytes(code).ConfigureAwait(false));
            var index = sCode.LastIndexOf("}\r\n", StringComparison.InvariantCulture);
            Cs = sCode.Substring(0, index + 1);
            Xaml = sCode.Substring(index + 3);
        }

        public async Task<byte[]> SaveCode()
        {
            var bytes = Encoding.UTF8.GetBytes(Cs.Trim('\r', '\n', ' ') + "\r\n" + Xaml.Trim('\r', '\n', ' '));
            return await BytesToGZip(bytes);

        }
        public static double CSD(string str)
        {
            str = str.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".", StringComparison.InvariantCulture);
            try
            {
                return Convert.ToDouble(str, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return 0.0;
            }
        }

        public static IEnumerable<T> FindLogicalChildren<T>(FrameworkElement fe) where T : FrameworkElement
        {
            if (fe != null)
            {
                foreach (object child in LogicalTreeHelper.GetChildren(fe))
                {
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    if (child is FrameworkElement)
                    {
                        foreach (T childOfChild in FindLogicalChildren<T>((FrameworkElement)child))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }

        private static async Task<byte[]> GzipToBytes(object gz)
        {
            if (gz is byte[] bytes)
            {
                if (bytes.Length == 0)
                    return null;

                try
                {
                    await using var ms = new MemoryStream();
                    await using var bytesSteam = new MemoryStream(bytes);
                    await using var gzStream = new GZipStream(bytesSteam, CompressionMode.Decompress);
                    await gzStream.CopyToAsync(ms).ConfigureAwait(false);
                    return ms.ToArray();
                }
                catch { }
            }
            return null;
        }
        private static async Task<byte[]> BytesToGZip(byte[] bytes)
        {
            if (bytes.Length == 0)
                return null;

            try
            {
                await using var ms = new MemoryStream(bytes);
                await using var gz = new MemoryStream();
                await using var zipStream = new GZipStream(gz, CompressionMode.Compress);
                await zipStream.WriteAsync(ms.ToArray(), 0, ms.ToArray().Length);
                zipStream.Close();
                return gz.ToArray();
            }
            catch { }

            return null;
        }
        public static async Task<string> ApplyLanguage(String text, string language = "")
        {
            // Choix de la langue
            if (language == "en")
                return await Task.Run(()=> Regex.Replace(Regex.Replace(text, @"\{FR=[\s|!-\|~-■]*}", ""), @"\{US=([\s|!-\|~-■]*)}", "$1")); // En anglais

            return await Task.Run(()=> Regex.Replace(Regex.Replace(text, @"\{US=[\s|!-\|~-■]*}", ""), @"\{FR=([\s|!-\|~-■]*)}", "$1")); // En français
        }
        public static int LineCount(string text)
        {
            if (text == null) return 0;
            var size = text.Length;
            var nb = size == 0 ? 0 : 1;
            for (var i = 0; i < size; i++)
                if (text[i] == '\n')
                    nb++;
            return nb;
        }

    }
}
