using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cacti.Utils.ObservableUtil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Cacti.Utils.JobUtil;
using System.Threading;

namespace Cacti.Utils.UnitTests
{
    [TestClass]
    public class ObservableUnitTest
    {
        [TestMethod]
        public void ObserveArray()
        {
            string[] values = new string[] {
                "test1",
                "test2",
                "test3"
            };

            List<string> readValues = new List<string>();
            IObserver<string> observer = new Observer<string>((value) => readValues.Add(value));
            IObservable<string> observable = values.ToObservable();

            //the processing is synchronous, so there's no need to unsubscribe.
            observable.Subscribe(observer);
            
            Assert.IsTrue(values.All(value => readValues.Contains(value)));
        }

        [TestMethod]
        public async Task JobObservableTest()
        {
            int index = 0;
            CancellationTokenSource tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            (IJob job, IObservable<int> observable) = JobObservable.Get(() => index++);

            List<string> readValues = new List<string>();

            using (observable.Subscribe(new Observer<int>(value => readValues.Add($"read {value}"))))
            {
                await job.Repeat(TimeSpan.FromMilliseconds(200))
                    .Handle<TaskCanceledException>()
                    .Execute(tokenSource.Token);   
            }

            Assert.IsTrue(readValues.Count >= 5);
        }
    }
}
