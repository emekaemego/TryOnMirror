using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories
{
    public interface IHairColorRepository
    {
        IEnumerable<HairColor> GetHairColors(string seach, int? page, int maxRows);
        string[] GetHairColorNames(string seach, int? page, int maxRows);
        int GetHairColorsCount(string seach);
        HairColor GetHairColor(int id);
        HairColor GetHairColor(string fileName);
        int GetHairColorId(string fileName);
        int Save(HairColor color, IEnumerable<Expression<Func<HairColor, object>>> properties);
        void Delete(int id);
    }
}