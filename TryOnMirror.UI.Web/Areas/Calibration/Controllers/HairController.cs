using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using Kaliko.ImageLibrary;
using PagedList;
using SymaCord.TryOnMirror.Core.Imaging;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.Areas.Calibration.Models;
using SymaCord.TryOnMirror.UI.Web.Utils;
using SymaCord.TryOnMirror.UI.Web.Utils.Impl;
using SymaCord.TryOnMirror.UI.Web.ViewModels;

namespace SymaCord.TryOnMirror.UI.Web.Areas.Calibration.Controllers
{
    [Authorize(Roles="Admin")]
    public class HairController : Controller
    {
        private IHairstyleService _hairstyleService;
        private IBrandService _brandService;
        private ICache _cache;
        private IWebContext _webContext;

        public HairController(IHairstyleService hairstyleService, ICache cache, IWebContext webContext,
            IBrandService brandService)
        {
            _hairstyleService = hairstyleService;
            _cache = cache;
            _webContext = webContext;
            _brandService = brandService;
        }

        public ActionResult Index(string s, bool? pgr, int? p)
        {
            int count = 0;
            var hairstyles = _hairstyleService.GetHairstyleNames(s, p, 4, out count);
            p = p ?? 1;

            //ViewBag.TotalCount = count;

            var model = new StaticPagedList<string>(hairstyles, p.Value, 4, count);

            //ViewBag.OnePageOfUsers = modelAsIPagedList;

            if (pgr.HasValue && pgr.Value)
                return Json(new { Success = true, html = this.Partial("PartialHairstyles", model) },
                    JsonRequestBehavior.AllowGet);

            return Json(new { Success = true, html = this.Partial("PartialHair", model) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Hairstyles(int? p)
        {
            var model = _hairstyleService.GetHairstyleNames(null, p, 20);

            return Json(new { Success = true, html = this.Partial("PartialHairstylesContent", model) }, 
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("new-hairstyle")]
        public ActionResult NewHairstyles(NewHairstyleModel model)
        {
            if(ModelState.IsValid)
            {
                _webContext.RemoveCookie(CookieKeys.HairTempFileName);
                string fileName = "hairstyle_" + Guid.NewGuid().ToString().ToLower().Replace("-", "_");
                var image = Image.FromStream(model.HairstyleImage.InputStream);
                
                //string parentDir = fileName.Substring(0, 2) + "/";
                //string saveToPath = Helper.TempFilePath;// +parentDir;
                var filePath = Helper.ConstructFilePath(fileName, Helper.TempFilePath, true);// saveToPath + "/" + fileName;
                var directory = Path.GetDirectoryName(filePath);

                Helper.CreateDirectoryIfNotExist(directory);
                
                image.SavePng(filePath + ".png", 95);
                _webContext.SetCookieValue(CookieKeys.HairTempFileName, fileName);

                var imageSrc = Helper.RelativeFromAbsolutePath(filePath + ".png", Request);
                    //Url.RouteUrl(new {action = "image-file", controller="home", fn=fileName, ext="png", t=true, area = ""});

                return Json(new {Success = true, imageSrc, fileName});
            }

            return Json(new {Success = false});
        }

        [HttpPost, ActionName("save-new-hairstyle")]
        public ActionResult SaveNewHairstyle(HairstyleModel model)
        {
            if(ModelState.IsValid)
            {
                var fileName = Path.GetFileNameWithoutExtension(_webContext.GetCookieValue(CookieKeys.HairTempFileName));

                var hairstyle = Mapper.Map<HairstyleModel, Hairstyle>(model);
                hairstyle.FileName = fileName;
                hairstyle.DateCreated = DateTime.UtcNow;
                hairstyle.UserId = _webContext.CurrentUserId;

                var image = Image.FromFile(Helper.GetFullFilePath(fileName, "png", true, null));
                var filePath = Helper.ConstructFilePath(fileName, Helper.HairstyleFilePath, false);
                string directory = Path.GetDirectoryName(filePath + ".png");

                Helper.CreateDirectoryIfNotExist(directory);
                
                var vmImg = new KalikoImage(image);
                var thumb = image.GetThumbnail(95, 120, ThumbnailMethod.Pad, Color.White);
                
                vmImg.SavePng(filePath + "_vm.png", 100);
                image.SaveJpg(filePath + ".jpg", 90);
                thumb.SaveJpg(filePath + "_t.jpg", 90);

                _hairstyleService.Save(hairstyle, null);

                _webContext.RemoveCookie(CookieKeys.HairTempFileName);

                int count = 0;
                var hairstyles = _hairstyleService.GetHairstyleNames(null, null, 4, out count);
                var pagedHairstyles = new StaticPagedList<string>(hairstyles, 1, 4, count);
                
                var imageSrc = Helper.RelativeFromAbsolutePath(filePath + "_vm.png", Request);

                return Json(new { Success = true, imageSrc, html = this.Partial("PartialHairstyles", pagedHairstyles) });
            }

            return Json(new {Success = false, Message = "Error saving hairstyle"});
        }

        [HttpPost, ActionName("update-hairstyle-trace")]
        public ActionResult UpdateHairstyleTrace(HairstyleModel model)
        {
            if (ModelState.IsValid)
            {
                var hairstyle = Mapper.Map<HairstyleModel, Hairstyle>(model);
                hairstyle.HairstyleId = _hairstyleService.GetHairstyleId(model.FileName);

                _hairstyleService.Save(hairstyle, new List<Expression<Func<Hairstyle, object>>>{x=>x.HeightPercentage,
                x=>x.LeftDistance, x=>x.TopDistance, x=>x.WidthPercentage});

                return Json(new { Success = true});
            }

            return Json(new { Success = false, Message = "Error saving hairstyle" });
        }

        [HttpPost, ActionName("new-thumb")]
        public ActionResult NewThumb(NewThumbImage model)
        {
            if (ModelState.IsValid)
            {
                string fileName = model.Id;
                var image = Image.FromStream(model.ImageFile.InputStream);

                var filePath = Helper.ConstructFilePath(fileName, Helper.TempFilePath, true);
                var directory = Path.GetDirectoryName(filePath);

                Helper.CreateDirectoryIfNotExist(directory);
                var resized = image.Resize(600, 600); //image.GetThumbnail(800, 800, ThumbnailMethod.Pad, Color.White);

                image.SaveJpg(filePath + ".jpg", 95);
                resized.SaveJpg(filePath+"_600.jpg", 90);

                var imageSrc = Helper.RelativeFromAbsolutePath(filePath + "_600.jpg", Request) + 
                "?_="+DateTime.Now.Ticks;

                return Json(new { Success = true, imageSrc, fileName });
            }

            return Json(new { Success = false });
        }

        public ActionResult Edit(string fn)
        {
            var hairstyle = _hairstyleService.GetHairstyle(fn);
            ViewBag.Brands = new SelectList(_brandService.GetBrands((int)BrandCategories.Hairs),"BrandId", "BrandName");

            var model = new EditHairstyleModel {Id = fn, Title = hairstyle.Title, BrandId = hairstyle.BrandId};

            return View(model);
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult Edit(EditHairstyleModel model)
        {
            if(ModelState.IsValid)
            {
                var id = _hairstyleService.GetHairstyleId(model.Id);
                var hairstyle = new Hairstyle {HairstyleId = id, Title = model.Title, BrandId = model.BrandId};

                if (model.W > 0 && model.H > 0)
                {
                    string filePath = Helper.ConstructFilePath(model.Id, Helper.HairstyleFilePath, true);
                    var savePath = Helper.ConstructFilePath(model.Id, Helper.HairstyleFilePath, false);

                    var originalImg = Image.FromFile(filePath + ".jpg");
                    var image = Image.FromFile(filePath + "_600.jpg");

                    var bmp = new Bitmap(model.W, model.H, image.PixelFormat);
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(image, new Rectangle(0, 0, model.W, model.H), 
                            new Rectangle(model.X, model.Y, model.W, model.H), GraphicsUnit.Pixel);
                    }

                    originalImg.SaveJpg(savePath + ".jpg", 90);
                    var thumb = new KalikoImage(bmp) {BackgroundColor = Color.White};
                    thumb = thumb.GetThumbnailImage(95, 120, ThumbnailMethod.Fit);

                    thumb.SaveJpg(savePath + "_t.jpg", 90);
                }

                _hairstyleService.Save(hairstyle, new List<Expression<Func<Hairstyle, object>>> { x => x.Title, x=>x.BrandId});

                return Json(new { Success = true, Message = "Hairstyle updated successfully" });
            }

            return Json(new {Success = false, Message = "Hairstyle was not updated. Please make sure " +
                                                        "all info were correctly entered"});
        }

        [HttpPost]
        public ActionResult Delete(string fn)
        {
            //string pathWithFileName = Helper.ConstructFilePath(fn, Helper.HairstyleFilePath, false);
            //var path = pathWithFileName.Substring(0, pathWithFileName.LastIndexOf("\\") + 1);
            var id = _hairstyleService.GetHairstyleId(fn);

            _hairstyleService.Delete(id);
            Helper.DeleteFile(fn, ".jpg", Helper.HairstyleFilePath);

            //var exist = System.IO.File.Exists(pathWithFileName + ".png");

            //if (System.IO.File.Exists(photoPath + fn + ".png"))
            //{
            //    new List<string>(Directory.GetFiles(photoPath)).ForEach(file =>
            //    {
            //        var re = new Regex(fn, RegexOptions.IgnoreCase);
            //        if (re.IsMatch(file))
            //            System.IO.File.Delete(file);
            //    });

            //    if (Directory.GetFiles(photoPath).Length == 0)
            //        Directory.Delete(photoPath);
            //}

            return Json(new { Success = true, Message = "hairstyle deleted successfull" });
        }
    }
}
