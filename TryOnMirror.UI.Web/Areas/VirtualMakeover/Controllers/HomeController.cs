using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using Kaliko.ImageLibrary;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.Utils;
using SymaCord.TryOnMirror.UI.Web.Utils.Impl;
using SymaCord.TryOnMirror.UI.Web.ViewModels;

namespace SymaCord.TryOnMirror.UI.Web.Areas.VirtualMakeover.Controllers
{
    public class HomeController : Controller
    {
        private ITracedPhotoService _tracedPhotoService;
        private IWebContext _webContext;
        private ISharedMakeoverService _sharedMakeoverService;

        public HomeController(ITracedPhotoService tracedPhotoService, IWebContext webContext, 
            ISharedMakeoverService sharedMakeoverService)
        {
            _tracedPhotoService = tracedPhotoService;
            _webContext = webContext;
            _sharedMakeoverService = sharedMakeoverService;
        }

        public ActionResult Index()
        {
            var tracedModelsPhotos = _tracedPhotoService.GetTracedPhotosFileName(null, null, true, null, int.MaxValue);
            ViewBag.UserPhotoFileName = _webContext.GetCookieValue(CookieKeys.LatestTrace) ?? string.Empty;

            return View(tracedModelsPhotos);
        }

        [HttpPost]
        [ActionName("finalize-makeover")]
        public ActionResult FinalizeMakeover(CompleteMakeoverModel model)
        {
            if(ModelState.IsValid)
            {
                var isModelPhoto = model.PhotoFileName.StartsWith("model_");
                var filePath = Helper.ConstructFilePath(model.PhotoFileName + ".jpg", 
                    isModelPhoto ? Helper.ModelPhotoeFilePath
                    : Helper.PhotoFilePath, false);

                var beforeImage = new KalikoImage(filePath);
                var headerImg = new Bitmap(Server.MapPath("~/assets/images/makeover-share-header.png"));
                KalikoImage makeoverImg = null;

                beforeImage.BackgroundColor = Color.White;
                beforeImage = beforeImage.GetThumbnailImage(150, 150, ThumbnailMethod.Fit);

                using (var fileStream = new MemoryStream(model.ImageData))
                {
                    makeoverImg = new KalikoImage(fileStream);
                }
                var size = new Size(headerImg.Width + 40, headerImg.Height + makeoverImg.Height + 80);

                if(makeoverImg.Width > 450 || makeoverImg.Height > 600)
                {
                    makeoverImg.BackgroundColor = Color.White;
                    makeoverImg = makeoverImg.GetThumbnailImage(450, 600);
                }

                var completeImg = new Bitmap(size.Width, size.Height);
                using (var g = Graphics.FromImage(completeImg))
                {
                    //Change the background to black
                    g.FillRectangle(Brushes.White, 0, 0, size.Width, size.Height);
                    g.DrawImage(headerImg, 15, 15, 611, 130);
                    g.DrawImage(beforeImage.Image, 15, headerImg.Height + 20);

                    g.DrawString("Before", new Font("Ariel", 16, FontStyle.Regular), Brushes.Black, 15, 
                        (headerImg.Height + beforeImage.Height) + 25);

                    g.DrawImage(makeoverImg.Image, (headerImg.Width + 15) - makeoverImg.Width, headerImg.Height + 20);

                    g.DrawString("New TryOn Mirror Look", new Font("Ariel", 14, FontStyle.Regular), Brushes.Black,
                        (headerImg.Width + 15) - makeoverImg.Width,
                        (headerImg.Height + makeoverImg.Height) + 30);
                }

                string saveFileName = "makeover_" + Guid.NewGuid().ToString().ToLower().Replace("-", "_");
                var saveFilePath = Helper.ConstructFilePath(saveFileName, Helper.MakeoverImageFilePath, false);
                string directory = Path.GetDirectoryName(saveFilePath + ".jpg");

                Helper.CreateDirectoryIfNotExist(directory);

                completeImg.Save(saveFilePath + ".jpg", ImageFormat.Jpeg);

                var urlCode = Helper.RandomString(7);
                var shared = new SharedMakeover
                    {
                        DateCreated = DateTime.UtcNow,
                        FileName = saveFileName,
                        Provider = model.Provider,
                        UrlCode = urlCode
                    };
                _sharedMakeoverService.Save(shared, null);

                var url = Request.Headers["host"] + Url.RouteUrl(new {action = "s", code = urlCode});

                return Json(new {Success = true, url});
            }

            return Json(new {Success = false, Message = "Sent data is invalid"});
        }

        public ActionResult S(string code)
        {
            var shared = _sharedMakeoverService.GetSharedMakeover(code);
            ViewBag.FileName = shared.FileName;

            return View("Shared");
        }
    }
}
