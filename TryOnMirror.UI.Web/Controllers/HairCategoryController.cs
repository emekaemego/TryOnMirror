using System.Web.Mvc;
using AutoMapper;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.Utils;
using SymaCord.TryOnMirror.UI.Web.ViewModels;

namespace SymaCord.TryOnMirror.UI.Web.Controllers
{
    public class HairCategoryController : Controller
    {
        private IHairCategoryService _hairCategoryService;

        public HairCategoryController(IHairCategoryService hairCategoryService)
        {
            _hairCategoryService = hairCategoryService;
        }

        public ActionResult List(string s, int? p)
        {
            int max = 30;

            var model = _hairCategoryService.GetHairCategories(s, p, max);

            return View(model);
        }

        public ActionResult Add()
        {
            var model = new HairCategoryModel();

            return Json(new {Success = true, Data = this.Partial("PartialAddHairCategory", model)},
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(HairCategoryModel model)
        {
            if(ModelState.IsValid)
            {
                var category = Mapper.Map<HairCategoryModel, HairCategory>(model);
                var id = _hairCategoryService.Save(category, null);
                category.CategoryId = id;

                return Json(new {Success = true, Data = this.Partial("PartialCategory", category), 
                    Message="Hair category added successfully"});
            }

            return Json(new {Success = false, Data = this.Partial("PartialAddHairCategory", model)});
        }
    }
}
