using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories.Impl
{
    public class GlassRepository : IGlassRepository
    {
        private ICache _cache;

        public GlassRepository(ICache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Glass> GetGlasses(int? categoryId, string seach, int? page, int maxRows)
        {
            string key = "Glasses_" + seach + "_" + page + "_" + categoryId + "_" + maxRows + "_GetGlasses";

            var result = new List<Glass>();

            if (_cache.Exists(key))
                result = (List<Glass>)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.Glasses select x;

                    if (categoryId.HasValue)
                    {
                        oq = oq.Where(x => x.CategoryId == categoryId.Value);
                    }

                    if (!string.IsNullOrEmpty(seach))
                    {
                        oq = oq.Where(x => x.FileName.Contains(seach));
                    }

                    result = oq.OrderByDescending(x => x.DateCreated).Page(page, maxRows).ToList();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public Glass GetGlass(int id)
        {
            string key = "Glass_" + id + "_GetGlass";
            Glass result = null;

            if (_cache.Exists(key))
                result = (Glass)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Glasses.FirstOrDefault(x => x.GlassId == id);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public Glass GetGlass(string fileName)
        {
            string key = "Glass_" + fileName + "_GetGlass";
            Glass result = null;

            if (_cache.Exists(key))
                result = (Glass)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Glasses.FirstOrDefault(x => x.FileName == fileName);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int GetGlassId(string fileName)
        {
            string key = "Glass_" + fileName + "_GetGlassId";
            int result = 0;

            if (_cache.Exists(key))
                result = (int)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Glasses.Where(x => x.FileName.Equals(fileName)).Select(x => x.GlassId).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int Save(Glass glass, IEnumerable<Expression<Func<Glass, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                if (properties != null)
                {
                    dc.Glasses.Attach(glass);
                    ObjectStateEntry entry = ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager
                        .GetObjectStateEntry(glass);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }
                else
                {
                    dc.Glasses.Add(glass);
                }

                dc.SaveChanges();
            }

            return glass.GlassId;
        }

        public void Delete(int id)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                var glass = new Glass { GlassId = id };
                dc.Glasses.Attach(glass);

                ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager.ChangeObjectState(glass,
                                                                                                EntityState.Deleted);

                dc.SaveChanges();
            }
        }
    }
}
