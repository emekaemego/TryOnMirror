using System.ComponentModel.DataAnnotations;

namespace SymaCord.TryOnMirror.UI.Web.ViewModels
{
    public class ContactModel
    {
        [Required(ErrorMessage="Your name is required")]
        public string Name { get; set; }

        //public string Compnay { get; set; }
        [Required(ErrorMessage="Your email address is required")]
        public string Email { get; set; }

        //public string Phone { get; set; }
        [Required(ErrorMessage="Your message is required")]
        public string Message { get; set; }
    }
}