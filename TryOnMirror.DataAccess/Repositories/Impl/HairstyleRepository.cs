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
    public class HairstyleRepository : IHairstyleRepository
    {
        private ICache _cache;

        public HairstyleRepository(ICache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Hairstyle> GetHairstyles(string seach, int? page, int maxRows)
        {
            string key = "Hairstyles_" + seach + "_" + page + "_" + maxRows + "_GetHairstyles";

            var result = new List<Hairstyle>();

            if (_cache.Exists(key))
                result = (List<Hairstyle>) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.Hairstyles select x;

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

        public string[] GetHairstyleNames(string seach, int? page, int maxRows)
        {
            string key = "Hairstyles_" + seach + "_" + page + "_" + maxRows + "_GetHairstyleNames";

            var result = new string[] {};

            if (_cache.Exists(key))
                result = (string[]) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.Hairstyles select x;

                    if (!string.IsNullOrEmpty(seach))
                    {
                        oq = oq.Where(x => x.FileName.Contains(seach));
                    }

                    result =
                        oq.OrderByDescending(x => x.DateCreated).Page(page, maxRows).Select(x => x.FileName).ToArray();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public int GetHairstylesCount(string seach)
        {
            string key = "Hairstyles_" + seach + "_" + "_GetHairstylesCount";

            int result;

            if (_cache.Exists(key))
                result = (int)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.Hairstyles select x;

                    if (!string.IsNullOrEmpty(seach))
                    {
                        oq = oq.Where(x => x.FileName.Contains(seach));
                    }

                    result = oq.Count();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public Hairstyle GetHairstyle(long id)
        {
            string key = "Hairstyle_" + id + "_GetHairstyle";
            Hairstyle result = null;

            if (_cache.Exists(key))
                result = (Hairstyle)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Hairstyles.FirstOrDefault(x=>x.HairstyleId == id);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public Hairstyle GetHairstyle(string fileName)
        {
            string key = "Hairstyle_" + fileName + "_GetHairstyle";
            Hairstyle result = null;

            if (_cache.Exists(key))
                result = (Hairstyle)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Hairstyles.Include("Brand").FirstOrDefault(x => x.FileName == fileName);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int GetHairstyleId(string fileName)
        {
            string key = "Hairstyle_" + fileName + "_GetId";
            int result = 0;

            if (_cache.Exists(key))
                result = (int)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Hairstyles.Where(x => x.FileName == fileName).Select(x => x.HairstyleId).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int Save(Hairstyle hairstyle, IEnumerable<Expression<Func<Hairstyle, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;

                if (properties != null)
                {
                    dc.Hairstyles.Attach(hairstyle);
                    ObjectStateEntry entry = ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager
                        .GetObjectStateEntry(hairstyle);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }
                else
                {
                    dc.Hairstyles.Add(hairstyle);
                }

                dc.SaveChanges();
            }

            return hairstyle.HairstyleId;
        }

        public void Delete(int id)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                var hairstyle = new Hairstyle { HairstyleId = id };
                dc.Hairstyles.Attach(hairstyle);

                ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager.ChangeObjectState(hairstyle,
                                                                                                EntityState.Deleted);

                dc.SaveChanges();
            }
        }
    }
}