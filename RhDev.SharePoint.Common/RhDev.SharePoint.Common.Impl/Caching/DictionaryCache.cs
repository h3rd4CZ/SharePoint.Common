using RhDev.SharePoint.Common.Caching;
using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RhDev.SharePoint.Common.Impl.Caching
{
    public class DictionaryCache<TValue> : ServiceBase, ICache<TValue> where TValue : class
    {
        private readonly IDictionary<CacheKey, TValue> cache = new Dictionary<CacheKey, TValue>();

        public IList<KeyValuePair<CacheKey, TValue>> GetCacheContent
        {
            get
            {
                lock (cache)
                {
                    return cache.AsEnumerable().ToList();
                }
            }
        }
        public DictionaryCache(ITraceLogger traceLogger) : base(traceLogger)
        {

        }

        protected override TraceCategory TraceCategory => TraceCategories.Common;

        public void AddValue(CacheKey key, TValue value)
        {
            
            if (key == null)
                throw new ArgumentNullException("key");

            if (value == null)
                throw new ArgumentNullException("value");

            lock (cache)
                cache[key] = value;
        }

        public TValue GetValue(CacheKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            lock (cache)
            {
                TValue value;
                cache.TryGetValue(key, out value);

                return value;
            }
        }

        public TValue GetOrFetchValue(CacheKey key, Func<TValue> valueProvider)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (valueProvider == null)
                throw new ArgumentNullException("valueProvider");

            if (CacheBypass.IsActive)
            {
                return valueProvider();
            }

            lock (cache)
            {
                TValue value = GetValue(key) ?? FetchValue(key, valueProvider);
                return value;
            }
        }

        private TValue FetchValue(CacheKey key, Func<TValue> valueProvider)
        {
            WriteTrace("Cache of {0}: miss for key '{1}', fetching value", typeof (TValue), key);
            TValue value = valueProvider();

            if (value != null)
                AddValue(key, value);
            else
                WriteTrace("Cache of {0}: null value fetched", typeof (TValue));

            return value;
        }

        public void Clear()
        {
            lock (cache)
            {
                WriteTrace("Cache of {0}: clearing requested", typeof(TValue));
                cache.Clear();
            }
        }

        public void InvalidateCacheItem(CacheKey key)
        {
            if (key == null) throw new ArgumentNullException("key");

            lock (cache)
            {
                cache.Remove(key);
            }
        }
    }
}
