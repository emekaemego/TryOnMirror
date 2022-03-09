using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SymaCord.TryOnMirror.UI.Web.ViewModels
{
    public class NewHairstyleModel
    {
        [Utils.ValidationAttributes.FileExtensions("png")]
        [Required(ErrorMessage = "No file selected")]
        public HttpPostedFileBase HairstyleImage { get; set; }
    }
}