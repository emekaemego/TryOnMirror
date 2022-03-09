using System;
using System.ComponentModel.DataAnnotations;

namespace SymaCord.TryOnMirror.UI.Web.ViewModels
{
    public class NewBookingCommentModel
    {
        public Guid BookingId { get; set; }

        [Required(ErrorMessage="Message is required")]
        public string Message { get; set; }
    }
}