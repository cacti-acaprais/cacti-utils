using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cacti.Utils.ObservableUtil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
    }
}
