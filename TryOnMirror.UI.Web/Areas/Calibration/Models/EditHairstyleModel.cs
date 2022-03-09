using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SymaCord.TryOnMirror.UI.Web.Areas.Calibration.Models
{
    public class EditHairstyleModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int W { get; set; }

        public int H { get; set; }

        [Display(Name="Brand")]
        public int? BrandId { get; set; }

        [Display(Name="Product Url")]
        public string ProductUrl { get; set; }

        [Display(Name = "Product Image")]
        public HttpPostedFileBase PackageImage { get; set; }

        //public int ScaledWidth { get; set; }

        //public int ScaledHeight { get; set; }
    }

    public class NewThumbImage
    {
        public string Id { get; set; }

        [Utils.ValidationAttributes.FileExtensions("png|jpg"), 
        Display(Name = "Upload file")]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}