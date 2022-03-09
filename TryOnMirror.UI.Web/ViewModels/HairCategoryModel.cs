using System.ComponentModel.DataAnnotations;

namespace SymaCord.TryOnMirror.UI.Web.ViewModels
{
    public class HairCategoryModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required"), Display(Name = "Category name")]
        public string CategoryName { get; set; }
    }
}