using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kaliko.ImageLibrary;
using SymaCord.VirtualMakeover.CV;
using SymaCord.VirtualMakeover.Core.Imaging;
using SymaCord.VirtualMakeover.DataService.Services;
using SymaCord.VirtualMakeover.UI.Web.Makeover.Areas.VirtualMakeover.Models;
using SymaCord.VirtualMakeover.UI.Web.Makeover.ContractResolver;
using SymaCord.VirtualMakeover.UI.Web.Makeover.Utils;
using SymaCord.VirtualMakeover.UI.Web.Makeover.Utils.Impl;

namespace SymaCord.VirtualMakeover.UI.Web.Makeover.Areas.Calibration.Controllers
{
    public class PhotoController : Controller
    {
        private ITracedPhotoService _tracedPhotoService;
        private ICvStasmDetection _cvStasmDetection;
        private IWebContext _webContext;

        public PhotoController(ITracedPhotoService tracedPhotoService, ICvStasmDetection cvStasmDetection, 
            IWebContext webContext)
        {
            _tracedPhotoService = tracedPhotoService;
            _cvStasmDetection = cvStasmDetection;
            _webContext = webContext;
        }

        public ActionResult Index()
        {
            return View();
        }

        [ActionName("section")]
        public ActionResult Section()
        {
            var model = _tracedPhotoService.GetTracedPhotos(null, null, true, null, int.MaxValue);
            
            if(Request.IsAjaxRequest())
            {
                return Json(new {Success = true, Data = this.Partial("PartialPhotoSection", model)});
            }

            return PartialView("PartialPhotoSection", model);
        }
        
    }
}
