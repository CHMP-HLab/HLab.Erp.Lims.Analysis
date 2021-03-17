using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using HLab.Base.Wpf;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    using H = H<FormTestClassHelper>;

    public class FormTestClassHelper : NotifierBase
    {
        public FormTestClassHelper() => H.Initialize(this);


        public TestFormMode Mode
        {
            get => _mode.Get();
            set
            {
                if(_mode.Set(value))
                    SetFormMode(value);
            }
        }

        private readonly IProperty<TestFormMode> _mode = H.Property<TestFormMode>();
        public string Xaml
        {
            get => _xaml.Get();
            set => _xaml.Set(value);
        }
        private readonly IProperty<string> _xaml = H.Property<string>();

        public string XamlMessage
        {
            get => _xamlMessage.Get();
            set => _xamlMessage.Set(value);
        }
        private readonly IProperty<string> _xamlMessage = H.Property<string>();
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

        public string Cs
        {
            get => _cs.Get();
            set => _cs.Set(value);
        }
        private readonly IProperty<string> _cs = H.Property<string>();

        public string CsMessage
        {
            get => _csMessage.Get();
            set => _csMessage.Set(value);
        }
        private readonly IProperty<string> _csMessage = H.Property<string>();
        public int CsErrorLine
        {
            get => _csErrorLine.Get();
            set => _csErrorLine.Set(value);
        }
        private readonly IProperty<int> _csErrorLine = H.Property<int>();
        public int CsErrorPos
        {
            get => _csErrorPos.Get();
            set => _csErrorPos.Set(value);
        }
        private readonly IProperty<int> _csErrorPos = H.Property<int>();

        public ITestForm Form
        {
            get => _form.Get();
            set => _form.Set(value);
        }
        private readonly IProperty<ITestForm> _form = H.Property<ITestForm>();

        public SampleTest Test
        {
            get => _test.Get();
            set => _test.Set(value);
        }
        private readonly IProperty<SampleTest> _test = H.Property<SampleTest>();

        public SampleTestResult Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }

        private readonly IProperty<SampleTestResult> _result = H.Property<SampleTestResult>();

        private readonly ITrigger _ = H.Trigger(c => c
            .On(e => e.Form.Test.Result)
            .NotNull(e => e.Result)
            .NotNull(e => e.Form.Test.Result)
            .Do(e => e.Result.Result = e.Form.Test.Result)

            .On(e => e.Form.Test.State)
            .NotNull(e => e.Result)
            .NotNull(e => e.Form.Test.State)
            .Do(e => e.Result.ConformityId = e.Form.Test.State)

            .On(e => e.Form.Test.Specifications)
            .NotNull(e => e.Test)
            .NotNull(e => e.Form.Test?.Specifications)
            .Do(e => e.Test.Specification = e.Form.Test.Specifications)

            .On(e => e.Form.Test.TestName)
            .NotNull(e => e.Test)
            .NotNull(e => e.Form.Test?.TestName)
            .Do(e => e.Test.TestName = e.Form.Test.TestName)

            .On(e => e.Form.Test.Description)
            .NotNull(e => e.Test)
            .NotNull(e => e.Form.Test?.Description)
            .Do(e => e.Test.Description = e.Form.Test.Description)

            .On(e => e.Form.Test.Conformity)
            .NotNull(e => e.Test)
            .NotNull(e => e.Form.Test?.Conformity)
            .Do(e => e.Test.Conform = e.Form.Test.Conformity)
        );

        public async Task ExtractCode(byte[] code)
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


        public async Task LoadForm()
        {
            CsMessage = "";
            XamlMessage = "";

            const string header = @"
            <UserControl 
                xmlns = ""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                xmlns:x = ""http://schemas.microsoft.com/winfx/2006/xaml""
                xmlns:mc = ""http://schemas.openxmlformats.org/markup-compatibility/2006""
                xmlns:d = ""http://schemas.microsoft.com/expression/blend/2008""
                xmlns:o = ""clr-namespace:HLab.Base.Wpf;assembly=HLab.Base.Wpf""
                UseLayoutRounding = ""True"" >
                <UserControl.Resources>
                    <ResourceDictionary Source = ""pack://application:,,,/HLab.Erp.Lims.Analysis.Module;component/FormClasses/FormsDictionary.xaml"" />          
                </UserControl.Resources >
                <Grid>
                <Grid.LayoutTransform>
                    <ScaleTransform 
                        ScaleX=""{Binding Scale,FallbackValue=4}"" 
                        ScaleY=""{Binding Scale,FallbackValue=4}""/>
                </Grid.LayoutTransform>
                <!--Content-->
                </Grid>
            </UserControl >
            ";

            var xaml = header.Replace("<!--Content-->", Xaml, StringComparison.InvariantCulture);
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
                #if DEBUG
                Form = new DummyTestForm {Content = new TextBlock() {Text = XamlMessage}};
                #endif
                return;
            }

            // Loading C#
            if (String.IsNullOrWhiteSpace(Cs))
            {
                CsMessage = "Code was empty";
                return;
            }

            var cs = @$"
                using System.Runtime;
                using System.ComponentModel;
                using HLab.Erp.Lims.Analysis.Module.TestClasses;
                using HLab.Notify.PropertyChanged;
                using HLab.Notify.Annotations;
                using HLab.Notify.Wpf; 
                {Cs}
            ";

            cs = cs
                .Replace("Test.CheckGroupe","TestLegacyHelper.CheckGroup")
                .Replace("Test.CheckGroup","TestLegacyHelper.CheckGroup");

            if (cs.Contains("using FM;"))
            {
                cs = cs.Replace("using FM;", "");
                CsMessage += "using FM is obsolete !\r\n";
            }

            int index = cs.IndexOf("public class", StringComparison.InvariantCulture)+12;
            var idx2 = EndOfLine(cs,index);

            var classname = cs.Substring(index,idx2-index);

            index = cs.IndexOf("public class", StringComparison.InvariantCulture);
            if(index > -1)
                cs = cs.Insert(index, $"using H = H<{classname}>;\n");


            // Ajout des dérivations de classes
            index = cs.IndexOf("public class", StringComparison.InvariantCulture) + 12;
            if (index > -1)
            {
                index = EndOfLine(cs,index);
            }


            cs = cs.Insert(index, " : UserControlNotifier, ITestForm");

            // Ajout des déclarations des objects du formulaire à lier à la classe et de la fonction Connect pour la liaison une fois instanciée
            string declarations = "";
            var connection = "\npublic void Connect(int connectionId, object target)\n{\nswitch (connectionId){\n";
            var elements = new List<FrameworkElement>();
            var n = 0;

            _getSpecPackedValues = null;
            _getPackedValues = null;

            _setValue.Clear();

            foreach (FrameworkElement fe in FindLogicalChildren<FrameworkElement>(form))
            {
                if (!string.IsNullOrEmpty(fe.Name))
                {

                    elements.Add(fe);
                    declarations += "public " + fe.GetType().FullName + " " + fe.Name + ";\n";
                    connection += "case " + n + ": this." + fe.Name + " = ((" + fe.GetType().FullName + ")(target)); return;\n";

                    if (fe is Control c)
                    {
                        var isSpec = false;
                        if(fe.Tag is string tag)
                        {
                            tag = tag.ToLower();
                            if (tag.Contains("spec") || tag.Contains("norme") ) isSpec = true;
                        }

                        Action<StringBuilder> getValues=null;

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
            }
            //declarations += "public event PropertyChangedEventHandler PropertyChanged;";
            declarations += @"
                public TestLegacyHelper Test => _test.Get();
                private IProperty<TestLegacyHelper> _test = H.Property<TestLegacyHelper>(c => c
                    .Set(e => new TestLegacyHelper()));
                ITestHelper ITestForm.Test => Test;";

            declarations += $"public {classname}() => H<{classname}>.Initialize(this);";
            cs = cs.Insert(cs.IndexOf('{', index) + 1, declarations + connection + "}\r\n}\r\n");

            var compiler = new Compiler.Wpf.Compiler { SourceCode = cs };

            if (!compiler.Compile())
            {
                Form = new DummyTestForm { Content = form };
                CsMessage = compiler.CsMessage;
                return;
            }

            var module = (ITestForm) compiler.Module;

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
                            module.Test.Reset();
                            module.Traitement(sender, args);
                            SetFormMode(Mode);
                        };
                        break;

                    case TextBox textBox:
                        textBox.TextChanged += (sender, args) =>
                        {
                            textBox.ApplySymbols();
                            module.Test.Reset();
                            module.Traitement(sender, args);
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
                                module.Test.Reset();
                                module.Traitement(sender, args);
                                SetFormMode(Mode);
                            };
                        }
                        else
                            checkBox.PreviewMouseDown += (sender, args) =>
                            {
                                args.Handled = true;
                                module.Test.Reset();
                                module.Traitement(sender, args);
                                SetFormMode(Mode);
                            };

                        break;

                    case Button button:
                        button.Click += (sender, args) =>
                        {
                            module.Traitement(sender, args);
                            SetFormMode(Mode);
                        };
                        break;

                }
            }

            CsMessage = "C# Ok";
            Form = module;
        }

        private static int EndOfLine(string s,int index)
        {
            var idx1 = s.IndexOf("\n",index, StringComparison.InvariantCulture);
            var idx2 = s.IndexOf("\r\n",index, StringComparison.InvariantCulture);
            if(idx1<0) return idx2;
            if(idx2<0) return idx1;
            return Math.Min(idx1,idx2);
        }

        public enum TestValueLevel
        {
            Test,
            Result
        }


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

        public void LoadValues([NotNull]string values)
        {
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

            SetFormMode(Mode);
        }


        private readonly Dictionary<string,Action<string>> _setValue = new Dictionary<string, Action<string>>(); 

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

        // TODO : internationalize
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


        private static readonly Brush _normalBrush = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF));
        private static readonly Brush _specificationNeededBrush = Brushes.MediumSpringGreen;
        private static readonly Brush _specificationDoneBrush = Brushes.DarkGreen;
        private static readonly Brush _mandatoryBrush = Brushes.PaleVioletRed;
        private static readonly Brush _hiddenBrush = Brushes.Black;


        public bool SetFormMode(TestFormMode mode)
        {
            var isSpecMode = (mode == TestFormMode.Specification);
            var isCaptureMode = (mode == TestFormMode.Capture);
            var isReadMode = (mode == TestFormMode.ReadOnly);


            var specificationNeeded = 0;
            var mandatoryNeeded = 0;
            var optionalEmpty = 0;

            var specificationDone = 0;
            var mandatoryDone = 0;
            var optionalDone = 0;

            if(Form == null) return false;

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

            if (isSpecMode && Test != null)
            {
                Test.Values = GetSpecPackedValues();
                if (specificationNeeded > 0)
                {
                    if(Form.Test!=null)
                        Form.Test.State = ConformityState.NotChecked;

                    Test.SpecificationsDone = false;
                }
                else Test.SpecificationsDone = true;
            }

            if (isCaptureMode && Result != null)
            {
                Result.Values = GetPackedValues();
                if (mandatoryNeeded > 0)
                {
                    if(Form.Test!=null)
                        Form.Test.State = mandatoryDone > 0 ? ConformityState.Running : ConformityState.NotChecked;
                    Result.MandatoryDone = false;
                }
                else Result.MandatoryDone = true;
            }


            if(Form.Test !=null && Form.Test.State>ConformityState.Running)
            {
                if (specificationNeeded > 0) Form.Test.State = ConformityState.NotChecked;
                if (mandatoryNeeded > 0) Form.Test.State = ConformityState.Running;
            }

            if (Result != null)
            {
                if (Form.Test != null && string.IsNullOrWhiteSpace(Form.Test.Conformity))
                {
                    Form.Test.Conformity = Form.Test.State switch
                    {
                        ConformityState.Undefined => "{Undefined}",
                        ConformityState.NotChecked => "{Not started}",
                        ConformityState.Running => "{Running}",
                        ConformityState.NotConform => "{Not conform}",
                        ConformityState.Conform => "{Conform}",
                        ConformityState.Invalid => "{Invalid}",
                        _ => throw new ArgumentOutOfRangeException()
                    };

                }

                if (Mode == TestFormMode.Capture && Form.Test!=null)
                {
                    Result.Conformity = Form.Test.Conformity;
                    Result.Result = Form.Test.Result;
                    Result.ConformityId = Form.Test.State;
                }
            }
            else
            {
                if (Test != null)
                {
                    if (Form.Test != null)
                    {
                        if (string.IsNullOrWhiteSpace(Form.Test.TestName)) 
                            Form.Test.TestName = Test.TestClass.Name;

                        if (Mode == TestFormMode.Specification)
                        {
                            Test.TestName = Form.Test.TestName;
                            Test.Description = Form.Test.Description;
                            Test.Specification = Form.Test.Specifications;
                        }
                    }
                }
            }


            return mandatoryNeeded>0 || specificationNeeded>0;
        }


        private static bool IsMandatory(Control c)
        {
            if(c.Tag==null) return false;
            var tag = c.Tag.ToString().ToLower();
            if(tag.Contains("mand")) return true;
            if (tag.Contains("obli")) return true;
            return false;
        }
        private static bool IsSpec(Control c)
        {
            if(c.Tag==null) return false;
            var tag = c.Tag.ToString().ToLower();
            if(tag.Contains("spec")) return true;
            if (tag.Contains("norme")) return true;
            return false;
        }

        public static void CalculEtat(FrameworkElement form, ref DateTime dateDebut, ref DateTime dateFin)
        {
            // Détermine comment les champs sont remplis
            var nbMandatory = 0;
            var nbMandatoryOK = 0;
            var nbSpecifications = 0;
            var nbSpecificationsOK = 0;

            Brush fontTextBox = new SolidColorBrush(Color.FromArgb(207, 255, 255, 255));


            foreach (Control c in FindLogicalChildren<Control>(form))
            {
                if (c.Tag == null)
                    continue;

                var tag = c.Tag.ToString();

                if (IsMandatory(c))
                {
                    nbMandatory++;

                    switch (c)
                    {
                        // TextBoxEx
                        case TextBoxEx tb when Math.Abs(tb.Double) > double.Epsilon:
                            nbMandatoryOK++;
                            c.Background = fontTextBox;
                            break;

                        // TextBox
                        case TextBoxEx tb:
                            c.Background = Brushes.LightBlue;
                            break;

                        case TextBox box when box.Text.Length > 0:
                            nbMandatoryOK++;
                            box.Background = fontTextBox;
                            break;

                        // CheckBox
                        case TextBox _:
                            c.Background = Brushes.LightBlue;
                            break;

                        case CheckBox box when box.IsChecked != null:
                            nbMandatoryOK++;
                            box.Background = Brushes.Transparent;
                            break;

                        case CheckBox _:
                            c.Background = Brushes.LightBlue;
                            break;
                    }
                }

                if (!IsSpec(c)) continue;
                nbSpecifications++;

                switch (c)
                {
                    // TextBoxEx
                    case TextBoxEx tb when Math.Abs(tb.Double) > double.Epsilon:
                        nbSpecificationsOK++;
                        c.Background = fontTextBox;
                        break;

                    // TextBox
                    case TextBoxEx tb:
                        c.Background = Brushes.PaleGreen;
                        break;

                    case TextBox tb when tb.Text.Length > 0:
                        nbSpecificationsOK++;
                        c.Background = fontTextBox;
                        break;

                    case TextBox tb:
                        c.Background = Brushes.PaleGreen;
                        break;
                }
            }


            ConformityState state;
            // Modifie l'état du test en fonction de son avancée
            if (nbMandatory > 0)
            {
                if (nbMandatoryOK == 0)
                {
                    if (dateDebut == DateTime.MinValue)
                        state = ConformityState.NotChecked;
                    else
                        state = ConformityState.Running;
                }
                else if (nbMandatoryOK < nbMandatory)
                {
                    state = ConformityState.Running;

                    // Si il n'y avait pas de date de début, en attribue une
                    if (dateDebut == DateTime.MinValue)
                        dateDebut = DateTime.Now;

                }

                // Si le test est terminé et qu'il n'y avait pas de date de fin
                else if (dateFin == DateTime.MinValue)
                {
                    // Si il n'y avait pas de date de début, en attribue une
                    if (dateDebut == DateTime.MinValue)
                        dateDebut = DateTime.Now;

                }
            }

            //// La validation du pharmacien
            //if(validation != TestValidation.Valide && validation != TestValidation.NonValide && validation != TestValidation.Signed)
            //{
            //   if(nbSpecificationsOK < nbSpecifications)
            //      validation = TestValidation.NonNorme;
            //   else
            //      validation = TestValidation.ToSign;
            //}
        }
        public Task LoadAsync(SampleTestResult result)
        {
            return LoadAsync(result.SampleTest, result);
        }
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        public async Task LoadAsync(SampleTest test, SampleTestResult result = null)
        {
            await _lock.WaitAsync();
            try
            {
                if (!ReferenceEquals(Test, test))
                {
                    if (Test != null) throw new Exception("Test should be null or same");
                    Test = test;
                    await ExtractCode(test.Code).ConfigureAwait(true);
                    await LoadForm().ConfigureAwait(true);
                    if (test?.Values != null)
                        LoadValues(test.Values);
                }

                //if (!ReferenceEquals(Result, result))
                {
                    Result = result;
                    if (result?.Values != null)
                        LoadValues(result.Values);
                }

                Form?.Traitement(null, null);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}