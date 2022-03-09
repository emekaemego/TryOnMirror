using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services
{
    public interface ISharedMakeoverService
    {
        SharedMakeover GetSharedMakeover(long id);
        SharedMakeover GetSharedMakeover(string urlCode);
        long Save(SharedMakeover shared, IEnumerable<Expression<Func<SharedMakeover, object>>> properties);
    }
}