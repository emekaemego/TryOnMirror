using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SymaCord.TryOnMirror.UI.Web.Areas.VirtualMakeover.Models
{
    public class NewTraceModel
    {
        public string PhotoTitle { get; set; }

        [Utils.ValidationAttributes.FileExtensions("jpg|jpeg|png")]
        [Required(ErrorMessage="No file selected")]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}