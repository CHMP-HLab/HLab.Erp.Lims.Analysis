using HLab.Erp.Lims.Analysis.Module.AssayClasses;
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
            var cs = "using system;";

            h.Xaml = xaml;
            h.Cs = cs;

            var code = await  h.SaveCode();

            await h.ExtractCode(code);

            Assert.Equal(xaml, h.Xaml);
            Assert.Equal(cs, h.Cs);
        }
    }
}
