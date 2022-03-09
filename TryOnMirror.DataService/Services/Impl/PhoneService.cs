using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SymaCord.VirtualMakeover.Core.Util;
using SymaCord.VirtualMakeover.DataAccess.Repositories;
using SymaCord.VirtualMakeover.Entities;

namespace SymaCord.VirtualMakeover.DataService.Services.Impl
{
   public class PhoneService : IPhoneService
   {
       private IPhoneRepository _repository;
       private ICache _cache;

       public PhoneService(IPhoneRepository repository, ICache cache)
       {
           _repository = repository;
           _cache = cache;
       }

       public Phone GetPhone(int id)
       {
           return _repository.GetPhone(id);
       }

       public Phone GetPhoneBySalonId(int salonId)
       {
           return _repository.GetPhoneBySalonId(salonId);
       }

       public int Save(Phone phone, IEnumerable<Expression<Func<Phone, object>>> properties)
       {
           var result = _repository.Save(phone, properties);

           _cache.DeleteItems("phone_" + phone.PhoneId + "_");

           return result;
       }

       public void Delete(int id)
       {
           _repository.Delete(id);
           _cache.DeleteItems("phone_" + id + "_");
       }
   }
}
