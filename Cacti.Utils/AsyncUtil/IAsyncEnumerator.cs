using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.AsyncUtil
{
    /// <summary>
    /// Based on  https://docs.microsoft.com/en-us/dotnet/api/microsoft.servicefabric.data.iasyncenumerator-1?view=azure-dotnet
    /// Asynchronous enumerator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncEnumerator<out T> : IDisposable
    {
        /// <summary>
        /// Gets the current element in the enumerator.
        /// </summary>
        T Current { get; }

        /// <summary>
        /// Advances the enumerator to the next element of the enumerator.
        /// </summary>
        /// <param name="token">The token to monitor for cancellation requests.</param>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        /// <exception cref="System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        Task<bool> MoveNextAsync(CancellationToken token);

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        void Reset();

    }
}
