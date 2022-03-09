using System.ComponentModel.DataAnnotations;

namespace SymaCord.TryOnMirror.UI.Web.ViewModels
{
    public class EditSalonModel
    {
        public string Hook { get; set; }

        [Required(ErrorMessage="Salon Name is required")]
        public string SalonName { get; set; }

        public string About { get; set; }
    }
}