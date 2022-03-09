using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
   public class HairColorService : IHairColorService
   {
       private IHairColorRepository _repository;
       private ICache _cache;

       public HairColorService(IHairColorRepository repository, ICache cache)
       {
           _repository = repository;
           _cache = cache;
       }

       public IEnumerable<HairColor> GetHairColors(string seach, int? page, int maxRows)
       {
           return _repository.GetHairColors(seach, page, maxRows);
       }

       public string[] GetHairColorNames(string seach, int? page, int maxRows)
       {
           return _repository.GetHairColorNames(seach, page, maxRows);
       }

       public int GetHairColorsCount(string seach)
       {
           return _repository.GetHairColorsCount(seach);
       }

       public HairColor GetHairColor(int id)
       {
           return _repository.GetHairColor(id);
       }

       public HairColor GetHairColor(string fileName)
       {
           return _repository.GetHairColor(fileName);
       }

       public int GetHairColorId(string fileName)
       {
           return _repository.GetHairColorId(fileName);
       }

       public int Save(HairColor color, IEnumerable<Expression<Func<HairColor, object>>> properties)
       {
           var result = _repository.Save(color, properties);

           return result;
       }

       public void Delete(int id)
       {
           _repository.Delete(id);
       }
   }
}
