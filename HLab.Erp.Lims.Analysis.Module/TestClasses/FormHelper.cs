using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Google.Protobuf.WellKnownTypes;
using HLab.Base;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Notify.PropertyChanged;
using Enum = Google.Protobuf.WellKnownTypes.Enum;

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
        public SampleTestResult Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }
        private readonly IProperty<SampleTestResult> _result = H.Property<SampleTestResult>();

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
            xaml = ApplyLanguage(xaml);

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

            var cs = "using HLab.Erp.Lims.Analysis.Module.TestClasses;\r\n" + Cs;

            if (cs.Contains("using FM;"))
            {
                cs = cs.Replace("using FM;", "");
                CsMessage += "using FM is obsolete !\r\n";
            }

            // Ajout des dérivations de classes
            int index = cs.IndexOf("public class", StringComparison.InvariantCulture) + 12;
            if (index > -1)
            {
                var i = cs.IndexOf("\r\n", index, StringComparison.InvariantCulture);
                if (i < 0) i = cs.IndexOf("\n", index, StringComparison.InvariantCulture);
                index = i;
            }


            cs = cs.Insert(index, " : UserControl, ITestForm");

            // Ajout des déclarations des objects du formulaire à lier à la classe et de la fonction Connect pour la liaison une fois instanciée
            string declarations = "";
            var connection = "\npublic void Connect(int connectionId, object target)\n{\nswitch (connectionId){\n";
            var elements = new List<FrameworkElement>();
            var n = 0;
            foreach (FrameworkElement fe in FindLogicalChildren<FrameworkElement>(form))
            {
                if (!String.IsNullOrEmpty(fe.Name))
                {
                    elements.Add(fe);
                    //declarations += "internal " + fe.GetType().Name + " " + fe.Name +";\r\n";
                    declarations += "public " + fe.GetType().Name + " " + fe.Name + ";\n";
                    connection += "case " + n + ": this." + fe.Name + " = ((" + fe.GetType().Name +
                                  ")(target)); return;\n";
                    n++;
                }
            }

            declarations += "public TestLegacyHelper Test {get;} = new TestLegacyHelper();\nITestHelper ITestForm.Test => Test;\n";

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

                            checkBox.PreviewMouseDown += (sender, args) =>
                            {
                                args.Handled = true;
                                test.CheckGroup(sender, chk.ToArray());
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


        public string GetPackedValues(TestValueLevel level)
        {
            if (Form is FrameworkElement form)
            {
                var values = "";
                foreach (var c in FindLogicalChildren<Control>(form))
                {
                    var valueLevel = TestValueLevel.Result;
                    if(c.Tag is string tag)
                    {
                        if (tag.Contains("spec") || tag.Contains("norme")) valueLevel = TestValueLevel.Test;
                    }


                    if(valueLevel != level) continue;

                    switch (c)
                    {
                        case TextBoxEx tbe:
                            values += c.Name + "=" + tbe.Double + "■";
                            break;
                        case TextBox tb:
                            values += c.Name + "=" + tb.Text.Replace("■", "") + "■"; // Le séparateur est un ALT + 254
                            break;
                        case CheckBox cb:
                            if (cb.Name.Contains("__"))
                            {
                                if (cb.IsChecked == true)
                                {
                                    values += c.Name.Replace("__", "=") + "■";
                                }
                            }
                            else
                            {
                                values +=
                                    cb.IsChecked switch
                                    {
                                        null => (c.Name + "=N■"),
                                        false => (c.Name + "=0■"),
                                        true => (c.Name + "=1■")
                                    };
                            }

                            break;
                    }
                }

                return values;
            }

            return "";
        }

        public void LoadValues([NotNull]string values)
        {
            LoadValues(values.Split('■').ToList()); // Le séparateur est un ALT + 254
        }

        public void LoadValues(List<string> values)
        {
            if (Form is FrameworkElement form)
            {
                foreach (var c in FindLogicalChildren<Control>(form))
                {
                    var nom = c.Name + "=";
                    if (c.Name.Contains("__") && c is CheckBox chk)
                    {
                        var idx = c.Name.IndexOf("__");
                        nom = c.Name.Substring(0, idx) + "=";
                    }

                    foreach (var value in values)
                    {
                        if (value.Length < nom.Length || value.Substring(0, nom.Length) != nom) continue;
                        switch (c)
                        {
                            case TextBoxEx tbe:
                                tbe.Double = CSD(value.Substring(nom.Length));
                                break;
                            case TextBox tb:
                                tb.Text = value.Substring(nom.Length);
                                break;
                            case CheckBox cb:
                                if (c.Name.Contains("__"))
                                {
                                    cb.IsChecked = (c.Name == nom.Replace("=", "__") + value.Substring(nom.Length));
                                }
                                else
                                    cb.IsChecked = value.Substring(nom.Length) switch
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


        public static string ApplyLanguage(String text, string language = "")
        {
            // Choix de la langue
            if (language == "en")
                return Regex.Replace(Regex.Replace(text, @"\{FR=[\s|!-\|~-■]*}", ""), @"\{US=([\s|!-\|~-■]*)}", "$1"); // En anglais

            return Regex.Replace(Regex.Replace(text, @"\{US=[\s|!-\|~-■]*}", ""), @"\{FR=([\s|!-\|~-■]*)}", "$1"); // En français
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
                await using (MemoryStream ms = new MemoryStream(bytes))
                {
                    await using var gz = new MemoryStream();
                    await using var zipStream = new GZipStream(gz, CompressionMode.Compress);
                    await zipStream.WriteAsync(ms.ToArray(), 0, ms.ToArray().Length);
                    zipStream.Close();
                    return gz.ToArray();
                }
            }
            catch { }

            return null;
        }


        public bool SetFormMode(TestFormMode mode)
        {

            Brush normalBrush = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF));
            Brush specificationNeededBrush = Brushes.PaleGreen;
            Brush specificationDoneBrush = Brushes.DarkGreen;
            Brush mandatoryBrush = Brushes.PaleVioletRed;

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
                    var tag = c.Tag?.ToString()??"";

                    var spec = (tag.Contains("Norme") || tag.Contains("Specification"));
                    var mandatory = (tag.Contains("Obligatoire") || tag.Contains("Mandatory"));

                    var doneBrush = spec?specificationDoneBrush:normalBrush;
                    var todoBrush = spec?specificationNeededBrush:mandatory?mandatoryBrush:normalBrush;

                    Action todo;
                    if (spec) todo = () => specificationNeeded++;
                    else if(mandatory) todo = () => mandatoryNeeded++;
                    else todo = () => optionalEmpty++;

                    Action done;
                    if (spec) done = () => specificationDone++;
                    else if(mandatory) done = () => mandatoryDone++;
                    else done = () => optionalDone++;

                    var enabled =
                        (spec && mode == TestFormMode.Specification)
                        || (!spec && mode == TestFormMode.Capture);

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
            await ExtractCode(test.Code).ConfigureAwait(true);
            await LoadForm().ConfigureAwait(true);
            
            LoadValues(test.Values);
            
            if (result!=null) 
                LoadValues(result.Values);
            
            test.Values = GetPackedValues(TestValueLevel.Test);

            if(result!=null)
                result.Values = GetPackedValues(TestValueLevel.Result);

            Form.Traitement(null,null);
        }
    }
}
