using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services
{
    public interface ISalonService
    {
        IEnumerable<Salon> GetSalons(string seach, int? userId, int? page, int maxRows);
        Salon GetSalon(int id);
        Salon GetSalon(Guid identifier);
        Salon GetSalonWithContactInfo(int id);
        int GetSalonId(Guid identifier);
        int Save(Salon salon, IEnumerable<Expression<Func<Salon, object>>> properties);
        void Delete(int id);
        Salon GetSalonWithContactInfo(Guid identifier);
        int GetSalonUserId(int id);
        Salon GetSalonWithContactInfo(string hook);
        int GetSalonId(string hook);
        Salon GetSalon(string hook);
        int GetSalonUserId(string hook);
        bool HasSalon(int userId);
        IEnumerable<Salon> GetSalonsLite(string seach, int? userId, int? page, int maxRows);
        bool CanModify(int userId, string hook);
    }
}