using System.Collections.Generic;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services
{
    public interface IBrandService
    {
        IEnumerable<Brand> GetBrands(int? categoryId);
    }
}