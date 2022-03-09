using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.DataService.Services
{
    public interface IHairstyleBookingService
    {
        IEnumerable<HairstyleBooking> GetHairstyleBookings(Guid salonIndentifier, int? page, int maxRows);
        HairstyleBooking GetHairstyleBooking(int id);
        HairstyleBooking GetHairstyleBookingWithComments(int id);
        HairstyleBooking GetHairstyleBooking(Guid identifier);
        int Save(HairstyleBooking booking, IEnumerable<Expression<Func<HairstyleBooking, object>>> properties);
        void Delete(int id);
        IEnumerable<HairstyleBooking> GetHairstyleBookings(int userId, int? page, int maxRows);
        IEnumerable<CustomBooking> GetHairstyleCustomBookings(int? userId, string search, int? page, int maxRows);
        int GetHairstyleBookingId(Guid identifier);
        IEnumerable<BookingStatus> GetBookingStatuses();
        HairstyleBooking GetHairstyleBookingLite(Guid identifier);
        bool CanModify(int userId, Guid identifier);
        int GetBookedByUserId(Guid identifier);
    }
}