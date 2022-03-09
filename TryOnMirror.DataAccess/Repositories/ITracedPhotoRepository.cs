using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories
{
    public interface ITracedPhotoRepository
    {
        IEnumerable<TracedPhoto> GetTracedPhotos(string userEmail, string seach, bool isModel, int? page, int maxRows);
        TracedPhoto GetTracedPhoto(long id);
        TracedPhoto GetTracedPhoto(string fileName);
        TracedPhoto GetTracedPhotoByFileName(string fileName);
        TracedPhoto GetTracedPhotoByAnonymouseId(Guid anonymouseId);
        long Save(TracedPhoto traced, IEnumerable<Expression<Func<TracedPhoto, object>>> properties);
        void Delete(long id);
        long GetTracedId(string fileName);
        string[] GetTracedPhotosFileName(string userEmail, string seach, bool isModel, int? page, int maxRows);
        bool IsModel(string fileName, out long traceId);
    }
}