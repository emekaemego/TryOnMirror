using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories
{
    public interface IHairCategoryRepository
    {
        IEnumerable<HairCategory> GetHairCategories(string seach, int? page, int maxRows);
        HairCategory GetHairCategory(int id);
        int Save(HairCategory category, IEnumerable<Expression<Func<HairCategory, object>>> properties);
        void Delete(int id);
    }
}