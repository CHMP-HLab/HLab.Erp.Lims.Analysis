#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using HLab.Base.Wpf;
using HLab.Compiler.Wpf;


namespace HLab.Erp.Lims.Analysis.Module.FormClasses;

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
    public ElementLevel Level { get; }

    public ElementInfo(string name, Type type, ElementLevel level)
    {
        Name = name;
        Type = type;
        Level = level;
    }
}
public interface IFormClassProvider
{
    IEnumerable<CompileError>? CsErrors { get; }
    IEnumerable<CompileError>? XamlErrors { get; }

    string? ClassName { get; }
    string? BaseClass { get; }

    Dictionary<string, ElementInfo>? NamedElements { get; }

    Task<IForm> CreateAsync();
}

public class FormClassProvider : IFormClassProvider
{
    Assembly? _assembly;
    Type? _type;
    string _xaml = string.Empty;

    public IEnumerable<CompileError>? CsErrors { get; private set; }
    public IEnumerable<CompileError>? DebugErrors { get; private set; }
    public IEnumerable<CompileError>? XamlErrors { get; private set; }

    public string? ClassName { get; private set; }
    public string? BaseClass { get; private set; }
    public string? FinalCode { get; private set; }

    public Dictionary<string, ElementInfo>? NamedElements { get; private set; }


    public long CompileDuration { get; private set; }

    public async Task BuildXamlAsync(string xaml)
    {
        _xaml = await PrepareXamlAsync(xaml);
        var ui = GetXamlUi();
        NamedElements = GetNamedElements(ui);
    }

    public async Task<byte[]> BuildCsAsync(string cs)
    {
        cs = await PrepareCsAsync(cs);
        _watch.Reset();
        _watch.Start();

        List<CompileError> messages = new();

        var binary = await Compiler.Wpf.Compiler.CompileAsync(messages, cs);

        CsErrors = messages.Select(TranslateCsError);

        _watch.Stop();
        CompileDuration = _watch.ElapsedMilliseconds;
        _watch.Reset();

#if DEBUG
        var parser = new CompilerCodeParser(cs);
        parser.FormatCode();
        FinalCode = parser.Code;

        List<CompileError> m2 = new();
        var dummy = await Compiler.Wpf.Compiler.CompileAsync(m2, FinalCode);
        DebugErrors = m2;
#endif

        return binary;
    }

    public async Task LoadAsync(byte[] binary)
    {
        await Task.Run(() =>
        {
            _assembly = Compiler.Wpf.Compiler.Load(binary);
            if (_assembly != null)
            {
                _type = _assembly.GetTypes()[0];
            }
        });
    }

    protected async virtual Task<string> PrepareXamlAsync(string xaml)
    {
        return xaml;
    }

    protected virtual async Task<string> PrepareCsAsync(string cs)
    {
        var parser = new CompilerCodeParser(cs);
        var className = parser.NextClass();
        if (className == "") return cs;

        await Task.Run(() =>
        {

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

            parser.Insert(BuildConnectFunction);
            parser.Insert(sb => BuildGetPacketValuesFunction(sb, "GetPackedValues", ElementLevel.Capture, ElementLevel.Mandatory, ElementLevel.Result));
            parser.Insert(sb => BuildGetPacketValuesFunction(sb, "GetSpecPackedValues", ElementLevel.Specification));

            parser.Insert(InsertFunctions(cs));

    #if DEBUG
            //parser.FormatCode();
    #endif

        });

        return parser.Code;
    }

    protected virtual string InsertFunctions(string cs) { return ""; }

    readonly Stopwatch _watch = new();

    public async Task<IForm> CreateAsync()
    {
        //Task.Factory.StartNew();
        _watch.Reset();
        _watch.Start();

        IForm? form = null;
        try
        {
            if(_type == null) form = new DummyForm("Dummy form");
            else
            {
                var f = Activator.CreateInstance(_type) as IForm;
                form = f ?? new DummyForm($"Unable to create {_type.Name}");
            }

            var ui = GetXamlUi();

            if (NamedElements != null)
            {
                List<FrameworkElement> namedElements = new();

                foreach (var obj in NamedElements.Select(element => ui.FindName(element.Key)))
                {
                    if (obj is FrameworkElement fe)
                        namedElements.Add(fe);
                    else
                        throw new Exception("Element not found");
                }

                form.NamedElements = namedElements;
            }


            if (form is UserControl e)
            {
                e.Content = ui;
            }

        }
        finally
        {
            _watch.Stop();
            if (form != null)
                form.CreationDuration = _watch.ElapsedMilliseconds;
        }
        return form;

    }


    FrameworkElement GetXamlUi()
    {
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

            XamlErrors = new List<CompileError> { TranslateXamlError(error) };
            return new TextBlock { Text = error.Message };
        }
    }

    protected virtual CompileError TranslateXamlError(CompileError error) => error;
    protected virtual CompileError TranslateCsError(CompileError error) => error;


    Dictionary<string, ElementInfo> GetNamedElements(FrameworkElement e) =>
        e
            .FindLogicalChildren<FrameworkElement>()
            .Where(fe => !string.IsNullOrEmpty(fe.Name) && fe.Name != "formulaContainerElement")
            .GroupBy(fe => fe.Name)
            .Where(g => g.Count() == 1)
            .Select(g => g.First())
            .ToDictionary(fe => fe.Name,fe => new ElementInfo(fe.Name, fe.GetType(), GetElementLevel(fe)));

    protected ElementLevel GetElementLevel(FrameworkElement fe)
    {
        // TextBlock are not editable and must contain Result value
        if (fe is TextBlock) return ElementLevel.Result;

        //If no tag specified it must be capture field 
        if (fe.Tag is not string tag || string.IsNullOrWhiteSpace(tag)) return ElementLevel.Capture;

        tag = tag.ToLower();

        return tag[0] switch
        {
            // m for mandatory or o for 'obligatoire' in french
            'm' or 'o' => ElementLevel.Mandatory,
            // s for specification or n for 'norme' in french
            's' or 'n' => ElementLevel.Specification,
            _ => ElementLevel.Capture,
        };
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

    void BuildConnectFunction(StringBuilder sb)
    {
        StringBuilder connect = new();

        connect.Append(
            "public void Connect(int connectionId, object target){"
            + "switch (connectionId){");

        if (NamedElements != null)
        {
            var n = 0;
            foreach (var fe in NamedElements.Values)
            {
                sb.Append($"public {fe.Type.FullName} {fe.Name};");
                connect.Append($"case {n} : if(target is {fe.Type.FullName} {fe.Name}Target) this.{fe.Name} = {fe.Name}Target; return;");
                n++;
            }
        }

        connect.Append("}}");
        sb.Append(connect);
    }

    protected void BuildGetPacketValuesFunction(StringBuilder sb, string functionName, params ElementLevel[] levels)
    {
        sb.Append($"public override string {functionName}()=>$\"");

        foreach (var e in NamedElements.Values.Where(e => levels.Contains(e.Level)))
        {
            sb.Append(AppendValue(e));
        }

        sb.Append("\";");
    }

    static string AppendValue(ElementInfo element)
    {
        if (element.Type.IsAssignableTo(typeof(TextBlock)))
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