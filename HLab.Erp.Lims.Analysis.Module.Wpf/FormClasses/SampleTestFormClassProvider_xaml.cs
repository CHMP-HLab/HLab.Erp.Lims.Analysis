using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Navigation;
using HLab.Compiler.Wpf;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses;

public partial class SampleTestFormClassProvider : FormClassProvider
{
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
            </UserControl >
            ";


    readonly int _headerLength = LineCount(XamlHeader.Substring(0, XamlHeader.IndexOf("<!--Content-->", StringComparison.InvariantCulture)))-1;

    protected override async Task<string> PrepareXamlAsync(string xaml)
    {
        await Task.Run(() =>
        {
            xaml = XamlHeader.Replace("<!--Content-->", xaml);

            xaml = ApplyLanguage(xaml);

            xaml = xaml
                    .Replace("\"Black\"", "\"{DynamicResource HLab.Brushes.Foreground}\"")
                    .Replace("\"White\"", "\"{DynamicResource MahApps.Brushes.ThemeBackground}\"")
                ;
        });
        return await base.PrepareXamlAsync(xaml);
    }

    //TODO : Legacy translation to be replaced
    static string ApplyLanguage(String text, string language = "")
    {
        // Choix de la langue
        if (language == "en")
            return Regex.Replace(Regex.Replace(text, @"\{FR=[\s|!-\|~-■]*}", ""), @"\{US=([\s|!-\|~-■]*)}", "$1"); // En anglais

        return Regex.Replace(Regex.Replace(text, @"\{US=[\s|!-\|~-■]*}", ""), @"\{FR=([\s|!-\|~-■]*)}", "$1"); // En français
    }

    protected override CompileError TranslateXamlError(CompileError error)
    {
        var errorLine = error.Line - _headerLength;
        var errorPos = error.Pos;

        var oldPos = $"Line {error.Line}, position {error.Pos}.";
        var newPos = $"Line {errorLine}, position {errorPos}.";

        var message = error.Message.Replace(oldPos, newPos);

        return base.TranslateXamlError(new CompileError
        {
            Message = message, 
            Line = errorLine, 
            Pos = errorPos, 
            Length = error.Length
        });
    }

}