using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
    public class SalonService : ISalonService
    {
        private ISalonRepository _repository;
        private ICache _cache;

        public SalonService(ISalonRepository repository, ICache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public IEnumerable<Salon> GetSalons(string seach, int? userId, int? page, int maxRows)
        {
            return _repository.GetSalons(seach, userId, page, maxRows);
        }

        public IEnumerable<Salon> GetSalonsLite(string seach, int? userId, int? page, int maxRows)
        {
            return _repository.GetSalonsLite(seach, userId, page, maxRows);
        }

        public Salon GetSalon(int id)
        {
            return _repository.GetSalon(id);
        }

        public Salon GetSalon(string hook)
        {
            return _repository.GetSalon(hook);
        }

        public Salon GetSalon(Guid identifier)
        {
            return _repository.GetSalon(identifier);
        }

        public Salon GetSalonWithContactInfo(string hook)
        {
            return _repository.GetSalonWithContactInfo(hook);
        }

        public Salon GetSalonWithContactInfo(int id)
        {
            return _repository.GetSalonWithContactInfo(id);
        }

        public Salon GetSalonWithContactInfo(Guid identifier)
        {
            return _repository.GetSalonWithContactInfo(identifier);
        }

        public bool CanModify(int userId, string hook)
        {
            return _repository.CanModify(userId, hook);
        }

        public int GetSalonId(Guid identifier)
        {
            return _repository.GetSalonId(identifier);
        }

        public int GetSalonId(string hook)
        {
            return _repository.GetSalonId(hook);
        }
        
        public int GetSalonUserId(int id)
        {
            return _repository.GetSalonUserId(id);
        }

        public int GetSalonUserId(string hook)
        {
            return _repository.GetSalonUserId(hook);
        }

        public bool HasSalon(int userId)
        {
            return _repository.HasSalon(userId);
        }

        public int Save(Salon salon, IEnumerable<Expression<Func<Salon, object>>> properties)
        {
            var result = _repository.Save(salon, properties);

            _cache.DeleteItems("salon_" + salon.SalonId + "_");
            _cache.DeleteItems("salon_" + salon.Identifier + "_");

            if (salon.SalonId == 0)
            {
                _cache.DeleteItems("salons_");
            }

            return result;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
            _cache.DeleteItems("salon_" + id + "_");
        }
    }
}