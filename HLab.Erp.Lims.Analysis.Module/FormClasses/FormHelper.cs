﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

using HLab.Compiler.Wpf;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Notify.PropertyChanged;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses;

using H = H<FormHelper>;
public static class Extensions
{
    public static XElement IgnoreNamespace(this XElement xElem)
    {
        XNamespace xmlns = "";
        var name = xmlns + xElem.Name.LocalName;
        return new XElement(name,
            from e in xElem.Elements()
            select e.IgnoreNamespace(),
            xElem.Attributes()
        );
    }
    public static XNode StripNamespaces(this XNode n)
    {
        var xe = n as XElement;
        if(xe == null)
            return n;
        var contents = 
            // add in all attributes there were on the original
            xe.Attributes()
                // eliminate the default namespace declaration
                .Where(xa => xa.Name.LocalName != "xmlns")
                .Cast<object>()
                // add in all other element children (nodes and elements, not just elements)
                .Concat(xe.Nodes().Select(node => node.StripNamespaces()).Cast<object>()).ToArray();
        var result = new XElement(XNamespace.None + xe.Name.LocalName, contents);
        return result;

    }

#if !LINQPAD
    public static T Dump<T>(this T t, string description = null)
    {
        if(description != null)
            Console.WriteLine(description);
        Console.WriteLine(t);
        return t;
    }
#endif
}
public class FormHelper : NotifierBase
{
    public FormHelper() => H.Initialize(this);

    public async Task LoadDefaultFormAsync()
    {
        Xaml = "<Grid></Grid>";
        Cs = @"using System;
            using System.Windows;
            using System.Windows.Controls;
            namespace Lims
            {
                public class TestIdentification
                {
                    public void Process(object sender, RoutedEventArgs e)
                    {

                        Test.Description = ""Denomination"";
                        Test.Norme       = ""Specification"";
                        Test.Resultat    = ""Resultat"";
                    }  
                }
            }";
        await CompileAsync().ConfigureAwait(false);
    }

    public IForm Form
    {
        get => _form.Get();
        set => _form.Set(value);
    }

    readonly IProperty<IForm> _form = H.Property<IForm>();

    public ITestResultProvider Result
    {
        get => _result.Get();
        set => _result.Set(value);
    }

    readonly IProperty<ITestResultProvider> _result = H.Property<ITestResultProvider>();

    public string Xaml
    {
        get => _xaml.Get();
        set { if(_xaml.Set(value)) FormUpToDate = false; }
    }
    readonly IProperty<string> _xaml = H.Property<string>();

    public string Cs
    {
        get => _cs.Get();
        set { if (_cs.Set(value)) FormUpToDate = false; }
    }
    readonly IProperty<string> _cs = H.Property<string>();
    public byte[] Binary
    {
        get => _binary.Get();
        set => _binary.Set(value);
    }
    readonly IProperty<byte[]> _binary = H.Property<byte[]>();

    public string XamlMessage
    {
        get => _xamlMessage.Get();
        set => _xamlMessage.Set(value);
    }

    readonly IProperty<string> _xamlMessage = H.Property<string>();

    public string CsMessage
    {
        get => _csMessage.Get();
        set => _csMessage.Set(value);
    }

    readonly IProperty<string> _csMessage = H.Property<string>();

    public ObservableCollection<CompileError> XamlErrors { get; } = new();
    public ObservableCollection<CompileError> CsErrors { get; } = new();
    public ObservableCollection<CompileError> DebugErrors { get; } = new();

    public CompileError SelectedXamlError
    {
        get => _selectedXamlError.Get();
        set => _selectedXamlError.Set(value);
    }

    readonly IProperty<CompileError> _selectedXamlError = H.Property<CompileError>();

    public CompileError SelectedCsError
    {
        get => _selectedCsError.Get();
        set => _selectedCsError.Set(value);
    }

    readonly IProperty<CompileError> _selectedCsError = H.Property<CompileError>();

    public CompileError SelectedDebugError
    {
        get => _selectedDebugError.Get();
        set => _selectedDebugError.Set(value);
    }
    readonly IProperty<CompileError> _selectedDebugError = H.Property<CompileError>();

    public bool FormUpToDate
    {
        get => _formUpToDate.Get();
        set => _formUpToDate.Set(value);
    }
    readonly IProperty<bool> _formUpToDate = H.Property<bool>();

    public int CsErrorPos
    {
        get => _csErrorPos.Get();
        set => _csErrorPos.Set(value);
    }
    readonly IProperty<int> _csErrorPos = H.Property<int>();

    public SampleTestFormClassProvider Provider { get; private set; }

