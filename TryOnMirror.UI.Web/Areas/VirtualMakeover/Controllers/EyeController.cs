using System.Web.Mvc;
using AutoMapper;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.ContractResolver;
using SymaCord.TryOnMirror.UI.Web.Utils;
using SymaCord.TryOnMirror.UI.Web.ViewModels;

namespace SymaCord.TryOnMirror.UI.Web.Areas.VirtualMakeover.Controllers
{
    public class EyeController : Controller
    {
        private IGlassService _glassService;
        private IContactLensService _contactLensService;

        public EyeController(IGlassService glassService, IContactLensService contactLensService)
        {
            _glassService = glassService;
            _contactLensService = contactLensService;
        }

        public ActionResult Index(int? p, int? cat)
        {
            var model = _glassService.GetGlasses(cat, null, p, 20);

            return Json(new { Success = true, html = this.Partial("PartialEye", model) }, JsonRequestBehavior.AllowGet);
        }

        [ActionName("try-on-glass")]
        public ActionResult TryOnGlass(string fn/*fileName*/)
        {
            var glass = Mapper.Map<Glass, GlassModel>(_glassService.GetGlass(fn));
            var imageSrc =
                Helper.RelativeFromAbsolutePath(Helper.ConstructFilePath(fn + "_vm.png", Helper.EyeWearFilePath, false), Request);
                //Url.RouteUrl(new { action = "image-file", controller = "home", fn, ext = "png", area = "" });

            var data = CamelCaseJsonSerializer.SerializeObject(glass);

            return Json(new { Success = true, Data = data, imageSrc }, JsonRequestBehavior.AllowGet);
        }

        [ActionName("try-on-contact-lens")]
        public ActionResult TryOnContactLens(string fn/*fileName*/)
        {
            var contact = Mapper.Map<ContactLens, ContactLensModel>(_contactLensService.GetContactLens(fn));
            var imageSrc =Helper.RelativeFromAbsolutePath
                (Helper.ConstructFilePath(fn + "_vm.png", Helper.EyeWearFilePath, false), Request);
                
            var data = CamelCaseJsonSerializer.SerializeObject(contact);

            return Json(new { Success = true, Data = new { data, imageSrc } }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Eyeglasses(int? p)
        {
            var model = _glassService.GetGlasses(2, null, p, 20);

            return Json(new { Success = true, html = this.Partial("PartialEyeglasses", model) },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult Sunglasses(int? p)
        {
            var model = _glassService.GetGlasses(1, null, p, 20);

            return Json(new { Success = true, html = this.Partial("PartialSunglasses", model) }, JsonRequestBehavior.AllowGet);
        }

        [ActionName("contact-lenses")]
        public ActionResult ContactLenses(int? p)
        {
            var model = _contactLensService.GetContactLensFileNames(null, p, 20);

            return Json(new { Success = true, html = this.Partial("PartialContactLenses", model) }, JsonRequestBehavior.AllowGet);
        }

    }
}
