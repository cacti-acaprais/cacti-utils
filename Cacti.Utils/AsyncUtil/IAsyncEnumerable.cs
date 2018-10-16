using System;
using System.Collections.Generic;
using System.Text;

namespace Cacti.Utils.AsyncUtil
{
    //based on https://docs.microsoft.com/en-us/dotnet/api/microsoft.servicefabric.data.iasyncenumerable-1?view=azure-dotnet
    public interface IAsyncEnumerable<out T>
    {
        /// <summary>
        /// Returns an IAsyncEnumerator<T> that asynchronously iterates through the collection.
        /// </summary>
        IAsyncEnumerator<T> GetAsyncEnumerator();
    }
}
