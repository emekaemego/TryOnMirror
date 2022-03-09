using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
    public class AddressService : IAddressService
    {
        private IAddressRepository _repository;
        private ICache _cache;

        public AddressService(IAddressRepository repository, ICache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public Address GetAddress(int id)
        {
            return _repository.GetAddress(id);
        }

        public Address GetAddressBySalonId(int salonId)
        {
            return _repository.GetAddressBySalonId(salonId);
        }

        public int GetAddressId(string salonHook)
        {
            return _repository.GetAddressId(salonHook);
        }

        public int Save(Address address, IEnumerable<Expression<Func<Address, object>>> properties)
        {
            var result = _repository.Save(address, properties);

            _cache.DeleteItems("address_" + address.AddressId + "_");

            return result;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);

            _cache.DeleteItems("address_" + id + "_");
        }
    }
}