using System;

namespace SymaCord.TryOnMirror.Entities
{
    public class CustomBooking
    {
        public string UserName { get; set; }

        public string Note { get; set; }

        public bool HasUnviewed { get; set; }

        public int StatusId { get; set; }

        public string ImageFileName { get; set; }

        public DateTime BookingDate { get; set; }

        public string SalonHook { get; set; }

        public Guid Identifier { get; set; }

        public int CommentCount { get; set; }
    }
}