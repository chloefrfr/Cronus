using System.Collections.Concurrent;

namespace Larry.Source.Utilities.Managers
{
    public class CacheManager<T>
    {
        private readonly ConcurrentDictionary<string, (T Data, DateTime CachedAt)> _cache = new();
        private readonly TimeSpan _cacheExpiration;

        public CacheManager(TimeSpan cacheExpiration)
        {
            _cacheExpiration = cacheExpiration;
        }

        public bool TryGetValue(string key, out T data)
        {
            if (_cache.TryGetValue(key, out var cacheEntry))
            {
                if (DateTime.Now - cacheEntry.CachedAt < _cacheExpiration)
                {
                    data = cacheEntry.Data;
                    return true;
                }
                else
                {
                    _cache.TryRemove(key, out _);
                }
            }

            data = default;
            return false;
        }

        public void Set(string key, T data)
        {
            _cache[key] = (data, DateTime.Now);
        }
    }
}
