using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymaCord.TryOnMirror.Core;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories.Impl
{
    public class BrandRepository : IBrandRepository
    {
        private ICache _cache;

        public BrandRepository(ICache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Brand> GetBrands(int? categoryId)
        {
            string key = "Brands_" + categoryId + "_GetBrands";

            var result = new List<Brand>();

            if (_cache.Exists(key))
                result = (List<Brand>)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.Brands select x;

                    if (categoryId.HasValue)
                    {
                        oq = oq.Where(x => x.ItemCategories.Any(c => c.CategoryId == categoryId));
                    }

                    //result = oq.OrderByDescending(x => x.DateCreated).ToList();

                    //string query = "SELECT VALUE b FROM {0}.Brands AS b ";
                    //var param = new Dictionary<string, object>();

                    //if (categoryId.HasValue)
                    //{
                    //    query = query.AppendClause() +
                    //            " EXISTS(SELECT VALUE c FROM b.ItemCategories AS c WHERE c.CategoryId=@categoryId) ";
                    //    param.Add("categoryId", categoryId.Value);
                    //}

                    //var context = ((IObjectContextAdapter) dc).ObjectContext;
                    //var oq = new ObjectQuery<Brand>(string.Format(query, context.DefaultContainerName), context, 
                    //    MergeOption.NoTracking);

                    //foreach (var p in param)
                    //{
                    //    oq.Parameters.Add(new ObjectParameter(p.Key, p.Value));
                    //}

                    result = oq.OrderBy(x => x.BrandName).ToList();

                    _cache.Set(key, result);
                }
            }
            return result;
        }
    }
}
