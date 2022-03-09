using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
    public class GlassService : IGlassService
    {
        private IGlassRepository _repository;
        private ICache _cache;

        public GlassService(IGlassRepository repository, ICache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public IEnumerable<Glass> GetGlasses(int? categoryId, string seach, int? page, int maxRows)
        {
            return _repository.GetGlasses(categoryId, seach, page, maxRows);
        }

        public Glass GetGlass(int id)
        {
            return _repository.GetGlass(id);
        }

        public Glass GetGlass(string fileName)
        {
            return _repository.GetGlass(fileName);
        }

        public int GetGlassId(string fileName)
        {
            return _repository.GetGlassId(fileName);
        }

        public int Save(Glass glass, IEnumerable<Expression<Func<Glass, object>>> properties)
        {
            var result = _repository.Save(glass, properties);

            _cache.DeleteItems("glass_" + glass.GlassId + "_");
            _cache.DeleteItems("glass_" + glass.FileName + "_");
            _cache.DeleteItems("glasses_");

            return result;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);

            _cache.DeleteItems("glass_" + id + "_");
            _cache.DeleteItems("glasses_");
        }
    }
}
