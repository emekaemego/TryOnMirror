using System;
using System.ComponentModel.DataAnnotations;

namespace SymaCord.TryOnMirror.UI.Web.ViewModels
{
    public class NewHairstyleBookingModel
    {
        public string Hook { get; set; }

        [Required(ErrorMessage="Photo file name not specified")]
        public string PhotoFileName { get; set; }

        [Required(ErrorMessage = "Hairstyle file name not specified")]
        public string HairstyleFileName { get; set; }

        [Required(ErrorMessage="Booking date is required"), Display(Name="Date & Time")]
        public DateTime BookingDate { get; set; }

        public byte[] ImageData { get; set; }

        public string Note { get; set; }
    }
}