using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using HLab.Base;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    public enum TestFormMode
    {
        Specification,
        Capture,
        ReadOnly
    }

    public interface ITestForm
    {
        void Connect(int connectionId, object target);
        void Traitement(object sender, RoutedEventArgs e);

        ITestHelper Test { get; }
    }

    public class DummyTestForm : UserControl, ITestForm
    {
        public ITestHelper Test => null;

        public void Connect(int connectionId, object target)
        {
        }

        public void Traitement(object sender, RoutedEventArgs e)
        {
        }
    }

    public class FormHelper : N<FormHelper>
    {
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
        private readonly IProperty<SampleTestResult> _result = H.Property<SampleTestResult>(c => c
            .On(e => e.Form.Test.Result)
            .NotNull(e => e.Result)
            .NotNull(e => e.Form.Test.Result)
                .Do(e => e.Result.Result = e.Form.Test.Result)

            .On(e => e.Form.Test.State)
            .NotNull(e => e.Result)
            .NotNull(e => e.Form.Test.State)
                .Do(e => e.Result.StateId = (int)e.Form.Test.State)

            .On(e => e.Form.Test.Specifications)
            .NotNull(e => e.Test)
            .NotNull(e => e.Form.Test.Specifications)
                .Do(e => e.Test.Specification = e.Form.Test.Specifications)

            .On(e => e.Form.Test.TestName)
            .NotNull(e => e.Test)
            .NotNull(e => e.Form.Test.TestName)
                .Do(e => e.Test.TestName = e.Form.Test.TestName)

            .On(e => e.Form.Test.Description)
            .NotNull(e => e.Test)
            .NotNull(e => e.Form.Test.Description)
                .Do(e => e.Test.Description = e.Form.Test.Description)

            .On(e => e.Form.Test.Conformity)
            .NotNull(e => e.Test)
            .NotNull(e => e.Form.Test.Conformity)
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
            xmlns:o = ""clr-namespace:HLab.Base;assembly=HLab.Base.Wpf""
            UseLayoutRounding = ""True"" >
                <UserControl.Resources><ResourceDictionary><ResourceDictionary.MergedDictionaries>
                    <ResourceDictionary Source = ""pack://application:,,,/HLab.Erp.Lims.Analysis.Module;component/TestClasses/FormsDictionary.xaml"" />          
                </ResourceDictionary.MergedDictionaries></ResourceDictionary></UserControl.Resources >
         
                <Grid>
                <Grid.LayoutTransform><ScaleTransform ScaleX=""{Binding Scale,FallbackValue=4}"" ScaleY=""{Binding Scale,FallbackValue=4}""/></Grid.LayoutTransform>
                <!--Content-->
                </Grid>
            </UserControl >
            ";

            var xaml = header.Replace("<!--Content-->", Xaml, StringComparison.InvariantCulture);
            xaml = await ApplyLanguage(xaml);

            // for theme compatibility
            xaml = xaml
                .Replace("\"Black\"", "\"{DynamicResource MahApps.Brushes.Black}\"")
                .Replace("\"White\"", "\"{DynamicResource MahApps.Brushes.White}\"")
                ;
            UserControl form;
            try
            {
                form = (UserControl)XamlReader.Parse(xaml);
                XamlMessage = "XAML OK";
            }
            catch (Exception ex)
            {
                int headerLength = LineCount(header.Substring(0, header.IndexOf("<!--Content-->", StringComparison.InvariantCulture)));

                XamlMessage = "Error " + Environment.NewLine;
                if (ex is XamlParseException parseEx)
                    XamlMessage +=
                        ex.GetType().Name
                        + Environment.NewLine
                        + parseEx.Message
                        + " Line "
                        + (parseEx.LineNumber - headerLength) + " , Position " + parseEx.LinePosition + ".";
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

            var cs = "using System.ComponentModel;\nusing HLab.Erp.Lims.Analysis.Module.TestClasses;\nusing HLab.Notify.PropertyChanged;\n" + Cs;

            if (cs.Contains("using FM;"))
            {
                cs = cs.Replace("using FM;", "");
                CsMessage += "using FM is obsolete !\r\n";
            }

            int index = cs.IndexOf("public class", StringComparison.InvariantCulture)+12;
            int idx2 = cs.IndexOf("\r\n",index);
            if(idx2<0) idx2 = cs.IndexOf("\n",index);

            var classname = cs.Substring(index,idx2-index);

            index = cs.IndexOf("public class", StringComparison.InvariantCulture);
            cs = cs.Insert(index, string.Format("using H = NotifyHelper<{0}>;\n",classname));


            // Ajout des dérivations de classes
            index = cs.IndexOf("public class", StringComparison.InvariantCulture) + 12;
            if (index > -1)
            {
                var i = cs.IndexOf("\r\n", index, StringComparison.InvariantCulture);
                if (i < 0) i = cs.IndexOf("\n", index, StringComparison.InvariantCulture);
                index = i;
            }


            cs = cs.Insert(index, " : UserControl, ITestForm, INotifyPropertyChanged");

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
                if (!String.IsNullOrEmpty(fe.Name))
                {
                    elements.Add(fe);
                    declarations += "public " + fe.GetType().Name + " " + fe.Name + ";\n";
                    connection += "case " + n + ": this." + fe.Name + " = ((" + fe.GetType().Name + ")(target)); return;\n";

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
            declarations += "public event PropertyChangedEventHandler PropertyChanged;";
            declarations += "public TestLegacyHelper Test => _test.Get();\nprivate IProperty<TestLegacyHelper> _test = H.Property<TestLegacyHelper>(c => c.Set(e => new TestLegacyHelper()));\nITestHelper ITestForm.Test => Test;\n";
            declarations += "public " + classname + "(){H.Initialize(this,a => PropertyChanged?.Invoke(this,a));}";
            cs = cs.Insert(cs.IndexOf('{', index) + 1, declarations + connection + "}\r\n}\r\n");

            var compiler = new Compiler { SourceCode = cs };

            if (!compiler.Compile())
            {
                Form = new DummyTestForm { Content = form };
                CsMessage = compiler.CsMessage;
                return;
            }

            var assemblyLoadContext = new AssemblyLoadContext("LimsForm", true);// SimpleUnloadableAssemblyLoadContext();

            Assembly assembly;
            using (var asm = new MemoryStream(compiler.Compiled))
            {
                assembly = assemblyLoadContext.LoadFromStream(asm);
            }
            // Création de l'instance de la classe venant du C#
            var module = (ITestForm)Activator.CreateInstance(assembly.GetTypes()[0]);

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
                            module.Traitement(sender, args);
                            SetFormMode(Mode);
                        };
                        break;

                    case TextBox textBox:
                        textBox.TextChanged += (sender, args) =>
                        {
                            module.Traitement(sender, args);
                            SetFormMode(Mode);
                        };
                        break;

                    case CheckBox checkBox:
                        if (checkBox.Name.Contains("__") && module.Test is TestLegacyHelper test)
                        {
                            var pos = checkBox.Name.LastIndexOf("__", StringComparison.InvariantCulture);
                            var name = checkBox.Name.Substring(0, pos);

                            if (!checkboxes.TryGetValue(name, out var chk))
                            {
                                chk = new List<CheckBox>();
                                checkboxes.Add(name, chk);
                            }

                            chk.Add(checkBox);

                            var t = test;
                            checkBox.PreviewMouseDown += (sender, args) =>
                            {
                                args.Handled = true;
                                t.CheckGroup(sender, chk.ToArray());
                                module.Traitement(sender, args);
                                SetFormMode(Mode);
                            };
                        }
                        else
                            checkBox.PreviewMouseDown += (sender, args) =>
                            {
                                args.Handled = true;
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
                    dict.Add(v[0],v[1]);
                }
                else if (v.Length == 1)
                {
                    dict.Add(v[0],"");
                }

            }

            LoadValues(dict);
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
            var size = text?.Length ?? 0;
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
        private static readonly Brush _specificationNeededBrush = Brushes.PaleGreen;
        private static readonly Brush _specificationDoneBrush = Brushes.DarkGreen;
        private static readonly Brush _mandatoryBrush = Brushes.PaleVioletRed;


        private bool SetFormMode(TestFormMode mode)
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

            var mandatoryBrush = isSpecMode ? _normalBrush : _mandatoryBrush;


            if (Form is FrameworkElement form)
            {
                foreach (var c in FindLogicalChildren<Control>(form))
                {
                    var tag = c.Tag?.ToString().ToLower()??"";

                    var spec = (tag.Contains("norme") || tag.Contains("spec"));
                    var mandatory = (tag.Contains("obligatoire") || tag.Contains("mandatory"));

                    var doneBrush = spec?_specificationDoneBrush:_normalBrush;;
                    var todoBrush = spec?_specificationNeededBrush:mandatory?mandatoryBrush:_normalBrush;


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

                        // TextBox
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

            if (isSpecMode && Test!=null) 
                Test.Values = GetSpecPackedValues();

            if (isCaptureMode && Result!=null) 
                Result.Values = GetPackedValues();

            if(mandatoryNeeded>0) Form.Test.State = mandatoryDone>0 ? TestState.Running : TestState.NotStarted;
            if(specificationNeeded>0) Form.Test.State = TestState.NotStarted;

            if(Form.Test.State>TestState.Running)
            {
                if (specificationNeeded > 0) Form.Test.State = TestState.NotStarted;
                if (mandatoryNeeded > 0) Form.Test.State = TestState.Running;
            }

            return mandatoryNeeded>0 || specificationNeeded>0;
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

                if (tag.Contains("Obligatoire") || tag.Contains("Mandatory"))
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

                if (!tag.Contains("Norme")) continue;
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


            TestState state;
            // Modifie l'état du test en fonction de son avancée
            if (nbMandatory > 0)
            {
                if (nbMandatoryOK == 0)
                {
                    if (dateDebut == DateTime.MinValue)
                        state = TestState.NotStarted;
                    else
                        state = TestState.NotStarted;
                }
                else if (nbMandatoryOK < nbMandatory)
                {
                    state = TestState.Running;

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

        public async Task LoadAsync(SampleTest test, SampleTestResult result = null)
        {
            Test = test;
            Result = result;

            await ExtractCode(test.Code).ConfigureAwait(true);
            await LoadForm().ConfigureAwait(true);
            
            LoadValues(test.Values);
            
            if (result!=null) 
                LoadValues(result.Values);
            
            test.Values = GetSpecPackedValues();

            if(result!=null)
                result.Values = GetPackedValues();

            Form.Traitement(null,null);
        }
    }
}
