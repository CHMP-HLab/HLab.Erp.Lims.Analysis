using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using HLab.Base.Wpf;
using HLab.Compiler.Wpf;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public enum ElementLevel
    {
        Capture,
        Mandatory,
        Specification,
        Result
    }

    public readonly struct ElementInfo
    {
        public string Name { get; }
        public Type Type { get; }
        public ElementLevel Level{ get; }

        public ElementInfo(string name, Type type, ElementLevel level)
        {
            Name = name;
            Type = type;
            Level = level;
        }
    }
    public interface IFormClassProvider
    {
        IEnumerable<CompileError> CsErrors { get; }
        IEnumerable<CompileError> XamlErrors { get; }

        string ClassName { get; }
        string BaseClass { get; }

        Dictionary<string,ElementInfo> NamedElements { get; }

        IForm Create();
    }

    public class FormClassProvider : IFormClassProvider
    {
        private readonly Assembly _assembly;
        private readonly Type _type;
        private readonly string _xaml;

        public IEnumerable<CompileError> CsErrors { get; }
        public IEnumerable<CompileError> DebugErrors { get; }
        public IEnumerable<CompileError> XamlErrors { get; private set; }

        public string ClassName { get; private set; }
        public string BaseClass { get; private set; }

        public string FinalCode { get; }

        public Dictionary<string,ElementInfo> NamedElements { get; }

        public FormClassProvider(Assembly assembly, string xaml)
        {
            _assembly = assembly;
            _xaml = xaml;
        }

        public FormClassProvider(string xaml, string cs)
        {
            _xaml = xaml;
            var ui = GetXamlUi();
            NamedElements = GetNamedElements(ui);

            cs = PrepareCs(cs);


            _assembly = Compiler.Wpf.Compiler.Compile(out var messages, cs);
            if (_assembly != null)
            {
                _type = _assembly.GetTypes()[0];
            }
            else
            {
                CsErrors = messages.Select(TranslateCsError);
            }
#if DEBUG
            var parser = new CompilerCodeParser(cs);
            parser.FormatCode();
            FinalCode = parser.Code;

            var dummy = Compiler.Wpf.Compiler.Compile(out var m2, FinalCode);
            DebugErrors = m2;
#endif
        }

        private string PrepareCs(string cs)
        {
            var parser = new CompilerCodeParser(cs);
            var className = parser.NextClass();
            if (className == "") return cs;

            BaseClass = parser.BaseClass();
            if (string.IsNullOrWhiteSpace(BaseClass))
            {
                BaseClass = "TestLegacyForm";
                parser.Insert($" : {BaseClass}");
            }

            parser.BeforeClassContent();

            parser.Insert(",IForm");

            parser.ClassContent();

//            parser.Insert($"public {className}() => InitialiseComponents();");

            parser.Insert(BuildConnectFunction());
            parser.Insert(BuildGetPacketValuesFunction("GetPackedValues",ElementLevel.Capture, ElementLevel.Mandatory, ElementLevel.Result));
            parser.Insert(BuildGetPacketValuesFunction("GetSpecPackedValues",ElementLevel.Specification));

            parser.Insert(InsertFunctions(cs));

            #if DEBUG
            //parser.FormatCode();
            #endif

            return parser.Code;
        }

        protected virtual string InsertFunctions(string cs) { return "";}


        public IForm Create()
        {
            var form = _type != null ? (IForm)Activator.CreateInstance(_type) : new DummyForm();
            var ui = GetXamlUi();

            List<FrameworkElement> namedElements = new ();
            foreach(var element in NamedElements)
            {
                var obj = ui.FindName(element.Key);
                if(obj is FrameworkElement fe)
                    namedElements.Add(fe);
                else
                    throw new Exception("Element not found");

                form.NamedElements = namedElements;
            }

            if(form is UserControl e)
            {
                e.Content = ui;
            }

            return form;
        }



        private FrameworkElement GetXamlUi()
        {
            if(_xaml==null) return new TextBlock {Text = "xaml was null"};

            try
            {
                return (FrameworkElement)XamlReader.Parse(_xaml);
            }
            catch (XamlParseException ex)
            {
                var error = new CompileError
                {
                    Message = ex.Message,
                    Line = ex.LineNumber,
                    Pos = ex.LinePosition,
                };
                
                XamlErrors = new List<CompileError>{TranslateXamlError(error)};
                return new TextBlock {Text = error.Message};
            }
        }

        protected virtual CompileError TranslateXamlError(CompileError error) => error;
        protected virtual CompileError TranslateCsError(CompileError error) => error;


        private Dictionary<string,ElementInfo> GetNamedElements(FrameworkElement e) => e
            .FindLogicalChildren<FrameworkElement>()
            .Where(fe => !string.IsNullOrEmpty(fe.Name) && fe.Name!= "formulaContainerElement")
            .GroupBy(fe => fe.Name).Where(g => g.Count()==1).Select(g => g.First())
            .ToDictionary(fe => fe.Name, fe => new ElementInfo(fe.Name, fe.GetType(), GetElementLevel(fe)));

        protected ElementLevel GetElementLevel(FrameworkElement fe)
        {
            if (fe is TextBlock) return ElementLevel.Result;
            if (fe.Tag is string tag && !string.IsNullOrWhiteSpace(tag))
            {
                tag = tag.ToLower();
                var t = tag[0];
                if ("mo".Contains(t)) return ElementLevel.Mandatory;
                if ("sn".Contains(t)) return ElementLevel.Specification;
            }

            return  ElementLevel.Capture;
        }

        private Assembly GetAssemblyFromCs()
        {
            throw new NotImplementedException();
        }

        protected virtual Exception BuildException(Exception ex) => ex;

        protected static int LineCount(string text)
        {
            if (text == null) return 0;
            var size = text.Length;
            var nb = size == 0 ? 0 : 1;
            for (var i = 0; i < size; i++)
                if (text[i] == '\n')
                    nb++;
            return nb;
        }

        private string BuildConnectFunction()
        {
            StringBuilder sb = new();
            StringBuilder connect = new();

            connect.Append(
                "public void Connect(int connectionId, object target){"
                + "switch (connectionId){");

            var n = 0;
            foreach (var fe in NamedElements.Values)
            {
                sb.Append($"public {fe.Type.FullName} {fe.Name};");
                connect.Append($"case {n} : if(target is {fe.Type.FullName} {fe.Name}Target) this.{fe.Name} = {fe.Name}Target; return;");
                n++;
            }

            connect.Append("}}");
            sb.Append(connect);
            return sb.ToString();
        }

        protected string BuildGetPacketValuesFunction(string functionName,params ElementLevel[] levels)
        {
            StringBuilder sb = new();
            sb.Append($"public override string {functionName}()=>$\"");

            foreach (var e in NamedElements.Values.Where(e => levels.Contains(e.Level)))
            {
                sb.Append(AppendValue(e));
            }

            sb.Append("\";");
            return sb.ToString();
        }

        private static string AppendValue(ElementInfo element)
        {
            if(element.Type.IsAssignableTo(typeof(TextBlock)))
                    return $"{element.Name}={{Sanitize({element.Name}.Text)}}■";// Le séparateur est un ALT + 254

            if (element.Type.IsAssignableTo(typeof(IDoubleProvider)))
                    return $"{element.Name}={{{element.Name}.Double}}■";// Le séparateur est un ALT + 254

            if (element.Type.IsAssignableTo(typeof(TextBox)))
                    return $"{element.Name}={{Sanitize({element.Name}.Text)}}■";

            if (element.Type.IsAssignableTo(typeof(CheckBox)))
                if (element.Name.Contains("__"))
                {
                    var cbValue = element.Name.Replace("__", "=");

                    //CheckBox cb = new CheckBox();
                    //var test = $"{(cb.IsChecked==true?$"{cbValue}■":"")}";
                    return $"{{({element.Name}.IsChecked==true?$\"{cbValue}■\":\"\")}}";
                }
                else
                {
                    //CheckBox cb = new CheckBox();
                    //var test = $"={cb.IsChecked switch {null => 'N', false => '0', true => '1'}}■";
                    return $"{element.Name}={{{element.Name}.IsChecked switch {{null => 'N', false => '0', true => '1'}}}}■";
                }
            return "";
        }

    }
}
