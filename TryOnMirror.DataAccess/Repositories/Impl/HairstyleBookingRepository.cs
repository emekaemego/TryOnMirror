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
    public class HairstyleBookingRepository : IHairstyleBookingRepository
    {
        private ICache _cache;

        public HairstyleBookingRepository(ICache cache)
        {
            _cache = cache;
        }

        public IEnumerable<HairstyleBooking> GetHairstyleBookings(Guid salonIndentifier, int? page, int maxRows)
        {
            string key = "HairstyleBookings_" + salonIndentifier + "_" + page + "_" + maxRows + "_GetHairstyleBookings";

            var result = new List<HairstyleBooking>();

            if (_cache.Exists(key))
                result = (List<HairstyleBooking>) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.HairstyleBookings select x;

                    oq = oq.Where(x => x.Salon.Identifier == salonIndentifier);

                    result = oq.OrderByDescending(x => x.DateCreated).Page(page, maxRows).ToList();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public IEnumerable<HairstyleBooking> GetHairstyleBookings(int userId, int? page, int maxRows)
        {
            string key = "HairstyleBookings_" + userId + "_" + page + "_" + maxRows + "_GetHairstyleBookings";

            var result = new List<HairstyleBooking>();

            if (_cache.Exists(key))
                result = (List<HairstyleBooking>)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.HairstyleBookings.Include("UserProfile").Include("Salon") select x;

                    oq = oq.Where(x => x.UserId == userId || x.Salon.UserId == userId);

                    result = oq.OrderByDescending(x => x.DateCreated).Page(page, maxRows).ToList();

                    _cache.Set(key, result);
                }
            }
            return result;
        }

        public IEnumerable<CustomBooking> GetHairstyleCustomBookings(int? userId, string search, int? page, int maxRows)
        {
            string key = "HairstyleBookings_" + userId + "_" + search + "_" + page + "_" + maxRows
                         + "_ GetHairstyleCustomBookings";

            var result = new List<CustomBooking>();
            
            if (_cache.Exists(key))
                result = (List<CustomBooking>)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    var oq = from x in dc.HairstyleBookings.Include("Salon") select x;

                    if(userId.HasValue)
                    {
                        oq = oq.Where(x => x.UserId == userId || x.Salon.UserId == userId);
                    }

                    if(!string.IsNullOrEmpty(search))
                    {
                        if(search.StartsWith("@"))
                        {
                            oq = oq.Where(x => x.Salon.Hook.Equals(search.Replace("@", "")));
                        }else
                        {
                            //TODO: Perform search
                        }
                    }

                    result = oq.OrderByDescending(x => x.DateCreated)
                        .Select(x=> new CustomBooking
                            {
                                Identifier = x.Identifier,
                                SalonHook = x.Salon.Hook,
                                UserName = x.UserProfile.UserName,
                                StatusId = x.StatusId,
                                Note = x.Note,
                                HasUnviewed = !x.Viewed,
                                ImageFileName = x.ImageFileName,
                                BookingDate = x.BookingDate,
                                CommentCount = x.Comments.Count + ((!string.IsNullOrEmpty(x.Note))? 1:0)
                            })
                        .Page(page, maxRows).ToList();

                    _cache.Set(key, result);
                }
            }
            return result;
        }
        
        public HairstyleBooking GetHairstyleBooking(int id)
        {
            string key = "HairstyleBooking_" + id + "_GetHairstyleBooking";
            HairstyleBooking result = null;

            if (_cache.Exists(key))
                result = (HairstyleBooking) _cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.HairstyleBookings.FirstOrDefault(x => x.BookingId == id);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public HairstyleBooking GetHairstyleBookingWithComments(int id)
        {
            string key = "HairstyleBooking_" + id + "_GetHairstyleBookingWithComments";
            HairstyleBooking result = null;

            if (_cache.Exists(key))
                result = (HairstyleBooking)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.HairstyleBookings.Include("Comments").FirstOrDefault(x => x.BookingId == id);

                    _cache.Set(key, result);
                }
            }

            return result;
        }
        
        public HairstyleBooking GetHairstyleBooking(Guid identifier)
        {
            string key = "HairstyleBooking_" + identifier + "_GetHairstyleBooking";
            HairstyleBooking result = null;

            if (_cache.Exists(key))
                result = (HairstyleBooking)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.HairstyleBookings.Include("Comments").Include("Comments.UserProfile")
                        .Include("UserProfile").Include("Salon")
                        .FirstOrDefault(x => x.Identifier == identifier);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public HairstyleBooking GetHairstyleBookingLite(Guid identifier)
        {
            string key = "HairstyleBooking_" + identifier + "_GetHairstyleBookingLite";
            HairstyleBooking result = null;

            if (_cache.Exists(key))
                result = (HairstyleBooking)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.HairstyleBookings.FirstOrDefault(x => x.Identifier == identifier);

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public int GetBookedByUserId(Guid identifier)
        {
            string key = "HairstyleBooking_" + identifier + "_GetBookedByUserId";
            int result;

            if (_cache.Exists(key))
                result = (int)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.HairstyleBookings.Where(x => x.Identifier == identifier).Select(x=>x.UserId)
                        .FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public bool CanModify(int userId, Guid identifier)
        {
            string key = "HairstyleBooking_" + userId + "_" + identifier + "_CanModify";
            var result = false;

            if (_cache.Exists(key))
                result = (bool)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result =
                        dc.HairstyleBookings
                        .Any(x => x.Identifier == identifier && (x.UserId == userId || x.Salon.UserId == userId));

                    _cache.Set(key, result);
                }
            }

            return result;
        }

        public IEnumerable<BookingStatus> GetBookingStatuses()
        {
            string key = "BookingStatus_GetBookingStatuses";

            List<BookingStatus> result;

            if (_cache.Exists(key))
                result = (List<BookingStatus>)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.BookingStatuses.OrderBy(x => x.StatusId).ToList();

                    _cache.Set(key, result);
                }
            }
            return result;
        }
        
        public int GetHairstyleBookingId(Guid identifier)
        {
            string key = "HairstyleBooking_" + identifier + "_GetHairstyleBookingId";
            int result = 0;

            if (_cache.Exists(key))
                result = (int)_cache.Get(key);
            else
            {
                using (var dc = new TryOnMirrorEntities())
                {
                    result = dc.HairstyleBookings.Where(x => x.Identifier == identifier)
                        .Select(x=>x.BookingId).FirstOrDefault();

                    _cache.Set(key, result);
                }
            }

            return result;
        }
        
        public int Save(HairstyleBooking booking, IEnumerable<Expression<Func<HairstyleBooking, object>>> properties)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                if (properties != null)
                {
                    dc.HairstyleBookings.Attach(booking);
                    ObjectStateEntry entry = ((IObjectContextAdapter) dc).ObjectContext.ObjectStateManager
                        .GetObjectStateEntry(booking);

                    foreach (var selector in properties)
                    {
                        string propertyName = selector.Body.PropertyToString();
                        entry.SetModifiedProperty(propertyName);
                    }
                }
                else
                {
                    dc.HairstyleBookings.Add(booking);
                }

                dc.SaveChanges();
            }

            return booking.BookingId;
        }

        public void Delete(int id)
        {
            using (var dc = new TryOnMirrorEntities())
            {
                var booking = new HairstyleBooking {BookingId = id};
                dc.HairstyleBookings.Attach(booking);

                ((IObjectContextAdapter) dc)
                    .ObjectContext.ObjectStateManager.ChangeObjectState(booking, EntityState.Deleted);

                dc.SaveChanges();
            }
        }
    }
}
