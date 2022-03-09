using System.ComponentModel.DataAnnotations;

namespace SymaCord.TryOnMirror.UI.Web.ViewModels
{
    public class PhoneModel
    {
        [Display(Name="Phone Numbers")]
        public string PhoneNumbers { get; set; }
    }
}