    public async Task LoadFormAsync(IFormTarget target)
    {
        CsMessage = "";
        XamlMessage = "";

        // Loading C#
        if (String.IsNullOrWhiteSpace(Cs))
        {
            CsMessage = "Code was empty";
            return;
        }

        Provider = new SampleTestFormClassProvider();
        await Provider.BuildXamlAsync(Xaml);

        Binary ??= await Provider.BuildCsAsync(Cs);

        if(Binary!=null)
            await Provider.LoadAsync(Binary);
                
        XamlErrors.Clear();
        CsErrors.Clear();
        DebugErrors.Clear();

        if (Provider.XamlErrors != null)
            foreach (var e in Provider.XamlErrors)
                XamlErrors.Add(e);

        if (Provider.CsErrors != null)
            foreach (var e in Provider.CsErrors)
                CsErrors.Add(e);

        if (Provider.DebugErrors != null)
            foreach (var e in Provider.DebugErrors)
                DebugErrors.Add(e);

        Form = await Provider.CreateAsync();

        Form.Target = target;

        FormUpToDate = true;
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

    public async Task ExtractCodeAsync(byte[] code)
    {
        if(code==null) return;

        var sCode = Encoding.UTF8.GetString(await GzipToBytes(code).ConfigureAwait(false));
        var index = sCode.LastIndexOf("}\r\n", StringComparison.InvariantCulture);
        Cs = sCode.Substring(0, index + 1);
        Xaml = sCode.Substring(index + 3);
    }

    public async Task<byte[]> PackCodeAsync()
    {
        var bytes = Encoding.UTF8.GetBytes(Cs.Trim('\r', '\n', ' ') + "\r\n" + Xaml.Trim('\r', '\n', ' '));
        return await BytesToGZip(bytes);
    }

    readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    public async Task LoadAsync(IFormTarget target)
    {
        await _lock.WaitAsync();
        try
        {
            if (!ReferenceEquals(Form?.Target, target))
            {
                //if (Form?.Target != null) throw new Exception("Target should be null or same");
                //Form.Target = target;
                await ExtractCodeAsync(target.Code).ConfigureAwait(true);

                await LoadFormAsync(target).ConfigureAwait(true);
            }

            Form.PreventProcess();

            if (target?.SpecificationValues != null)
                Form.LoadValues(target.SpecificationValues);

            if (target?.ResultValues != null)
                Form.LoadValues(target.ResultValues);

            Form.AllowProcess();

            Form?.TryProcess(null, new RoutedEventArgs());
        }
        finally
        {
            _lock.Release();
        }
    }


    static async Task<byte[]> GzipToBytes(object gz)
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

    static async Task<byte[]> BytesToGZip(byte[] bytes)
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
        
    public async Task CompileAsync()
    {
        var specs = Form?.Target?.SpecificationValues;
        var values = Form?.Target?.ResultValues;

        await LoadFormAsync(new DummyTarget()).ConfigureAwait(true);

        Form.PreventProcess();

        Form.LoadValues(specs);
        Form.LoadValues(values);

        Form.AllowProcess();

        try
        {
            Form.Process(null, null);
            FormUpToDate = true;
        }
        catch (Exception ex)
        {
            CsMessage += ex.Message;
        }
    }

    const string XamlHeader = @"
        <UserControl 
            xmlns = ""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            xmlns:x = ""http://schemas.microsoft.com/winfx/2006/xaml""
            xmlns:mc = ""http://schemas.openxmlformats.org/markup-compatibility/2006""
            xmlns:d = ""http://schemas.microsoft.com/expression/blend/2008""
            xmlns:o = ""clr-namespace:HLab.Base.Wpf;assembly=HLab.Base.Wpf""
            xmlns:lang=""clr-namespace:HLab.Localization.Wpf.Lang;assembly=HLab.Localization.Wpf""
            xmlns:math=""clr-namespace:WpfMath.Controls;assembly=WpfMath""
            UseLayoutRounding = ""False"" >

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
        </UserControl>";
        
    public async Task FormatAsync()
    {
        var tree = CSharpSyntaxTree.ParseText(Cs);
        var root = tree.GetRoot().NormalizeWhitespace();
        Cs = root.ToFullString();

        try
        {
            var doc = XDocument.Parse(XamlHeader.Replace("<!--Content-->",$"{Xaml}"));
            var r = ((XElement)doc.FirstNode).FirstNode.NextNode;
            r = ((XElement)r).FirstNode.NextNode;


            var xaml = ((XElement)r).ToString();

            xaml = xaml.Replace(" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"","");
            xaml = xaml.Replace(" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"","");
            xaml = xaml.Replace(" xmlns:o=\"clr-namespace:HLab.Base.Wpf;assembly=HLab.Base.Wpf\"","");
            xaml = xaml.Replace(" xmlns:lang=\"clr-namespace:HLab.Localization.Wpf.Lang;assembly=HLab.Localization.Wpf\"","");
            xaml = xaml.Replace(" xmlns:math=\"clr-namespace:WpfMath.Controls;assembly=WpfMath\"","");

            Xaml = xaml;
        }
        catch (Exception)
        {
            // Handle and throw if fatal exception here; don't just ignore them
        }
    }

}