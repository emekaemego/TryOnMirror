using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories
{
    public interface IAddressRepository
    {
        Address GetAddress(int id);
        Address GetAddressBySalonId(int salonId);
        int Save(Address address, IEnumerable<Expression<Func<Address, object>>> properties);
        void Delete(int id);
        int GetAddressId(string salonHook);
    }
}