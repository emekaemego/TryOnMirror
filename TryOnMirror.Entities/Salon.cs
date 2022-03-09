//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SymaCord.TryOnMirror.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Salon
    {
        public Salon()
        {
            this.HairstyleBookings = new HashSet<HairstyleBooking>();
        }
    
        public int SalonId { get; set; }
        public string SalonName { get; set; }
        public int AddressId { get; set; }
        public Nullable<int> PhoneId { get; set; }
        public int UserId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public byte[] TimeStamp { get; set; }
        public System.Guid Identifier { get; set; }
        public string Hook { get; set; }
        public string About { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string PhoneNumbers { get; set; }
    
        public virtual Address Address { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<HairstyleBooking> HairstyleBookings { get; set; }
    }
}
