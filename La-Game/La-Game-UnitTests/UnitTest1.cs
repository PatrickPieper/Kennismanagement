using La_Game.Services;
using System;
using Xunit;

namespace La_Game_UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void TestMethod1()
        {
            DashboardService service = new DashboardService();
            service.CreateLiveTableViewModel(44);

            int i = 5;

            Assert.Equal(5, i);
        }
    }
}
