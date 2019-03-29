using Cacti.Utils.ProcessUtil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.UnitTests
{
    [TestClass]
    public class ProcessUnitTest
    {
        [TestMethod]
        public async Task ExecuteAnActivity()
        {
            var activity = new Activity<string, int>(data => data.ToString());
            string result = await activity.Execute(5, CancellationToken.None);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public async Task ExecuteAProcess()
        {
            var intToStringActivity = new Activity<string, int>(data => data.ToString());
            var stringToIntActivity = new Activity<int, string>(data => int.Parse(data));

            var process = intToStringActivity
                .Then(stringToIntActivity);

            int result = await process.Execute(5, CancellationToken.None);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public async Task WhileActivity()
        {
            var incrementActivity = new Activity<int, int>(data => ++data);
            var process = incrementActivity.While(
                predicate: data => data < 5,
                accumulator: incrementActivity.SetLastResultToDataAccumulator());

            int result = await process.Execute(1, CancellationToken.None);

            Assert.AreEqual(5, result);
        }
    }
}
