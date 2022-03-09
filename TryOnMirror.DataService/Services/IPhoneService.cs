using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.VirtualMakeover.Entities;

namespace SymaCord.VirtualMakeover.DataService.Services
{
    public interface IPhoneService
    {
        Phone GetPhone(int id);
        Phone GetPhoneBySalonId(int salonId);
        int Save(Phone phone, IEnumerable<Expression<Func<Phone, object>>> properties);
        void Delete(int id);
    }
}