using System;
using System.Runtime.Caching;

namespace fusionInsiteServicesData.Cache
{
    public static class CacheHelper
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;

        public static void AddItem(string key, object item, double minutes)
        {
            var cp = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddMinutes(minutes) };

            var cacheitem = new CacheItem(key)
            {
                Value = item
            };

            Cache.Add(cacheitem, cp);
        }

        public static T GetItem<T>(string key)
        {
            return (T)Cache.Get(key);
        }

        public static T GetOrLoad<T>(string key, Func<T> method, double minutes)
        {
            T item;
            if (Cache.Contains(key))
            {
                item = (T)Cache.Get(key);
            }
            else
            {
                item = method();
                AddItem(key, item, minutes);
            }

            return item;
        }


        public static void RemoveItem(string key)
        {
            Cache.Remove(key);
        }

    }
}
