using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Windows;
using System.Windows.Controls;
using ControlzEx.Standard;
using HLab.Base;
using HLab.Notify.PropertyChanged;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Outils;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    class Compiler
    {
        public string SourceCode { get; set; }
        public string CsMessage { get; private set; }
        public byte[] Compiled { get; private set; }

        public bool Compile()
        {
            var sourceCode = "using HLab.Base;\r\n" + SourceCode;

            using (var peStream = new MemoryStream())
            {
                var result = GenerateCode(sourceCode).Emit(peStream);

                if (!result.Success)
                {
                    CsMessage = "Compilation done with error.";

                    var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                    {
                        CsMessage += "\n" + String.Format("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    return false;
                }

                CsMessage = "C# OK";

                peStream.Seek(0, SeekOrigin.Begin);

                Compiled = peStream.ToArray();

                return true;
            }
        }

        private static CSharpCompilation GenerateCode(string sourceCode)
        {
            var codeString = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);


            var references = new MetadataReference[]
            {
                //System.Runtime
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                //System.Private.CoreLib
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                //PresentationCore
                MetadataReference.CreateFromFile(typeof(UIElement).Assembly.Location),
                //PresentationFramework
                MetadataReference.CreateFromFile(typeof(TextBox).Assembly.Location),
                //WindowBase
                MetadataReference.CreateFromFile(typeof(DependencyObject).Assembly.Location),
                //HLab.Base.Wpf
                MetadataReference.CreateFromFile(typeof(TextBoxEx).Assembly.Location),
                //HLab.Erp.Lims.Analysis.Module
                MetadataReference.CreateFromFile(typeof(ITestForm).Assembly.Location),
                //System.ObjectModel
                MetadataReference.CreateFromFile(typeof(INotifyPropertyChanged).Assembly.Location),
                //HLab.Notify.PropertyChanged
                MetadataReference.CreateFromFile(typeof(N<>).Assembly.Location),
                //
                MetadataReference.CreateFromFile( Assembly.GetExecutingAssembly().Location),
                //MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Core").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Xaml").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.ComponentModel.Primitives").Location)
            };

            return CSharpCompilation.Create("Hello.dll",
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }

        public void Execute(byte[] compiledAssembly, string[] args)
        {
            var assemblyLoadContextWeakRef = LoadAndExecute(compiledAssembly, args);

            for (var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Console.WriteLine(assemblyLoadContextWeakRef.IsAlive ? "Unloading failed!" : "Unloading success!");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference LoadAndExecute(byte[] compiledAssembly, string[] args)
        {
            using (var asm = new MemoryStream(compiledAssembly))
            {
                var assemblyLoadContext = new AssemblyLoadContext("LimsForm",true);// SimpleUnloadableAssemblyLoadContext();

                var assembly = assemblyLoadContext.LoadFromStream(asm);

                var entry = assembly.EntryPoint;

                _ = entry != null && entry.GetParameters().Length > 0
                    ? entry.Invoke(null, new object[] { args })
                    : entry.Invoke(null, null);

                assemblyLoadContext.Unload();

                return new WeakReference(assemblyLoadContext);
            }
        }
    }
}

