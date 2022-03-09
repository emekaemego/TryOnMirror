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
   public class ContactLensRepository : IContactLensRepository
   {
       private ICache _cache;

       public ContactLensRepository(ICache cache)
       {
           _cache = cache;
       }

       public IEnumerable<ContactLens> GetContactLenses(string seach, int? page, int maxRows)
       {
           string key = "ContactLenses_" + seach + "_" + page + "_" + maxRows + "_GetContactLenses";

           var result = new List<ContactLens>();

           if (_cache.Exists(key))
               result = (List<ContactLens>)_cache.Get(key);
           else
           {
               using (var dc = new TryOnMirrorEntities())
               {
                   var oq = from x in dc.ContactLenses select x;
                   
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

       public string[] GetContactLensFileNames(string seach, int? page, int maxRows)
       {
           string key = "ContactLenses_" + seach + "_" + page + "_" + maxRows + "_GetContactLensFileNames";

           var result = new string[] {};

           if (_cache.Exists(key))
               result = (string[]) _cache.Get(key);
           else
           {
               using (var dc = new TryOnMirrorEntities())
               {
                   var oq = from x in dc.ContactLenses select x;

                   if (!string.IsNullOrEmpty(seach))
                   {
                       oq = oq.Where(x => x.FileName.Contains(seach));
                   }

                   result = oq.OrderByDescending(x => x.DateCreated).Select(x => x.FileName).Page(page, maxRows).ToArray();

                   _cache.Set(key, result);
               }
           }

           return result;
       }

       public ContactLens GetContactLens(long id)
       {
           string key = "ContactLens_" + id + "_GetContactLens";
           ContactLens result = null;

           if (_cache.Exists(key))
               result = (ContactLens)_cache.Get(key);
           else
           {
               using (var dc = new TryOnMirrorEntities())
               {
                   result = dc.ContactLenses.FirstOrDefault(x => x.ContactId == id);

                   _cache.Set(key, result);
               }
           }

           return result;
       }

       public ContactLens GetContactLens(string fileName)
       {
           string key = "ContactLens_" + fileName + "_GetContactLens";
           ContactLens result = null;

           if (_cache.Exists(key))
               result = (ContactLens)_cache.Get(key);
           else
           {
               using (var dc = new TryOnMirrorEntities())
               {
                   result = dc.ContactLenses.FirstOrDefault(x => x.FileName == fileName);

                   _cache.Set(key, result);
               }
           }

           return result;
       }

       public int Save(ContactLens contact, IEnumerable<Expression<Func<ContactLens, object>>> properties)
       {
           using (var dc = new TryOnMirrorEntities())
           {
               if (properties != null)
               {
                   dc.ContactLenses.Attach(contact);
                   ObjectStateEntry entry = ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager
                       .GetObjectStateEntry(contact);

                   foreach (var selector in properties)
                   {
                       string propertyName = selector.Body.PropertyToString();
                       entry.SetModifiedProperty(propertyName);
                   }
               }
               else
               {
                   dc.ContactLenses.Add(contact);
               }

               dc.SaveChanges();
           }

           return contact.ContactId;
       }

       public void Delete(int id)
       {
           using (var dc = new TryOnMirrorEntities())
           {
               var contact = new ContactLens { ContactId= id };
               dc.ContactLenses.Attach(contact);

               ((IObjectContextAdapter)dc).ObjectContext.ObjectStateManager.ChangeObjectState(contact, EntityState.Deleted);

               dc.SaveChanges();
           }
       }
   }
}
