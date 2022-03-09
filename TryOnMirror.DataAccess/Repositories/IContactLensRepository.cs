using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories
{
    public interface IContactLensRepository
    {
        IEnumerable<ContactLens> GetContactLenses(string seach, int? page, int maxRows);
        string[] GetContactLensFileNames(string seach, int? page, int maxRows);
        ContactLens GetContactLens(long id);
        ContactLens GetContactLens(string fileName);
        int Save(ContactLens contact, IEnumerable<Expression<Func<ContactLens, object>>> properties);
        void Delete(int id);
    }
}