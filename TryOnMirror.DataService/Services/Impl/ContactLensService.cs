using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
   public class ContactLensService : IContactLensService
   {
       private IContactLensRepository _repository;
       private ICache _cache;

       public ContactLensService(IContactLensRepository repository, ICache cache)
       {
           _repository = repository;
           _cache = cache;
       }

       public IEnumerable<ContactLens> GetContactLenses(string seach, int? page, int maxRows)
       {
           return _repository.GetContactLenses(seach, page, maxRows);
       }

       public string[] GetContactLensFileNames(string seach, int? page, int maxRows)
       {
           return _repository.GetContactLensFileNames(seach, page, maxRows);
       }

       public ContactLens GetContactLens(long id)
       {
           return _repository.GetContactLens(id);
       }

       public ContactLens GetContactLens(string fileName)
       {
           return _repository.GetContactLens(fileName);
       }
       
       public int Save(ContactLens contact, IEnumerable<Expression<Func<ContactLens, object>>> properties)
       {
           var result = _repository.Save(contact, properties);

           _cache.DeleteItems("contactlens_" + contact.ContactId + "_");
           _cache.DeleteItems("contactlens_" + contact.FileName + "_");
           _cache.DeleteItems("contactlenses_");

           return result;
       }

       public void Delete(int id)
       {
           _cache.DeleteItems("contactlens_" + id + "_");
           _cache.DeleteItems("contactlenses_");

           _repository.Delete(id);
       }
   }
}
