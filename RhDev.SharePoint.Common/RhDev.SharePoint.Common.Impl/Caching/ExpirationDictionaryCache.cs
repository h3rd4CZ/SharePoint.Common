using System;
using System.Collections.Generic;
using System.Linq;
using RhDev.SharePoint.Common.Caching;
using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.Impl.Caching
{
    public class ExpirationDictionaryCache<TValue> : ServiceBase, ICache<TValue> where TValue : class
    {
        private readonly IDictionary<CacheKey, ExpirationDictionaryCacheItem<TValue>> cache = new Dictionary<CacheKey, ExpirationDictionaryCacheItem<TValue>>();

        private readonly ExpirationCacheConfiguration configuration;
        private readonly ICentralClockProvider _centralCLockProvider;

        public IDictionary<CacheKey, ExpirationDictionaryCacheItem<TValue>> GetCache => cache;

        public ExpirationDictionaryCache(ExpirationCacheConfiguration configuration, ICentralClockProvider centralCLockProvider, ITraceLogger traceLogger) : base(traceLogger)
        {
            this.configuration = configuration;
            _centralCLockProvider = centralCLockProvider;
        }

        
        public IList<KeyValuePair<CacheKey, TValue>> GetCacheContent
        {
            get
            {
                lock (cache)
                {
                    return cache.ToList().Select(c => new KeyValuePair<CacheKey, TValue>(c.Key, c.Value.Value))
                        .ToList();
                }
            }
        }

        protected override TraceCategory TraceCategory => TraceCategories.Common;

        public void AddValue(CacheKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (value == null)
                throw new ArgumentNullException("value");

            lock (cache)
                cache[key] = ExpirationDictionaryCacheItem<TValue>.Create(value, _centralCLockProvider.Now().ExportDateTime);
        }

        public TValue GetValue(CacheKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            lock (cache)
            {
                ExpirationDictionaryCacheItem<TValue> value;
                cache.TryGetValue(key, out value);

                return !Equals(null, value) ? value.Value : null;
            }
        }

        public  ExpirationDictionaryCacheItem<TValue> GetCacheItemValue(CacheKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            lock (cache)
            {
                ExpirationDictionaryCacheItem<TValue> value;
                cache.TryGetValue(key, out value);

                return value;
            }
        }

        public TValue GetOrFetchExpiredValue(CacheKey key, Func<TValue> valueProvider)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            lock (cache)
            {
                ExpirationDictionaryCacheItem<TValue> value;
                cache.TryGetValue(key, out value);

                if (value == null)
                    return FetchValue(key, valueProvider);

                var now = _centralCLockProvider.Now();

                if (value.InsertionDateTime.AddMinutes(configuration.ExpirationDurationInMinutes) < now.ExportDateTime)
                    return FetchExpiredValue(key, valueProvider);

                return value.Value;
            }
        }

        private TValue FetchExpiredValue(CacheKey key, Func<TValue> valueProvider)
        {
            WriteTrace("Cache of {0}: expired for key '{1}', fetching value", typeof(TValue), key);
            TValue value = valueProvider();

            if (value != null)
            {
                lock (cache)
                {
                    cache.Remove(key);
                    cache[key] = ExpirationDictionaryCacheItem<TValue>.Create(value, _centralCLockProvider.Now().ExportDateTime);
                }
            }
            else
                WriteTrace("Cache of {0}: null value fetched", typeof(TValue));

            return value;
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
            WriteTrace("Cache of {0}: miss for key '{1}', fetching value", typeof(TValue), key);
            TValue value = valueProvider();

            if (value != null)
                AddValue(key, value);
            else
                WriteTrace("Cache of {0}: null value fetched", typeof(TValue));

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
