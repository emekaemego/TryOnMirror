using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
    public class HairCategoryService : IHairCategoryService
    {
        private IHairCategoryRepository _categoryRepository;
        private ICache _cache;

        public HairCategoryService(IHairCategoryRepository categoryRepository, ICache cache)
        {
            _categoryRepository = categoryRepository;
            _cache = cache;
        }

        public IEnumerable<HairCategory> GetHairCategories(string search, int? page, int maxRows)
        {
            return _categoryRepository.GetHairCategories(search, page, maxRows);
        }

        public HairCategory GetHairCategory(int id)
        {
            return _categoryRepository.GetHairCategory(id);
        }

        public int Save(HairCategory category, IEnumerable<Expression<Func<HairCategory, object>>> properties)
        {
            var id = _categoryRepository.Save(category, properties);

            DeleteCaches(id);

            return id;
        }

        public void Delete(int id)
        {
            _categoryRepository.Delete(id);

            DeleteCaches(id);
        }

        private void DeleteCaches(int id)
        {
            _cache.DeleteItems("haircategory_" + id + "_");
            _cache.DeleteItems("haircategories_");
        }
    }
}
