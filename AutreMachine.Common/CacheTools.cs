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


        #region Cache keys used in the app - Blazor Server
        public static string QUOTE(string assetType, string code, string preferredCurrency) { return generateKey("Quote", assetType, code, preferredCurrency); }
        public static string ASSETS_VALUE(string userId) { return generateKey("AssetsValue", userId); }
        //public static string ASSETS_VALUE_LOCAL(string userId) { return generateKey("AssetsValueLocal", userId); }
        public static string BALANCE(string userId) { return generateKey("Balance", userId); }
        public static string ASSET(string userId, string assetType, string code) { return generateKey("Asset", userId, assetType, code); }
        public static string ASSETS(string userId) { return generateKey("Assets", userId); }
        //public static string RECEIVED_MONEY(string userId) { return generateKey("ReceivedMoney", userId); }
        //public static string CURRENCIES() { return "Currencies"; }
        public static string PREFERRED_CURRENCY(string userId) { return generateKey("PreferredCurrency", userId); }
        public static string PREFERRED_CULTURE(string userId) { return generateKey("PreferredCulture", userId); }
        public static string MESSAGE(int messageId) { return generateKey("Message", messageId.ToString()); }
        public static string MESSAGES(string userId) { return generateKey("Messages", userId); }
        public static string SEARCHSYMBOLS(string assetType, string text) { return generateKey("SearchSymbols", assetType.ToString(), text); }
        public static string CHARTDATA(string assetType, string code, string range, string interval) { return generateKey("ChartData", assetType, code, range, interval); }
        public static string ASSETNAMEDESCRIPTIONS(string assetType) { return generateKey("AssetNameDescriptions", assetType); }
        public static string ASSETSUGGESTIONS(string assetType, int nbMax) { return generateKey("AssetSuggestions", assetType, nbMax.ToString()); }
        public static string ASSETRATEHISTORY(string assetType, string code) { return generateKey("AssetHistory", assetType.ToString(), code); }
        public static string USEREXISTS(string userId) { return generateKey("UserExists", userId); }
        public static string MONEY(string userId) { return generateKey("Money", userId); }
        public static string MONEYHISTORY(string userId, string range) { return generateKey("MoneyHistory", userId, range); }
        public static string BETS() { return generateKey("Bets"); }
        public static string BET(int betId) { return generateKey("Bet", betId.ToString()); }
        public static string BETSMINE(string userId) { return generateKey("BetsMine", userId); }
        public static string BETBLOCKEDASSET(string userId, string assetType, string code) { return generateKey("BetBlockedAsset", userId, assetType, code); }
        // for all the keys of BETBLOCKEDASSET
        public static string BETBLOCKEDASSET_Start(string userId) { return generateKey("BetBlockedAsset", userId); }
        public static string BETBLOCKEDASSETS(string userId) { return generateKey("BetBlockedAssets", userId); }
        public static string HALLOFFAME(int size) { return generateKey("HallOfFame", size.ToString()); }

        private static string generateKey(params string[] keys) { return string.Join('_', keys).Trim().ToUpper(); }
        #endregion

        #region Cache keys used in CurrencyRateService
        public static string CURRENCY(string currency) { return generateKey("Currency", currency); }
        public static string CURRENCYDATE(string currency, DateTime date) { return generateKey("Currency", currency, date.ToString("yyyy-MM-dd")); }
        #endregion

        #region Methods without type
        // These methods are in this class because it needs no Type to pass
        public static void ClearCache(string key)
        {
            key = key.Trim().ToUpper();
            _cache.Remove(key);   
        }

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

    /// <summary>
    /// Cache keys for the Client app - WASM
    /// </summary>
    public class CacheKeysWASM
    {
        #region Cache keys used in Blazor Client
        public static string USERDATA { get { return "UserData"; } }
        public static string USERID { get { return "UserId"; } }
        public static string MYBETS { get { return "MyBets"; } }
        #endregion
    }
}
