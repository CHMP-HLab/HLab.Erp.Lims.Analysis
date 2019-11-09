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

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    public interface ITestForm
    {
        void Connect(int connectionId, object target);
        void Traitement(object sender, RoutedEventArgs e);

        ITestHelper Test {get ;}
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

    public class FormHelper
    {
        public string Xaml { get; set; }
        public string Cs { get; set; }

        public string XamlMessage { get; private set; }
        public string CsMessage { get; private set; }
        public string Values { get; set; }

        public async Task ExtractCode(byte[] code)
        {
            var sCode = Encoding.UTF8.GetString(await GzipToBytes(code).ConfigureAwait(false));
            var index    = sCode.LastIndexOf("}\r\n",StringComparison.InvariantCulture);
            Cs  = sCode.Substring(0, index+1);
            Xaml         = sCode.Substring(index+3);
        }
        public async Task<byte[]> SaveCode()
        {
            var bytes = Encoding.UTF8.GetBytes(Cs.Trim('\r', '\n', ' ') + "\r\n" + Xaml.Trim('\r', '\n' , ' '));
            return await BytesToGZip(bytes);

        }

        public async Task<object> LoadForm(string values)
        {
            return await LoadForm(values.Split('■').ToList()); // Le séparateur est un ALT + 254
        }

        public async Task<object> LoadForm(List<string> values)
        {
            var form = (FrameworkElement)await LoadForm().ConfigureAwait(false);
            LoadValues(form,values);
            return form;
        }

        public async Task<ITestForm> LoadForm()
        {
            CsMessage = "";
            XamlMessage = "";

            var header = @"
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
/*
                    <ResourceDictionary Source = ""pack://application:,,,/MahApps.Metro;component/Styles/Themes/light.steel.xaml"" />          
 
 */
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
            //catch(XamlParseException ee)
            //{
            //   ee.LinePosition
            //}
            catch (Exception ex)
            {
                int headerLength = LineCount(header.Substring(0, header.IndexOf("<!--Content-->",StringComparison.InvariantCulture)));

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
                return null;
            }

            // Loading C#
            if (String.IsNullOrWhiteSpace(Cs))
            {
                CsMessage = "Code was empty";
                return null;
            }

            var cs = "using HLab.Erp.Lims.Analysis.Module.TestClasses;\r\n" + Cs;

            if (cs.Contains("using FM;"))
            {
                cs = cs.Replace("using FM;", "");
                CsMessage += "using FM is obsolete !\r\n";
            }

            // Ajout des dérivations de classes
            int index = cs.IndexOf("public class",StringComparison.InvariantCulture) + 12;
            if (index>-1)
            {
                var i = cs.IndexOf("\r\n", index,StringComparison.InvariantCulture);
                if(i<0) i = cs.IndexOf("\n", index,StringComparison.InvariantCulture);
                index = i;
            }


            cs = cs.Insert(index, " : UserControl, ITestForm");

            // Ajout des déclarations des objects du formulaire à lier à la classe et de la fonction Connect pour la liaison une fois instanciée
            String declarations = "";
            String connection = "\npublic void Connect(int connectionId, object target)\n{\nswitch (connectionId){\n";
            List<FrameworkElement> elements = new List<FrameworkElement>();
            int n = 0;
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

            var compiler = new Compiler{SourceCode = cs};
                
            if (!compiler.Compile())
            {
                var testForm = new DummyTestForm { Content = form };
                CsMessage = compiler.CsMessage;
                return testForm;
            }

            var assemblyLoadContext = new AssemblyLoadContext("LimsForm", true);// SimpleUnloadableAssemblyLoadContext();

            Assembly assembly;
            using (var asm = new MemoryStream(compiler.Compiled))
            {
                assembly = assemblyLoadContext.LoadFromStream(asm);
            }
            // Création de l'instance de la classe venant du C#
            ITestForm module = (ITestForm)Activator.CreateInstance(assembly.GetTypes()[0]);

            if(module is UserControl uc)
                uc.Content = form;

            var checkboxes = new Dictionary<string, List<CheckBox>>();

            // Liaisons entre les objets Xaml et C#
            for (int i = 0; i < n; i++)
            {
                module.Connect(i, elements[i]);

                if (elements[i] is TextBox textBox)
                {
                    textBox.TextChanged += delegate (object sender, TextChangedEventArgs args)
                    {
                        module.Traitement(sender, args);
                    };
                }

                if (elements[i] is TextBoxEx textBoxEx)
                {
                    textBoxEx.DoubleChange += delegate (object sender, RoutedEventArgs args)
                    {
                        module.Traitement(sender, args);
                    };
                }


                if (elements[i] is CheckBox checkbox)
                {
                    if(checkbox.Name.Contains("__"))
                    {
                        var pos = checkbox.Name.LastIndexOf("__");
                        var name = checkbox.Name.Substring(0, pos);

                        if(!checkboxes.TryGetValue(name,out var chk))
                        {
                            chk = new List<CheckBox>();
                            checkboxes.Add(name,chk);
                        }

                        chk.Add(checkbox);

                        checkbox.PreviewMouseDown += delegate (object sender, MouseButtonEventArgs e)
                        {
                            e.Handled = true;
                            if (module.Test is TestLegacyHelper test)
                            {
                                test.CheckGroupe(sender, chk.ToArray());
                            }

                            module.Traitement(sender, null);
                        };
                    }
                    else
                        checkbox.PreviewMouseDown += delegate (object sender, MouseButtonEventArgs e)
                        {
                            e.Handled = true;
                            module.Traitement(sender, null);
                        };
                }

                if (elements[i] is Button button)
                {
                    button.Click += delegate (object sender, RoutedEventArgs args)
                    {
                        module.Traitement(sender, null);
                    };

                }
            }

            CsMessage = "C# Ok";

            return module;
        }


        public string GetPackedValues(object form)
        {
            if(form is FrameworkElement fe)
            {
                var values = "";
                foreach(var c in FindLogicalChildren<Control>(fe))
                {
                    switch (c)
                    {
                        case TextBoxEx tbe:
                            values += c.Name + "=" + tbe.Double +"■";
                            break;
                        case TextBox tb:
                            values += c.Name + "=" + tb.Text.Replace("■", "") +"■"; // Le séparateur est un ALT + 254
                            break;
                        case CheckBox cb:
                            if(cb.Name.Contains("__"))
                            {
                                if(cb.IsChecked==true)
                                {
                                    values += c.Name.Replace("__", "=")+ "■";
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

        public void LoadValues(FrameworkElement form, [NotNull]string values)
        {
            LoadValues(form, values.Split((char)254).ToList()); // Le séparateur est un ALT + 254
        }

        public void LoadValues(FrameworkElement form, List<string> values)
        {
            foreach(var c in FindLogicalChildren<Control>(form))
            {
                var nom = c.Name + "=";
                if(c.Name.Contains("__") && c is CheckBox chk)
                {
                    var idx = c.Name.IndexOf("__");
                    nom = c.Name.Substring(0,idx)+"=";
                }
                foreach(var value in values)
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
                            if(c.Name.Contains("__"))
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

        // TODO : internationalize
        public static double CSD(string str)
        {
            str = str.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".",StringComparison.InvariantCulture);
            try
            {
                return Convert.ToDouble(str,CultureInfo.InvariantCulture);
            }
            catch(Exception)
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

    }
}
