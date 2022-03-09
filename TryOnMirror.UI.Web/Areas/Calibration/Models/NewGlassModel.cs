using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SymaCord.TryOnMirror.UI.Web.Areas.Calibration.Models
{
    public class NewGlassModel
    {
        [Utils.ValidationAttributes.FileExtensions("png|gif")]
        [Required(ErrorMessage = "No file selected")]
        public HttpPostedFileBase GlassImage { get; set; }

        //public int Cat { get; set; }
    }
}