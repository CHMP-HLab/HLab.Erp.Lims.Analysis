using HLab.Erp.Lims.Analysis.Module;
using Xunit;

namespace HLab.Erp.Analysis.UTests
{
    public class ProgressViewModelTest
    {
        [Fact]
        public void Test()
        {
            var vm = new ProgressViewModel();

            vm.Value = 1.0;
            Assert.Equal(100.0, vm.Percent);

            vm.Value = 0.2;
            Assert.Equal(20.0, vm.Percent);
        }
    }
}
