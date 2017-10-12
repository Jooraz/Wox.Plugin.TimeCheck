using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Wox.Plugin.TimeCheck.Helpers;

namespace Wox.Plugin.TimeCheck.Tests
{
    [TestClass]
    public class TestTimeCheck
    {
        [TestMethod]
        public void Got424Zones()
        {
            var x = new TimeChecker();
            x.Init(null);

            Assert.IsTrue(x.Times != null && x.Times.Count == 424);
        }

        [TestMethod]
        public void TestDebouncer()
        {
            var debouncer = new Debouncer(TimeSpan.FromSeconds(1));

            var tempValue = 1;

            var testAction = new Action(() =>
            {
                tempValue++;
            });

            for(var i = 0; i < 5; i++)
            {
                Task.Run(() =>
                {
                    debouncer.AddAction(testAction);
                });
            }

            Assert.IsTrue(tempValue == 1);
        }

        [TestMethod]
        public void TestDebouncerWithDelay()
        {
            var milliseconds = 100;
            var debouncer = new Debouncer(TimeSpan.FromMilliseconds(milliseconds));

            var tempValue = 1;

            var testAction = new Action(() =>
            {
                tempValue++;
            });

            for (var i = 0; i < 5; i++)
            {
                Task.Delay(milliseconds).Wait();
                Task.Run(() =>
                {
                    debouncer.AddAction(testAction);
                });
            }

            Assert.IsTrue(tempValue == 5);
        }
    }
}
