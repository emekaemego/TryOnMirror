using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
    public class TracedPhotoService : ITracedPhotoService
    {
        private ITracedPhotoRepository _tracedPhotoRepository;
        private ICache _cache;

        public TracedPhotoService(ICache cache, ITracedPhotoRepository tracedPhotoRepository)
        {
            _tracedPhotoRepository = tracedPhotoRepository;
            _cache = cache;
        }

        public IEnumerable<TracedPhoto> GetTracedPhotos(string userEmail, string seach, bool isModel, int? page, int maxRows)
        {
            return _tracedPhotoRepository.GetTracedPhotos(userEmail, seach, isModel, page, maxRows);
        }

        public string[] GetTracedPhotosFileName(string userEmail, string seach, bool isModel, int? page, int maxRows)
        {
            return _tracedPhotoRepository.GetTracedPhotosFileName(userEmail, seach, isModel, page, maxRows);
        }

        public TracedPhoto GetTracedPhoto(long id)
        {
            return _tracedPhotoRepository.GetTracedPhoto(id);
        }

        public TracedPhoto GetTracedPhoto(string fileName)
        {
            return _tracedPhotoRepository.GetTracedPhoto(fileName);
        }
        
        public TracedPhoto GetTracedPhotoByFileName(string fileName)
        {
            return _tracedPhotoRepository.GetTracedPhotoByFileName(fileName);
        }

        public long GetTracedId(string fileName)
        {
            return _tracedPhotoRepository.GetTracedId(fileName);
        }

        public bool IsModel(string fileName, out long traceId)
        {
            return _tracedPhotoRepository.IsModel(fileName, out traceId);
        }

        public TracedPhoto GetTracedPhotoByAnonymouseId(Guid anonymouseId)
        {
            return _tracedPhotoRepository.GetTracedPhotoByAnonymouseId(anonymouseId);
        }

        public long Save(TracedPhoto traced, IEnumerable<Expression<Func<TracedPhoto, object>>> properties)
        {
            var id = _tracedPhotoRepository.Save(traced, properties);

            _cache.DeleteItems("tracedphoto_" + traced.TraceId + "_");
            _cache.DeleteItems("tracedphoto_" + traced.AnonymouseId + "_");

            return id;
        }

        public void Delete(long id)
        {
            _tracedPhotoRepository.Delete(id);

            _cache.DeleteItems("tracedphoto_" + id + "_");
            _cache.DeleteItems("tracephotos_");
        }
    }
}
