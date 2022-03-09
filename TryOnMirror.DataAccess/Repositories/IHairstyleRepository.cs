using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories
{
    public interface IHairstyleRepository
    {
        IEnumerable<Hairstyle> GetHairstyles(string seach, int? page, int maxRows);
        string[] GetHairstyleNames(string seach, int? page, int maxRows);
        Hairstyle GetHairstyle(long id);
        Hairstyle GetHairstyle(string fileName);
        int Save(Hairstyle hairstyle, IEnumerable<Expression<Func<Hairstyle, object>>> properties);
        void Delete(int id);
        int GetHairstyleId(string fileName);
        int GetHairstylesCount(string seach);
    }
}