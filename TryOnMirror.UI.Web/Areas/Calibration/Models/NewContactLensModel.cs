using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SymaCord.TryOnMirror.UI.Web.Areas.Calibration.Models
{
    public class NewContactLensModel
    {
        [Utils.ValidationAttributes.FileExtensions("png|gif")]
        [Required(ErrorMessage = "No file selected")]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}