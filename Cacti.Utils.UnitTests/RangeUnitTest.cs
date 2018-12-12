using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cacti.Utils.UnitTests
{
    [TestClass]
    public class RangeUnitTest
    {
        [TestMethod]
        public void CreatePeriod()
        {
            Period period = new Period(DateTime.Now, DateTime.Now);

            Assert.IsNotNull(period);
        }

        [TestMethod]
        public void InvalidCreatePeriod()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                new Period(DateTime.Now.AddHours(1), DateTime.Now);
            });
        }

        [TestMethod]
        public void ContainPeriod()
        {
            Period smallPeriod = new Period(DateTime.Now.AddHours(-1), DateTime.Now.AddHours(1));
            Period bigPeriod = new Period(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));

            bool contains = bigPeriod.Contains(smallPeriod);

            Assert.IsTrue(contains);
        }

        [TestMethod]
        public void ContainSamePeriod()
        {
            Period period = new Period(DateTime.Now, DateTime.Now);
            bool contains = period.Contains(period);

            Assert.IsTrue(contains);
        }

        [TestMethod]
        public void NotContainsPeriod()
        {
            Period smallPeriod = new Period(DateTime.Now.AddHours(-1), DateTime.Now.AddHours(1));
            Period bigPeriod = new Period(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));

            bool contains = smallPeriod.Contains(bigPeriod);

            Assert.IsFalse(contains);
        }

        [TestMethod]
        public void NotContainsOvelapPeriod()
        {
            Period smallPeriod = new Period(DateTime.Now, DateTime.Now.AddHours(2));
            Period bigPeriod = new Period(DateTime.Now.AddHours(-1), DateTime.Now.AddHours(1));

            bool contains = bigPeriod.Contains(smallPeriod);

            Assert.IsFalse(contains);
        }

        [TestMethod]
        public void IntersectPeriod()
        {
            Period smallPeriod = new Period(DateTime.Now, DateTime.Now.AddHours(2));
            Period bigPeriod = new Period(DateTime.Now.AddHours(-1), DateTime.Now.AddHours(1));

            bool contains = smallPeriod.Intersect(bigPeriod);

            Assert.IsTrue(contains);
        }
    }
}
