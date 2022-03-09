using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SymaCord.VirtualMakeover.ViewModel
{
   public class UploadCalibrateModel
    {
       [DisplayName("Model type")]
       [Required(ErrorMessage="Model type is required")]
       public int ModelTypeId { get; set; }
    }
}
