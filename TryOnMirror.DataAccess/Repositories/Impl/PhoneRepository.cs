using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using SymaCord.VirtualMakeover.Core;
using SymaCord.VirtualMakeover.Core.Util;
using SymaCord.VirtualMakeover.Entities;

namespace SymaCord.VirtualMakeover.DataAccess.Repositories.Impl
{
    public class PhoneRepository : IPhoneRepository
    {
        private ICache _cache;

        public PhoneRepository(ICache cache)
        {
            _cache = cache;
        }

        public Phone GetPhone(int id)
        {
            string key = "Phone_" + id + "_GetPhone";
            Phone result = null;

            if (_cache.Exists(key))
                result = (Phone) _cache.Get(key);
            else
            {
                using (var dc = new VirtualMakeoverEntities())
                {
                    result = dc.Phones.FirstOrDefault(x => x.PhoneId == id);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public Phone GetPhoneBySalonId(int salonId)
        {
            string key = "Phone_" + salonId + "_GetPhoneBySalonId";
            Phone result = null;

            if (_cache.Exists(key))
                result = (Phone) _cache.Get(key);
            else
            {
                using (var dc = new VirtualMakeoverEntities())
                {
                    result = (from s in dc.Salons
                              join p in dc.Phones on s.PhoneId equals p.PhoneId
                              where s.SalonId == salonId
                              select p).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int Save(Phone phone, IEnumerable<Expression<Func<Phone, object>>> properties)
        {
            using (var dc = new VirtualMakeoverEntities())
            {
                if (properties != null)
                {
                    dc.Phones.Attach(phone);
                    ObjectStateEntry entry = ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager
                        .GetObjectStateEntry(phone);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }
                else
                {
                    dc.Phones.Add(phone);
                }

                dc.SaveChanges();
            }

            return phone.PhoneId;
        }

        public void Delete(int id)
        {
            using (var dc = new VirtualMakeoverEntities())
            {
                var phone = new Phone {PhoneId = id};
                dc.Phones.Attach(phone);

                ((IObjectContextAdapter) dc)
                    .ObjectContext.ObjectStateManager.ChangeObjectState(phone, EntityState.Deleted);

                dc.SaveChanges();
            }
        }
    }
}