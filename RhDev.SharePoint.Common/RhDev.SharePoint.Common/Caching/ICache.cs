using RhDev.SharePoint.Common.Caching.Composition;
using System;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Caching
{
    public interface ICache<TValue> : IService where TValue : class 
    {
        IList<KeyValuePair<CacheKey, TValue>> GetCacheContent { get; }

        void AddValue(CacheKey key, TValue value);

        TValue GetValue(CacheKey key);

        TValue GetOrFetchValue(CacheKey key, Func<TValue> valueProvider);

        void Clear();
        void InvalidateCacheItem(CacheKey key);
    }
}
