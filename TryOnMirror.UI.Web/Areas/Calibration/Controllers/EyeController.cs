using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using Kaliko.ImageLibrary;
using SymaCord.TryOnMirror.Core.Imaging;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.Areas.Calibration.Models;
using SymaCord.TryOnMirror.UI.Web.Utils;
using SymaCord.TryOnMirror.UI.Web.Utils.Impl;
using SymaCord.TryOnMirror.UI.Web.ViewModels;

namespace SymaCord.TryOnMirror.UI.Web.Areas.Calibration.Controllers
{
    [Authorize(Roles="Admin")]
    public class EyeController : Controller
    {
        private IGlassService _glassService;
        private IContactLensService _contactLensService;
        private IWebContext _webContext;

        public EyeController(IGlassService glassService, IContactLensService contactLensService, IWebContext webContext)
        {
            _glassService = glassService;
            _contactLensService = contactLensService;
            _webContext = webContext;
        }

        public ActionResult Index(int? p, int? cat)
        {
            var model = _glassService.GetGlasses(cat, null, p, 20);

            return Json(new {Success = true, html = this.Partial("PartialEye", model)}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("new-glass")]
        public ActionResult NewGlass(NewGlassModel model, int cat)
        {
            if (ModelState.IsValid)
            {
                _webContext.RemoveCookie(CookieKeys.GlassTempFileName);
                _webContext.RemoveCookie(CookieKeys.GlassCatId);

                string fileName = "eyewear_" + Guid.NewGuid().ToString().ToLower().Replace("-", "_");
                var image = Image.FromStream(model.GlassImage.InputStream);

                var filePath = Helper.ConstructFilePath(fileName, Helper.TempFilePath, true);
                    // saveToPath + "/" + fileName;
                var directory = Path.GetDirectoryName(filePath);

                Helper.CreateDirectoryIfNotExist(directory);

                image.SavePng(filePath + ".png", 95);
                _webContext.SetCookieValue(CookieKeys.GlassTempFileName, fileName);
                _webContext.SetCookieValue(CookieKeys.GlassCatId, cat.ToString());

                var imageSrc = Helper.RelativeFromAbsolutePath(filePath+".png", Request);
                    //Url.RouteUrl(new
                    //{
                    //    action = "image-file",
                    //    controller = "home",
                    //    fn = fileName,
                    //    ext = "png",
                    //    t = true,
                    //    area = ""
                    //});

                return Json(new {Success = true, imageSrc});
            }

            return Json(new {Success = false});
        }

        [HttpPost, ActionName("save-new-glass")]
        public ActionResult SaveNewGlass(GlassModel model)
        {
            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileNameWithoutExtension(_webContext.GetCookieValue(CookieKeys.GlassTempFileName));

                var glass = Mapper.Map<GlassModel, Glass>(model);
                glass.FileName = fileName;
                glass.DateCreated = DateTime.UtcNow;
                glass.UserId = _webContext.CurrentUserId;
                glass.CategoryId = int.Parse(_webContext.GetCookieValue(CookieKeys.GlassCatId));

                var image = Image.FromFile(Helper.GetFullFilePath(fileName, "png", true, null));
                var filePath = Helper.ConstructFilePath(fileName, Helper.EyeWearFilePath, false);
                string directory = Path.GetDirectoryName(filePath + ".png");

                Helper.CreateDirectoryIfNotExist(directory);

                var thumb = image.GetThumbnail(95, 43, ThumbnailMethod.Pad, Color.White);
                var vmImg = new KalikoImage(image);

                vmImg.SavePng(filePath + "_vm.png", 95);
                image.SaveJpg(filePath + ".jpg", 95);
                thumb.SaveJpg(filePath + "_t.jpg", 90);

                _glassService.Save(glass, null);

                _webContext.RemoveCookie(CookieKeys.GlassTempFileName);
                _webContext.RemoveCookie(CookieKeys.GlassCatId);

                var glasses = _glassService.GetGlasses(glass.CategoryId, null, null, 20);
                var imageSrc = Helper.RelativeFromAbsolutePath(filePath + "_vm.png", Request);

                var partial = (glass.CategoryId == 1) ? "PartialSunglasses" : "PartialEyeglasses";

                return Json(new {Success = true, imageSrc, html = this.Partial(partial, glasses)});
            }

            return Json(new {Success = false, Message = "Error saving hairstyle"});
        }

        public ActionResult EyeGlasses(int? p)
        {
            var model = _glassService.GetGlasses(2, null, p, 20);

            return Json(new {Success = true, html = this.Partial("PartialEyeglasses", model)},
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult SunGlasses(int? p)
        {
            var model = _glassService.GetGlasses(1, null, p, 20);

            return Json(new {Success = true, html = this.Partial("PartialSunglasses", model)}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("update-glass")]
        public ActionResult UpdateGlass(GlassModel model)
        {
            if (ModelState.IsValid)
            {
                var glass = Mapper.Map<GlassModel, Glass>(model);
                glass.GlassId = _glassService.GetGlassId(model.FileName);

                _glassService.Save(glass, new List<Expression<Func<Glass, object>>>{x=>x.HeightPercentage,
                x=>x.LeftPercentage, x=>x.TopPercentage, x=>x.WidthPercentage});

                return Json(new { Success = true });
            }

            return Json(new { Success = false, Message = "Error saving hairstyle" });
        }

        [HttpPost, ActionName("new-contact-lens")]
        public ActionResult NewContactLens(NewContactLensModel model, int cat)
        {
            if (ModelState.IsValid)
            {
                string fileName = "eyewear_" + Guid.NewGuid().ToString().ToLower().Replace("-", "_");
                //var image = Image.FromStream(model.ImageFile.InputStream);

                var image = Image.FromStream(model.ImageFile.InputStream);
                
                var filePath = Helper.ConstructFilePath(fileName, Helper.EyeWearFilePath, false);
                string directory = Path.GetDirectoryName(filePath + ".png");

                Helper.CreateDirectoryIfNotExist(directory);

                var thumb = image.GetThumbnail(50, 50, ThumbnailMethod.Pad, Color.White);
                var vmImg = new KalikoImage(image);

                vmImg.SavePng(filePath + "_vm.png", 100);
                image.SaveJpg(filePath + ".jpg", 90);
                thumb.SaveJpg(filePath + "_t.jpg", 90);

                var contact = new ContactLens
                    {FileName = fileName, DateCreated = DateTime.UtcNow, UserId = _webContext.CurrentUserId};

                _contactLensService.Save(contact, null);

                _webContext.RemoveCookie(CookieKeys.TempFileName);

                var contacts = _contactLensService.GetContactLensFileNames(null, null, 20);

                var imageSrc = Helper.RelativeFromAbsolutePath(filePath + "_vm.png", Request);

                return Json(new {Success = true, html = this.Partial("PartialContactLenses", contacts), imageSrc});
            }

            return Json(new {Success = false, Message = "Error saving contact lens"});
        }

        [ActionName("contact-lenses")]
        public ActionResult ContactLenses(int? p)
        {
            var model = _contactLensService.GetContactLensFileNames(null, p, 20);

            return Json(new { Success = true, html = this.Partial("PartialContactLenses", model)}, JsonRequestBehavior.AllowGet);
        }
    }
}