using System.Collections.Generic;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
   public class BrandService : IBrandService
   {
       private IBrandRepository _repository;

       public BrandService(IBrandRepository repository)
       {
           _repository = repository;
       }

       public IEnumerable<Brand> GetBrands(int? categoryId)
       {
           return _repository.GetBrands(categoryId);
       }
   }
}
