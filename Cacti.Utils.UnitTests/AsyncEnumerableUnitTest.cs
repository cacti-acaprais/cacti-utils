using Cacti.Utils.AsyncUtil;
using Cacti.Utils.UnitTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cacti.Utils.ObservableUtil;

namespace Cacti.Utils.UnitTests
{
    [TestClass]
    public class AsyncEnumerableUnitTest
    {
        [TestMethod]
        public async Task CachedEnumerable()
        {
            const int TOTAL = 10;

            PaginatedContext<string> paginatedContext = new PaginatedContext<string>(TOTAL, (index) => $"index: {index}");
            PaginatedAsyncEnumerable<string> paginatedAsync = new PaginatedAsyncEnumerable<string>(3, paginatedContext);

            List<string> values = new List<string>();
            await paginatedAsync.ForEach((value) => values.Add(value), CancellationToken.None);

            Assert.AreEqual(TOTAL, values.Count);
        }

        [TestMethod]
        public async Task MapAsyncEnumerable()
        {
            const int TOTAL = 10;
            const string MAPPING_VALUE = "map";

            PaginatedContext<string> paginatedContext = new PaginatedContext<string>(TOTAL, (index) => $"index: {index}");
            PaginatedAsyncEnumerable<string> paginatedAsync = new PaginatedAsyncEnumerable<string>(3, paginatedContext);

            List<string> mappedValues = await paginatedAsync
                .Select((value) => $"{MAPPING_VALUE} : {value}")
                .ToListAsync(CancellationToken.None);

            Assert.IsTrue(mappedValues.TrueForAll(value => value.Contains(MAPPING_VALUE)));
        }

        [TestMethod]
        public async Task FilteredAsyncEnumerable()
        {
            const int TOTAL = 10;

            PaginatedContext<int> paginatedContext = new PaginatedContext<int>(TOTAL, (index) => index);
            PaginatedAsyncEnumerable<int> paginatedAsync = new PaginatedAsyncEnumerable<int>(3, paginatedContext);

            List<int> filteredValues = await paginatedAsync
                .Where(value => value % 2 > 0)
                .ToListAsync(CancellationToken.None);

            Assert.AreEqual(5, filteredValues.Count);
        }

        [TestMethod]
        public async Task AsyncEnumerableToObservable()
        {
            const int TOTAL = 10;

            PaginatedContext<int> paginatedContext = new PaginatedContext<int>(TOTAL, (index) => index);
            PaginatedAsyncEnumerable<int> paginatedAsync = new PaginatedAsyncEnumerable<int>(3, paginatedContext);

            List<string> readValues = new List<string>();

            await paginatedAsync
                .Where(value => value % 2 > 0)
                .Observe(new Observer<int>((value) => readValues.Add($"read : {value}")), CancellationToken.None);

            Assert.IsTrue(readValues.Count == 5);
        }
    }
}
