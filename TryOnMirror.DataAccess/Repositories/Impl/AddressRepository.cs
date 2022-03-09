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
    public class AddressRepository : IAddressRepository
    {
        private ICache _cache;

        public AddressRepository(ICache cache)
        {
            _cache = cache;
        }

        public Address GetAddress(int id)
        {
            string key = "Address_" + id + "_GetAddress";
            Address result = null;

            if (_cache.Exists(key))
                result = (Address) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.Addresses.FirstOrDefault(x => x.AddressId == id);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int GetAddressId(string salonHook)
        {
            string key = "Address_" + salonHook + "_GetAddressId";
            int result;

            if (_cache.Exists(key))
                result = (int)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = (from a in dc.Addresses
                              from s in a.Salons
                              where s.Hook.Equals(salonHook)
                              select a.AddressId).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public Address GetAddressBySalonId(int salonId)
        {
            string key = "Address_" + salonId + "_GetAddressBySalonId";
            Address result = null;

            if (_cache.Exists(key))
                result = (Address) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = (from s in dc.Salons
                              join a in dc.Addresses on s.AddressId equals a.AddressId
                              where s.SalonId == salonId
                              select a).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int Save(Address address, IEnumerable<Expression<Func<Address, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                if (properties != null)
                {
                    dc.Addresses.Attach(address);
                    ObjectStateEntry entry = ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager
                        .GetObjectStateEntry(address);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }
                else
                {
                    dc.Addresses.Add(address);
                }

                dc.SaveChanges();
            }

            return address.AddressId;
        }

        public void Delete(int id)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                var address = new Address {AddressId = id};
                dc.Addresses.Attach(address);

                ((IObjectContextAdapter) dc)
                    .ObjectContext.ObjectStateManager.ChangeObjectState(address, EntityState.Deleted);

                dc.SaveChanges();
            }
        }
    }
}
