using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories.Impl
{
    public class SharedMakeoverRepository : ISharedMakeoverRepository
    {
        private ICache _cache;

        public SharedMakeoverRepository(ICache cache)
        {
            _cache = cache;
        }

        public SharedMakeover GetSharedMakeover(long id)
        {
            string key = "SharedMakeover_" + id + "_GetSharedMakeover";
            SharedMakeover result = null;

            if (_cache.Exists(key))
                result = (SharedMakeover) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.SharedMakeovers.FirstOrDefault(x => x.SharedId == id);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public SharedMakeover GetSharedMakeover(string urlCode)
        {
            string key = "SharedMakeover_" + urlCode + "_GetSharedMakeover";
            SharedMakeover result = null;

            if (_cache.Exists(key))
                result = (SharedMakeover) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.SharedMakeovers.FirstOrDefault(x => x.UrlCode.Equals(urlCode));

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public long Save(SharedMakeover shared, IEnumerable<Expression<Func<SharedMakeover, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                if (properties != null)
                {
                    dc.SharedMakeovers.Attach(shared);
                    ObjectStateEntry entry = ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager
                        .GetObjectStateEntry(shared);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }
                else
                {
                    dc.SharedMakeovers.Add(shared);
                }

                dc.SaveChanges();
            }

            return shared.SharedId;
        }
    }
}
