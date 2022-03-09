using System.ComponentModel.DataAnnotations;

namespace SymaCord.TryOnMirror.UI.Web.ViewModels
{
    public class AddressModel
    {
        public string Hook { get; set; }

        [Required(ErrorMessage="Address is required"), Display(Name="Address")]
        public string AddressLine1 { get; set; }

        [Required(ErrorMessage="City is required"), Display(Name="City")]
        public string City { get; set; }

        [Required(ErrorMessage="State is required"), Display(Name="State")]
        public string State { get; set; }
    }
}