using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories
{
    public interface IGlassRepository
    {
        IEnumerable<Glass> GetGlasses(int? categoryId, string seach, int? page, int maxRows);
        Glass GetGlass(int id);
        Glass GetGlass(string fileName);
        int Save(Glass glass, IEnumerable<Expression<Func<Glass, object>>> properties);
        void Delete(int id);
        int GetGlassId(string fileName);
    }
}