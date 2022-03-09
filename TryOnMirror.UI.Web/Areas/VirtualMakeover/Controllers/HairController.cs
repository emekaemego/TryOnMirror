using System;
using System.Web.Mvc;
using AutoMapper;
using PagedList;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.ContractResolver;
using SymaCord.TryOnMirror.UI.Web.Utils;
using SymaCord.TryOnMirror.UI.Web.Utils.Impl;
using SymaCord.TryOnMirror.UI.Web.ViewModels;

namespace SymaCord.TryOnMirror.UI.Web.Areas.VirtualMakeover.Controllers
{
    public class HairController : Controller
    {
        //
        // GET: /VirtualMakeover/Hair/

        private readonly IHairstyleService _hairstyleService;
        private IHairColorService _hairColorService;

        public HairController(IHairstyleService hairstyleService, IHairColorService hairColorService)
        {
            _hairstyleService = hairstyleService;
            _hairColorService = hairColorService;
        }

        public ActionResult Index(string s, bool? pgr, int? p)
        {
            int count = 0;
            var hairstyles = _hairstyleService.GetHairstyleNames(s, p, 4, out count);
            p = p ?? 1;

            //ViewBag.TotalCount = count;

            var model = new StaticPagedList<string>(hairstyles, p.Value, 4, count);
            ViewBag.HairColors  = _hairColorService.GetHairColors(null, null, int.MaxValue);
            //ViewBag.OnePageOfUsers = modelAsIPagedList;

            if(pgr.HasValue && pgr.Value)
                return Json(new { Success = true, html = this.Partial("PartialHairstyles", model) }, 
                    JsonRequestBehavior.AllowGet);

            return Json(new { Success = true, html = this.Partial("PartialHair",model) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Colors()
        {
            var model = _hairColorService.GetHairColors(null, null, int.MaxValue);

            return Json(new { Success = true, html = this.Partial("PartialHairColors", model) }, 
                JsonRequestBehavior.AllowGet);
        }

        [ActionName("try-on-hairstyle")]
        public ActionResult TryOnHairstyle(string fn/*fileName*/)
        {
            var hairstyle = _hairstyleService.GetHairstyle(fn);

            if(hairstyle != null)
            {
                var model = Mapper.Map<Hairstyle, HairstyleModel>(hairstyle);
                var absPath = Helper.ConstructFilePath(fn + "_vm.png", Helper.HairstyleFilePath, false);

                var imageSrc = Helper.RelativeFromAbsolutePath(absPath, Request);// +"?" + DateTime.Now.Ticks.ToString();
                    //Url.RouteUrl(new { action = "image-file", controller = "home", fn, ext = "png", area = "" });

                var data = CamelCaseJsonSerializer.SerializeObject(model);

                return Json(new { Success = true, Data = data, imageSrc, fileName=fn }, JsonRequestBehavior.AllowGet);
            }

            return Json(new {Success = false, Message = "No hairstyle found for the specified file name"});
        }

        [ActionName("change-color")]
        public ActionResult ChangeColor(string fn/*fileName*/, int c)
        {
            var hairstyleId = _hairstyleService.GetHairstyleId(fn);

            if (hairstyleId > 0)
            {
                var absPath = Helper.ConstructFilePath(fn + "_vm.png", Helper.HairstyleFilePath, false);
                var color = _hairColorService.GetHairColor(c);

                var coloredPath = HairColoration.ColorHair(absPath, color.Hue, (float)color.Saturation, (float)color.Luminance);

                var imageSrc = Helper.RelativeFromAbsolutePath(coloredPath, Request);// +"?" + DateTime.Now.Ticks.ToString();
                //Url.RouteUrl(new { action = "image-file", controller = "home", fn, ext = "png", area = "" });
                
                return Json(new { Success = true, imageSrc, fileName = fn }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = false, Message = "No hairstyle found for the specified file name" });
        }
    }
}
