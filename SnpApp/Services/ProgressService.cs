using System;
using SnpApp.Interfaces;

namespace SnpApp.Services
{
    /// <inheritdoc/>
    public class ProgressService : IProgressService
    {
        private IProgress<int>? _handler;
        private int _current;

        /// <inheritdoc/>
        public void Init(Action<int> handler)
        {
            // The Progress<T> constructor captures our UI context,
            // so the lambda will be run on the UI thread.
            _handler = new Progress<int>(handler);
            _current = 0;
        }

        /// <inheritdoc/>
        public void IncrementProgress(int total, int startPercentage = 0, int maxPercentage = 100)
        {
            var currentPercentage = _current * (maxPercentage - startPercentage) / total;
            ++_current;
            _handler?.Report(startPercentage + currentPercentage);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            _current = 0;
        }
    }
}
