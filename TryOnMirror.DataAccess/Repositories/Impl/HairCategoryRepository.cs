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
    public class HairCategoryRepository : IHairCategoryRepository
    {
        private ICache _cache;

        public HairCategoryRepository(ICache cache)
        {
            _cache = cache;
        }

        public IEnumerable<HairCategory> GetHairCategories(string seach, int? page, int maxRows)
        {
            string key = "HairCategories_" + seach + "_" + page + "_" + maxRows + "_GetHairCategories";
            var result = new List<HairCategory>();

            if (_cache.Exists(key))
                result = (List<HairCategory>) _cache.Get(key);
            else
            {
                using(var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.HairCategories select x;

                    result = oq.Page(page, maxRows).ToList();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public HairCategory GetHairCategory(int id)
        {
            string key = "HairCategory_" + id + "_GetHairCategory";
            HairCategory result = null;

            if (_cache.Exists(key))
                result = (HairCategory)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.HairCategories where x.CategoryId == id select x;
                    result = oq.FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int Save(HairCategory category, IEnumerable<Expression<Func<HairCategory, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                if (properties != null)
                {
                    dc.HairCategories.Attach(category);
                    ObjectStateEntry entry = ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager.GetObjectStateEntry(category);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }
                else
                {
                    dc.HairCategories.Add(category);
                }

                dc.SaveChanges();
            }

            return category.CategoryId;
        }

        public void Delete(int id)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                var hair = new HairCategory {CategoryId = id };
                dc.HairCategories.Attach(hair);

                ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager.ChangeObjectState(hair, EntityState.Deleted);
                
                dc.SaveChanges();
            }
        }
    }
}
