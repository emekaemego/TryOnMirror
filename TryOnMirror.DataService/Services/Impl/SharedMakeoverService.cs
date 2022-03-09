using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
   public class SharedMakeoverService : ISharedMakeoverService
   {
       private ISharedMakeoverRepository _repository;

       public SharedMakeoverService(ISharedMakeoverRepository repository)
       {
           _repository = repository;
       }

       public SharedMakeover GetSharedMakeover(long id)
       {
           return _repository.GetSharedMakeover(id);
       }

       public SharedMakeover GetSharedMakeover(string urlCode)
       {
           return _repository.GetSharedMakeover(urlCode);
       }

       public long Save(SharedMakeover shared, IEnumerable<Expression<Func<SharedMakeover, object>>> properties)
       {
           var id =_repository.Save(shared, properties);
           return id;
       }

   }
}
