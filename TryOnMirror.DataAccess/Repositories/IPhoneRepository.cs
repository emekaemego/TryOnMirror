using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.VirtualMakeover.Entities;

namespace SymaCord.VirtualMakeover.DataAccess.Repositories
{
    public interface IPhoneRepository
    {
        Phone GetPhone(int id);
        Phone GetPhoneBySalonId(int salonId);
        int Save(Phone phone, IEnumerable<Expression<Func<Phone, object>>> properties);
        void Delete(int id);
    }
}