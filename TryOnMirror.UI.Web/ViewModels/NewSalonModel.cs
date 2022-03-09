using System.ComponentModel.DataAnnotations;

namespace SymaCord.TryOnMirror.UI.Web.ViewModels
{
    public class NewSalonModel
    {
        [Required(ErrorMessage = "Salon name is required"), Display(Name = "Salon Name")]
        public string SalonName { get; set; }

        public AddressModel Address { get; set; }
    }
}