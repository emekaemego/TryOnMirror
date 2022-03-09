using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories
{
    public interface ISharedMakeoverRepository
    {
        SharedMakeover GetSharedMakeover(long id);
        SharedMakeover GetSharedMakeover(string urlCode);
        long Save(SharedMakeover shared, IEnumerable<Expression<Func<SharedMakeover, object>>> properties);
    }
}