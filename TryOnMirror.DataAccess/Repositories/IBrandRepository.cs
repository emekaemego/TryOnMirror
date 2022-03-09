using System.Collections.Generic;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories
{
    public interface IBrandRepository
    {
        IEnumerable<Brand> GetBrands(int? categoryId);
    }
}