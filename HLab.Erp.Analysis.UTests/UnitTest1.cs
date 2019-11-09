using HLab.Erp.Lims.Analysis.Module.TestClasses;
using System;
using Xunit;

namespace HLab.Erp.Analysis.UTests
{
    public class FormsTests
    {
        [Fact]
        public async void FormStorage()
        {
            var h = new FormHelper();

            var xaml = "<Grid></Grid>";
            var cs = "using system; class test{}  ";

            h.Xaml = xaml;
            h.Cs = cs;

            var code = await  h.SaveCode();

            await h.ExtractCode(code);

            Assert.Equal(xaml.Trim('\n','\r',' '), h.Xaml.Trim('\n','\r',' '));
            Assert.Equal(cs.Trim('\n','\r',' '), h.Cs.Trim('\n','\r',' '));
        }
    }
}
