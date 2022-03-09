using System;
using System.Collections.Generic;
using System.Collections;
using System.Web.Caching;

namespace SymaCord.TryOnMirror.Core.Util.Impl
{
    public class Cache : ICache
    {
        private System.Web.Caching.Cache cache;
        private TimeSpan timeSpan;

        public Cache()
        {
            cache = System.Web.HttpRuntime.Cache;
            timeSpan = new TimeSpan(1, 0, 0, 0);
        }

        public object Get(string cache_key)
        {
            return cache.Get(cache_key);
        }

        public List<string> GetCacheKeys()
        {
            List<string> keys = new List<string>();
            IDictionaryEnumerator ca = cache.GetEnumerator();
            while (ca.MoveNext())
            {
                keys.Add(ca.Key.ToString());
            }
            return keys;
        }

        public void Set(string cache_key, object cache_object)
        {
            Set(cache_key, cache_object, timeSpan);
        }

        public void Set(string cache_key, object cache_object, DateTime expiration)
        {
            Set(cache_key, cache_object, expiration, CacheItemPriority.Normal);
        }

        public void Set(string cache_key, object cache_object, TimeSpan expiration)
        {
            Set(cache_key, cache_object, expiration, CacheItemPriority.Normal);
        }

        public void Set(string cache_key, object cache_object, DateTime expiration, CacheItemPriority priority)
        {
            if (cache_object != null)
                cache.Insert(cache_key, cache_object, null, expiration, System.Web.Caching.Cache.NoSlidingExpiration, priority, null);
        }

        public void Set(string cache_key, object cache_object, TimeSpan expiration, CacheItemPriority priority)
        {
            if (cache_object != null)
                cache.Insert(cache_key, cache_object, null, System.Web.Caching.Cache.NoAbsoluteExpiration, expiration, priority, null);
        }

        public void Delete(string cache_key)
        {
            if (Exists(cache_key))
                cache.Remove(cache_key);
        }

        public void DeleteItems(string prefix)
        {
            prefix = prefix.ToLower();
            List<string> itemsToRemove = new List<string>();

            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().ToLower().StartsWith(prefix))
                    itemsToRemove.Add(enumerator.Key.ToString());
            }

            foreach (string itemToRemove in itemsToRemove)
                cache.Remove(itemToRemove);
        }

        public bool Exists(string cache_key)
        {
            if (cache[cache_key] != null)
                return true;
            else
                return false;
        }

        public void Flush()
        {
            foreach (string s in GetCacheKeys())
            {
                Delete(s);
            }
        }
    }
}
