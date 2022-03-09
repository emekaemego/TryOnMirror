using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataAccess.Repositories;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services.Impl
{
    public class HairstyleBookingService : IHairstyleBookingService
    {
        private IHairstyleBookingRepository _repository;
        private ICache _cache;

        public HairstyleBookingService(IHairstyleBookingRepository repository, ICache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public IEnumerable<HairstyleBooking> GetHairstyleBookings(Guid salonIndentifier, int? page, int maxRows)
        {
            return _repository.GetHairstyleBookings(salonIndentifier, page, maxRows);
        }

        public IEnumerable<HairstyleBooking> GetHairstyleBookings(int userId, int? page, int maxRows)
        {
            return _repository.GetHairstyleBookings(userId, page, maxRows);
        }

        public IEnumerable<CustomBooking> GetHairstyleCustomBookings(int? userId, string search, int? page, int maxRows)
        {
            return _repository.GetHairstyleCustomBookings(userId, search, page, maxRows);
        }

        public HairstyleBooking GetHairstyleBooking(int id)
        {
            return _repository.GetHairstyleBooking(id);
        }

        public HairstyleBooking GetHairstyleBookingWithComments(int id)
        {
            return _repository.GetHairstyleBookingWithComments(id);
        }

        public HairstyleBooking GetHairstyleBooking(Guid identifier)
        {
            return _repository.GetHairstyleBooking(identifier);
        }

        public HairstyleBooking GetHairstyleBookingLite(Guid identifier)
        {
            return _repository.GetHairstyleBookingLite(identifier);
        }

        public bool CanModify(int userId, Guid identifier)
        {
            return _repository.CanModify(userId, identifier);
        }

        public int GetBookedByUserId(Guid identifier)
        {
            return _repository.GetBookedByUserId(identifier);
        }

        public IEnumerable<BookingStatus> GetBookingStatuses()
        {
            return _repository.GetBookingStatuses();
        }

        public int GetHairstyleBookingId(Guid identifier)
        {
            return _repository.GetHairstyleBookingId(identifier);
        }

        public int Save(HairstyleBooking booking, IEnumerable<Expression<Func<HairstyleBooking, object>>> properties)
        {
            var result = _repository.Save(booking, properties);

            _cache.DeleteItems("hairstylebooking_" + booking.BookingId + "_");
            _cache.DeleteItems("hairstylebookings_" + booking.UserId + "_");

            return result;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);

            _cache.DeleteItems("hairstylebooking_" + id + "_");
        }
    }
}
