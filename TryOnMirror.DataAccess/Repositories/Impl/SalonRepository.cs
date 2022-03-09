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
    public class SalonRepository : ISalonRepository
    {
        private ICache _cache;

        public SalonRepository(ICache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Salon> GetSalons(string seach, int? userId, int? page, int maxRows)
        {
            string key = "Salons_" + seach + "_" + userId + "_" + page + "_" + maxRows + "_GetSalons";

            var result = new List<Salon>();

            if (_cache.Exists(key))
                result = (List<Salon>) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.Salons.Include("Address") select x;

                    if (!string.IsNullOrEmpty(seach))
                    {
                        if (seach.StartsWith("@"))
                        {
                            oq = oq.Where(x => x.Hook.Equals(seach.Remove(0, 1)));
                        }
                        else
                        {
                            oq = oq.Where(x => x.SalonName.Contains(seach) || x.Address.City.Contains(seach) ||
                                               x.Address.State.Contains(seach));
                        }
                    }

                    if (userId.HasValue)
                    {
                        oq = oq.Where(x => x.UserId == userId.Value);
                    }

                    result = oq.OrderByDescending(x => x.DateCreated).Page(page, maxRows).ToList();

                    _cache.Set(key, result);

                }
            }
            return result;
        }

        public IEnumerable<Salon> GetSalonsLite(string seach, int? userId, int? page, int maxRows)
        {
            string key = "Salons_" + seach + "_" + userId + "_" + page + "_" + maxRows + "_GetSalonsLite";

            var result = new List<Salon>();

            if (_cache.Exists(key))
                result = (List<Salon>) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.Salons select x;

                    if (!string.IsNullOrEmpty(seach))
                    {
                        if (seach.StartsWith("@"))
                        {
                            oq = oq.Where(x => x.Hook.Equals(seach.Remove(0, 1)));
                        }
                        else
                        {
                            oq = oq.Where(x => x.SalonName.Contains(seach) || x.Address.City.Contains(seach) ||
                                               x.Address.State.Contains(seach));
                        }
                    }

                    if (userId.HasValue)
                    {
                        oq = oq.Where(x => x.UserId == userId.Value);
                    }

                    result = oq.OrderByDescending(x => x.DateCreated)
                        //.Select(x => new Salon
                        //    {
                        //        SalonName = x.SalonName,
                        //        SalonId = x.SalonId,
                        //        Identifier = x.Identifier,
                        //        Hook = x.Hook,
                        //        DateCreated = x.DateCreated
                        //    })
                        .Page(page, maxRows).ToList();

                    _cache.Set(key, result);

                }
            }
            return result;
        }

        public bool HasSalon(int userId)
        {
            string key = "Salons_" + userId + "_HasSalon";

            var result = false;

            if (_cache.Exists(key))
                result = (bool) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Salons.Any(x => x.UserId == userId);

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public bool CanModify(int userId, string hook)
        {
            string key = "Salons_" + userId + "_" + hook + "_CanModify";

            var result = false;

            if (_cache.Exists(key))
                result = (bool) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Salons.Any(x => x.UserId == userId && x.Hook.Equals(hook.Replace("@", "")));

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public Salon GetSalon(int id)
        {
            string key = "Salon_" + id + "_GetSalon";
            Salon result = null;

            if (_cache.Exists(key))
                result = (Salon) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Salons.FirstOrDefault(x => x.SalonId == id);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public Salon GetSalonWithContactInfo(int id)
        {
            string key = "Salon_" + id + "_GetSalonWithContactInfo";
            Salon result = null;

            if (_cache.Exists(key))
                result = (Salon) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Salons.Include("Address").FirstOrDefault(x => x.SalonId == id);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public Salon GetSalonWithContactInfo(string hook)
        {
            string key = "Salon_" + hook + "_GetSalonWithContactInfo";
            Salon result = null;

            if (_cache.Exists(key))
                result = (Salon) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Salons.Include("Address")
                        .FirstOrDefault(x => x.Hook.Equals(hook.Replace("@", "")));

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public Salon GetSalonWithContactInfo(Guid identifier)
        {
            string key = "Salon_" + identifier + "_GetSalonWithContactInfo";
            Salon result = null;

            if (_cache.Exists(key))
                result = (Salon) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result =
                        dc.Salons.Include("Address").Include("Phone").FirstOrDefault(x => x.Identifier == identifier);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public Salon GetSalon(Guid identifier)
        {
            string key = "Salon_" + identifier + "_GetSalon";
            Salon result = null;

            if (_cache.Exists(key))
                result = (Salon) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Salons.FirstOrDefault(x => x.Identifier == identifier);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public Salon GetSalon(string hook)
        {
            string key = "Salon_" + hook + "_GetSalon";
            Salon result = null;

            if (_cache.Exists(key))
                result = (Salon) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Salons.FirstOrDefault(x => x.Hook.Equals(hook.Replace("@", "")));

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int GetSalonId(string hook)
        {
            string key = "Salon_" + hook + "_GetSalonId";
            int result = 0;

            if (_cache.Exists(key))
                result = (int) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Salons.Where(x => x.Hook.Equals(hook)).Select(x => x.SalonId).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int GetSalonId(Guid identifier)
        {
            string key = "Salon_" + identifier + "_GetSalonId";
            int result = 0;

            if (_cache.Exists(key))
                result = (int) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Salons.Where(x => x.Identifier == identifier).Select(x => x.SalonId).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int GetSalonUserId(int id)
        {
            string key = "Salon_" + id + "_GetSalonUserId";
            int result = 0;

            if (_cache.Exists(key))
                result = (int) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Salons.Where(x => x.SalonId == id).Select(x => x.UserId).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int GetSalonUserId(string hook)
        {
            string key = "Salon_" + hook + "_GetSalonUserId";
            int result = 0;

            if (_cache.Exists(key))
                result = (int) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Salons.Where(x => x.Hook.Equals(hook)).Select(x => x.UserId).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int Save(Salon salon, IEnumerable<Expression<Func<Salon, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                if (properties != null)
                {
                    dc.Salons.Attach(salon);
                    ObjectStateEntry entry = ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager
                        .GetObjectStateEntry(salon);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }
                else
                {
                    dc.Salons.Add(salon);
                }

                dc.SaveChanges();
            }

            return salon.SalonId;
        }

        public void Delete(int id)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                var salon = new Salon {SalonId = id};
                dc.Salons.Attach(salon);

                ((IObjectContextAdapter) dc)
                    .ObjectContext.ObjectStateManager.ChangeObjectState(salon, EntityState.Deleted);

                dc.SaveChanges();
            }
        }
    }
}