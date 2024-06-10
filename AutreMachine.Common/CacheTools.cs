using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Reflection.Metadata.Ecma335;
using System.Configuration;

namespace AutreMachine.Common
{
    public class CacheTools<T>
    {
        // TODO: change memorycache with Redis
        static MemoryCache _cache = MemoryCache.Default;

        /// <summary>
        /// Multiple GetCache if key is composite
        /// </summary>
        /// <param name="key"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public static T? GetCache(string key)
        {
            key = key.Trim().ToUpper();
            if (_cache[key] != null && _cache[key] is T)
                return (T)_cache[key];

            return default(T);
        }
        public static T? GetCache(params string[] keys)
        {
            // Keys are trimmed - uppered()
            keys = keys.Select(x => x.Trim().ToUpper()).ToArray();
            var key = string.Join('-', keys);

            // If key empty ?
            if (string.IsNullOrEmpty(key))
                return default(T);

            return GetCache(key);
        }


        /// <summary>
        /// Cache per default : 5 minutes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        private static void setCache(string key, T value, CacheDuration duration)
        {
            key = key.Trim().ToUpper();

            var policy = new CacheItemPolicy();
            TimeSpan ts = TimeSpan.Zero;
            switch (duration)
            {
                case CacheDuration._1Min:
                    ts = new TimeSpan(0, 1, 0);
                    break;
                case CacheDuration._5Min:
                    ts = new TimeSpan(0, 5, 0);
                    break;
                case CacheDuration._30Min:
                    ts = new TimeSpan(0, 30, 0);
                    break;
                case CacheDuration._1Hour:
                    ts = new TimeSpan(1, 0, 0);
                    break;
                case CacheDuration._6Hours:
                    ts = new TimeSpan(6, 0, 0);
                    break;
                default:
                    throw new Exception("Duration not handled");
            }
            policy.SlidingExpiration = ts;
            _cache.Set(new CacheItem(key, value), policy);

        }

        public static void SetCache(string key, T value, CacheDuration duration)
        {
            setCache(key, value, duration);
        }

         public static void SetCache(string key, T value)
        {
            SetCache(key, value, CacheDuration._5Min);
        }

        
    }

    public enum CacheDuration
    {
        _1Min,
        _5Min,
        _30Min,
        _1Hour,
        _6Hours
    }

    public class CacheKeys
    {
        static MemoryCache _cache = MemoryCache.Default;

        #region Methods without type
        // These methods are in this class because it needs no Type to pass
        public static void ClearCache(string key)
        {
            key = key.Trim().ToUpper();
            _cache.Remove(key);   
        }

        public static string GenerateKey(params string[] keys) { return string.Join('_', keys).Trim().ToUpper(); }


        /// <summary>
        /// Clear all cache beginning with a key
        /// </summary>
        /// <param name="key"></param>
        public static void ClearCacheStartingWith(string key)
        {
            key = key.Trim().ToUpper();
            var removedKeys = _cache.Where(x => x.Key.StartsWith(key)).Select(x => x.Key).ToList();
            foreach(var unitKey in removedKeys)
                _cache.Remove(unitKey);
        }

        public static bool ExistsEntriesStartingWith(string key)
        {
            key = key.Trim().ToUpper();
            var foundKeys = _cache.Where(x => x.Key.StartsWith(key)).Any();
            return (foundKeys);
        }
        #endregion

    }


}
