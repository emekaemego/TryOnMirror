using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
    public class HairstyleService : IHairstyleService
    {
        private ICache _cache;
        private IHairstyleRepository _repository;

        public HairstyleService(ICache cache, IHairstyleRepository repository)
        {
            _repository = repository;
            _cache = cache;
        }

        public IEnumerable<Hairstyle> GetHairstyles(string seach, int? page, int maxRows)
        {
            return _repository.GetHairstyles(seach, page, maxRows);
        }

        public IEnumerable<Hairstyle> GetHairstyles(string seach, int? page, int maxRows, out int totalCount)
        {
            totalCount = _repository.GetHairstylesCount(seach);
            return _repository.GetHairstyles(seach, page, maxRows);
        }

        public string[] GetHairstyleNames(string seach, int? page, int maxRows)
        {
            return _repository.GetHairstyleNames(seach, page, maxRows);
        }

        public string[] GetHairstyleNames(string seach, int? page, int maxRows, out int totalCount)
        {
            totalCount = _repository.GetHairstylesCount(seach);
            return _repository.GetHairstyleNames(seach, page, maxRows);
        }

        public Hairstyle GetHairstyle(long id)
        {
            return _repository.GetHairstyle(id);
        }

        public Hairstyle GetHairstyle(string fileName)
        {
            return _repository.GetHairstyle(fileName);
        }

        public int GetHairstyleId(string fileName)
        {
            return _repository.GetHairstyleId(fileName);
        }

        public int Save(Hairstyle hairstyle, IEnumerable<Expression<Func<Hairstyle, object>>> properties)
        {
            var result = _repository.Save(hairstyle, properties);

            _cache.DeleteItems("hairstyle_" + hairstyle.HairstyleId + "_");
            _cache.DeleteItems("hairstyle_" + hairstyle.FileName + "_");
            _cache.DeleteItems("hairstyles_");

            return result;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);

            _cache.DeleteItems("hairstyle_" + id + "_");
            _cache.DeleteItems("hairstyles_");
        }
    }
}
