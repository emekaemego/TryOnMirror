using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services
{
    public interface IHairCategoryService
    {
        IEnumerable<HairCategory> GetHairCategories(string search, int? page, int maxRows);
        HairCategory GetHairCategory(int id);
        int Save(HairCategory category, IEnumerable<Expression<Func<HairCategory, object>>> properties);
        void Delete(int id);
    }
}