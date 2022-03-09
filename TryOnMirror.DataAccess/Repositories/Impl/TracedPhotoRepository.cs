using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataAccess.Repositories.Impl
{
    public class TracedPhotoRepository : ITracedPhotoRepository
    {
        private ICache _cache;

        public TracedPhotoRepository(ICache cache)
        {
            _cache = cache;
        }

        public IEnumerable<TracedPhoto> GetTracedPhotos(string userEmail, string seach, bool isModel, int? page, int maxRows)
        {
            string key = "TracedPhotos_" + userEmail + "_" + seach + "_" + isModel + "_" + page + "_" + maxRows +
                         "_GetTracedPhotos";

            var result = new List<TracedPhoto>();

            if (_cache.Exists(key))
                result = (List<TracedPhoto>) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.TracedPhotos select x;

                    if (!string.IsNullOrEmpty(seach))
                    {
                        oq = oq.Where(x => x.PhotoTitle.Contains(seach));
                    }

                    if(isModel)
                    {
                        oq = oq.Where(x => x.IsModel);
                    }

                    result = oq.OrderByDescending(x=>x.DateCreated).Page(page, maxRows).ToList();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public string[] GetTracedPhotosFileName(string userEmail, string seach, bool isModel, int? page, int maxRows)
        {
            string key = "TracedPhotos_" + userEmail + "_" + seach + "_" + isModel + "_" + page + "_" + maxRows +
                         "_GetTracedPhotosFileName";

            var result = new string[]{};

            if (_cache.Exists(key))
                result = (string[])_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.TracedPhotos select x;

                    if (!string.IsNullOrEmpty(seach))
                    {
                        oq = oq.Where(x => x.PhotoTitle.Contains(seach));
                    }

                    if (isModel)
                    {
                        oq = oq.Where(x => x.IsModel);
                    }

                    result = oq.OrderByDescending(x => x.DateCreated).Select(x => x.FileName).Page(page, maxRows).ToArray();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public TracedPhoto GetTracedPhoto(long id)
        {
            string key = "TracedPhoto_" + id + "_GetTracedPhoto";
            TracedPhoto result = null;

            if (_cache.Exists(key))
                result = (TracedPhoto) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.TracedPhotos where x.TraceId == id select x;
                    result = oq.FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public TracedPhoto GetTracedPhoto(string fileName)
        {
            string key = "TracedPhoto_" + fileName + "_GetTracedPhoto";
            TracedPhoto result = null;

            if (_cache.Exists(key))
                result = (TracedPhoto)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.TracedPhotos where x.FileName == fileName select x;
                    result = oq.FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public long GetTracedId(string fileName)
        {
            string key = "TracedPhoto_" + fileName + "_GetTracedId";
            long result = 0;

            if (_cache.Exists(key))
                result = (long)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.TracedPhotos.Where(x=>x.FileName == fileName).Select(x=>x.TraceId).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public bool IsModel(string fileName, out long traceId)
        {
            string key = "TracedPhoto_" + fileName + "_IsModel";
            bool result = false;
            traceId = 0;
            TracedPhoto obj = null;

            if (_cache.Exists(key))
            {
                obj = (TracedPhoto)_cache.Get(key);
                result = obj.IsModel;
                traceId = obj.TraceId;
            }
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    obj = dc.TracedPhotos.Where(x => x.FileName == fileName).ToList()
                        .Select(x => new TracedPhoto { TraceId = x.TraceId, IsModel = x.IsModel }).FirstOrDefault();

                    //var rec = (from t in dc.TracedPhotos
                    //            where t.FileName == fileName
                    //            select new TraceLite {IsModel = t.IsModel, TraceId = t.TraceId}).FirstOrDefault();

                    if (obj != null)
                    {
                        result = obj.IsModel;
                        traceId = obj.TraceId;
                        
                        _cache.Set(key, obj);
                    }
                }
            }

            return result;
        }

        private class TraceLite
        {
            public long TraceId { get; set; }

            public bool IsModel { get; set; }
        }
        public TracedPhoto GetTracedPhotoByAnonymouseId(Guid anonymouseId)
        {
            string key = "TracedPhoto_" + anonymouseId + "_GetTracedPhotoByAnonymouseId";
            TracedPhoto result = null;

            if (_cache.Exists(key))
                result = (TracedPhoto) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.TracedPhotos.FirstOrDefault(x => x.AnonymouseId == anonymouseId);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public TracedPhoto GetTracedPhotoByFileName(string fileName)
        {
            string key = "TracedPhoto_" + fileName + "_GetTracedPhotoByFileName";
            TracedPhoto result = null;

            if (_cache.Exists(key))
                result = (TracedPhoto) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.TracedPhotos.FirstOrDefault(x => x.FileName.Equals(fileName));

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public long Save(TracedPhoto traced, IEnumerable<Expression<Func<TracedPhoto, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                if (properties != null)
                {
                    dc.TracedPhotos.Attach(traced);
                    ObjectStateEntry entry = ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager
                        .GetObjectStateEntry(traced);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }
                else
                {
                    dc.TracedPhotos.Add(traced);
                }

                dc.SaveChanges();
            }

            return traced.TraceId;
        }

        public void Delete(long id)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                var traced = new TracedPhoto {TraceId = id};
                dc.TracedPhotos.Attach(traced);

                ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager.ChangeObjectState(traced,
                                                                                                EntityState.Deleted);

                dc.SaveChanges();
            }
        }
    }
}
