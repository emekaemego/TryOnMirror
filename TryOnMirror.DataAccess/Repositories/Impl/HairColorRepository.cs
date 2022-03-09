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
    public class HairColorRepository : IHairColorRepository
    {
        private ICache _cache;

        public HairColorRepository(ICache cache)
        {
            _cache = cache;
        }

        public IEnumerable<HairColor> GetHairColors(string seach, int? page, int maxRows)
        {
            string key = "HairColors_" + seach + "_" + page + "_" + maxRows + "_GetHairColors";

            var result = new List<HairColor>();

            if (_cache.Exists(key))
                result = (List<HairColor>) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.HairColors select x;

                    if (!string.IsNullOrEmpty(seach))
                    {
                        oq = oq.Where(x => x.ImageFileName.Contains(seach));
                    }

                    result = oq.OrderByDescending(x => x.DateCreated).Page(page, maxRows).ToList();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public string[] GetHairColorNames(string seach, int? page, int maxRows)
        {
            string key = "HairColors_" + seach + "_" + page + "_" + maxRows + "_GetHairColorNames";

            var result = new string[] {};

            if (_cache.Exists(key))
                result = (string[]) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.HairColors select x;

                    if (!string.IsNullOrEmpty(seach))
                    {
                        oq = oq.Where(x => x.ImageFileName.Contains(seach));
                    }

                    result =
                        oq.OrderByDescending(x => x.DateCreated).Page(page, maxRows).Select(x => x.ImageFileName).ToArray();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public int GetHairColorsCount(string seach)
        {
            string key = "HairColors_" + seach + "_" + "_GetHairColorsCount";

            int result;

            if (_cache.Exists(key))
                result = (int) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.HairColors select x;

                    if (!string.IsNullOrEmpty(seach))
                    {
                        oq = oq.Where(x => x.ImageFileName.Contains(seach));
                    }

                    result = oq.Count();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public HairColor GetHairColor(int id)
        {
            string key = "HairColor_" + id + "_GetHairColor";
            HairColor result = null;

            if (_cache.Exists(key))
                result = (HairColor) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.HairColors.FirstOrDefault(x => x.HairColorId == id);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public HairColor GetHairColor(string fileName)
        {
            string key = "HairColor_" + fileName + "_GetHairColor";
            HairColor result = null;

            if (_cache.Exists(key))
                result = (HairColor) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.HairColors.FirstOrDefault(x => x.ImageFileName == fileName);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int GetHairColorId(string fileName)
        {
            string key = "HairColor_" + fileName + "_GetId";
            int result = 0;

            if (_cache.Exists(key))
                result = (int) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result =
                        dc.HairColors.Where(x => x.ImageFileName == fileName).Select(x => x.HairColorId).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int Save(HairColor color, IEnumerable<Expression<Func<HairColor, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;

                if (properties != null)
                {
                    dc.HairColors.Attach(color);
                    ObjectStateEntry entry = ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager
                        .GetObjectStateEntry(color);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }
                else
                {
                    dc.HairColors.Add(color);
                }

                dc.SaveChanges();
            }

            return color.HairColorId;
        }

        public void Delete(int id)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                var color = new HairColor {HairColorId = id};
                dc.HairColors.Attach(color);

                ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager.ChangeObjectState(color, EntityState.Deleted);

                dc.SaveChanges();
            }
        }
    }
}