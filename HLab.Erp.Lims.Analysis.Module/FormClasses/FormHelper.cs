using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HLab.Compiler.Wpf;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{    using H = H<FormHelper>;

    public class FormHelper : NotifierBase
    {
        public FormHelper() => H.Initialize(this);

        public async Task LoadDefaultFormAsync()
        {
            Xaml = "<Grid></Grid>";
            Cs = @"using System;
            using System.Windows;
            using System.Windows.Controls;
            using FM;
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
        private readonly IProperty<IForm> _form = H.Property<IForm>();

        public ITestResultProvider Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }
        private readonly IProperty<ITestResultProvider> _result = H.Property<ITestResultProvider>();

        public string Xaml
        {
            get => _xaml.Get();
            set { if(_xaml.Set(value)) FormUpToDate = false; }
        }

        private readonly IProperty<string> _xaml = H.Property<string>();

        public string Cs
        {
            get => _cs.Get();
            set { if (_cs.Set(value)) FormUpToDate = false; }
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

        public ObservableCollection<CompileError> XamlErrors { get; } = new();
        public ObservableCollection<CompileError> CsErrors { get; } = new();
        public ObservableCollection<CompileError> DebugErrors { get; } = new();

        public CompileError SelectedXamlError
        {
            get => _selectedXamlError.Get();
            set => _selectedXamlError.Set(value);
        }
        private readonly IProperty<CompileError> _selectedXamlError = H.Property<CompileError>();

        public CompileError SelectedCsError
        {
            get => _selectedCsError.Get();
            set => _selectedCsError.Set(value);
        }
        private readonly IProperty<CompileError> _selectedCsError = H.Property<CompileError>();

        public CompileError SelectedDebugError
        {
            get => _selectedDebugError.Get();
            set => _selectedDebugError.Set(value);
        }
        private readonly IProperty<CompileError> _selectedDebugError = H.Property<CompileError>();

        public bool FormUpToDate
        {
            get => _formUpToDate.Get();
            set => _formUpToDate.Set(value);
        }
        private readonly IProperty<bool> _formUpToDate = H.Property<bool>();

        public int CsErrorPos
        {
            get => _csErrorPos.Get();
            set => _csErrorPos.Set(value);
        }
        private readonly IProperty<int> _csErrorPos = H.Property<int>();



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

            Provider = new SampleTestFormClassProvider(Xaml, Cs);
                
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

            Form = Provider.Create();

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

        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
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

                if (target?.SpecificationValues != null)
                    Form.LoadValues(target.SpecificationValues);

                if (target?.ResultValues != null)
                    Form.LoadValues(target.ResultValues);

                Form?.Process(null, new RoutedEventArgs());
            }
            finally
            {
                _lock.Release();
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
        
        public async Task CompileAsync()
        {
            var specs = Form?.Target?.SpecificationValues;
            var values = Form?.Target?.ResultValues;

            await LoadFormAsync(new DummyTarget()).ConfigureAwait(true);

            Form.LoadValues(specs);
            Form.LoadValues(values);

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

    }
}
