using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Kaliko.ImageLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SymaCord.TryOnMirror.CV;
using SymaCord.TryOnMirror.Core.Imaging;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.Areas.VirtualMakeover.Models;
using SymaCord.TryOnMirror.UI.Web.ContractResolver;
using SymaCord.TryOnMirror.UI.Web.Utils;
using SymaCord.TryOnMirror.UI.Web.Utils.Impl;

namespace SymaCord.TryOnMirror.UI.Web.Areas.VirtualMakeover.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TraceController : Controller
    {
        //private ICvDetection _cvDetection;
        private ICvStasmDetection _cvStasmDetection;
        private ITracedPhotoService _tracedPhotoService;
        private IWebContext _webContext;
        private ICache _cache;

        public TraceController(ICvStasmDetection cvStasmDetection, IWebContext webContext, 
            ITracedPhotoService tracedPhotoService, ICache cache)
        {
            //_cvDetection = cvDetection;
            _cvStasmDetection = cvStasmDetection;
            _tracedPhotoService = tracedPhotoService;
            _webContext = webContext;
            _cache = cache;
        }

        [HttpPost, ActionName("new-trace"), AllowAnonymous]
        public ActionResult NewTrace(NewTraceModel model)
        {
            if (ModelState.IsValid)
            {
                _webContext.RemoveCookie(CookieKeys.TraceTempFileName);
                string fileName = "photo_" + Guid.NewGuid().ToString().Replace("-", "_").ToLower();
                var image = Image.FromStream(model.ImageFile.InputStream);

                //Make sure the image height or width is not above the required sizes
                if (image.Width > 1600 || image.Height > 2005)
                {
                    image = image.Resize(1600, 2005);
                }
                //Check if the width can be divisible by 4, if not, make it to be divisible by 4
                else if (image.Width % 4 != 0)
                {
                    image = image.ResizeByWidth((image.Width / 4) * 4);
                }

                //string parentDir = fileName.Substring(0, 2) + "/";
                //string saveToPath = Helper.TempFilePath;// +parentDir;
                var filePath = Helper.ConstructFilePath(fileName, Helper.TempFilePath, true);// saveToPath + "/" + fileName;
                var directory = Path.GetDirectoryName(filePath);

                Helper.CreateDirectoryIfNotExist(directory);

                image.SaveJpg(filePath + ".jpg", 95);

                var detectedPoints = _cvStasmDetection.SearchFacialFeatures(filePath + ".jpg");
                var data = detectedPoints.Count > 0 ? CamelCaseJsonSerializer.SerializeObject(detectedPoints) : null;

                _webContext.SetCookieValue(CookieKeys.TraceTempFileName, fileName);
                var imgUrl = Helper.RelativeFromAbsolutePath(filePath + ".jpg", Request);
                //Url.RouteUrl(new {controller = "home", action = "photo-file", area = "", fn = fileName, s = "o"});

                return Json(new { Success = true, Data = data, Html = this.Partial("PartialTraceWizard", null), imgUrl },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = false });
        }

        [HttpPost, ActionName("save-new-trace"), AllowAnonymous]
        public ActionResult SaveNewTrace(string data, bool openLips)
        {
            //System.Threading.Thread.Sleep(5000);

            string fileName = _webContext.GetCookieValue(CookieKeys.TraceTempFileName);
            var tracedPhoto = ParseTraceObject(JObject.Parse(data), openLips);

            tracedPhoto.FileName = fileName;
            tracedPhoto.PhotoTitle = fileName;
            tracedPhoto.IsModel = false;
            tracedPhoto.DateCreated = DateTime.UtcNow;
            tracedPhoto.OpenLips = openLips;

            if (!_webContext.IsAuthenticated)
                tracedPhoto.AnonymouseId = new Guid(_webContext.AnonymouseId);
            else
            {
                tracedPhoto.UserId = _webContext.CurrentUserId;
            }

            var image = Image.FromFile(Helper.GetFullFilePath(fileName, "jpg", true, null));
            var filePath = Helper.ConstructFilePath(fileName, Helper.PhotoFilePath, false);
            string directory = Path.GetDirectoryName(filePath + ".jpg");

            Helper.CreateDirectoryIfNotExist(directory);

            var thumb = image.GetThumbnail(95, 120, ThumbnailMethod.Pad, Color.White);

            image.SaveJpg(filePath + ".jpg", 100);
            thumb.SaveJpg(filePath + "_t.jpg", 90);

            _tracedPhotoService.Save(tracedPhoto, null);
            _cache.DeleteItems("tracedphotos___true_");

            _webContext.RemoveCookie(CookieKeys.TraceTempFileName);
            _webContext.RemoveCookie(CookieKeys.LatestTrace);
            _webContext.SetCookieValue(CookieKeys.LatestTrace, fileName, DateTime.UtcNow.AddDays(30));

            return Json(new
            {
                Success = true,
                Data = this.Partial("PartialPhoto", fileName),
                Message = "Photo traced successfully"
            });
        }

        [HttpPost, ActionName("trace-new-model")]
        public ActionResult TraceNewModel(NewTraceModel model)
        {
            if (ModelState.IsValid)
            {
                _webContext.RemoveCookie(CookieKeys.NewModelTrace);
                string fileName = "model_" + Guid.NewGuid().ToString().Replace("-", "_").ToLower();
                var image = Image.FromStream(model.ImageFile.InputStream);

                //Make sure the image height or width is not above the required sizes
                if (image.Width > 1600 || image.Height > 2005)
                {
                    image = image.Resize(1600, 2005);
                }
                //Check if the width can be divisible by 4, if not, make it to be divisible by 4
                else if (image.Width % 4 != 0)
                {
                    image = image.ResizeByWidth((image.Width / 4) * 4);
                }

                //string parentDir = fileName.Substring(0, 2) + "/";
                //string saveToPath = Helper.TempFilePath;// +parentDir;
                var filePath = Helper.ConstructFilePath(fileName, Helper.TempFilePath, true);// saveToPath + "/" + fileName;
                var directory = Path.GetDirectoryName(filePath);

                Helper.CreateDirectoryIfNotExist(directory);

                image.SaveJpg(filePath + ".jpg", 95);

                var detectedPoints = _cvStasmDetection.SearchFacialFeatures(filePath + ".jpg");
                var data = detectedPoints.Count > 0 ? CamelCaseJsonSerializer.SerializeObject(detectedPoints) : null;

                _webContext.SetCookieValue(CookieKeys.NewModelTrace, fileName);
                var imgUrl = Helper.RelativeFromAbsolutePath(filePath + ".jpg", Request);
                    //Url.RouteUrl(new {controller = "home", action = "photo-file", area = "", fn = fileName, s = "o"});

                return Json( new { Success = true, Data = data, Html = this.Partial("PartialTraceWizard", null), imgUrl },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = false });
        }

        [HttpPost, ActionName("save-new-model-trace")]
        public ActionResult SaveNewModelTrace(string data, bool openLips)
        {
            //var result = JsonConvert.DeserializeObject<Dictionary<string, CvResult>>(data);
            //var stringData = JObject.Parse(JsonConvert.SerializeObject(result));

            string fileName = _webContext.GetCookieValue(CookieKeys.NewModelTrace);
            var tracedPhoto = ParseTraceObject(JObject.Parse(data), openLips);

            tracedPhoto.FileName = fileName;
            tracedPhoto.PhotoTitle = fileName;
            tracedPhoto.IsModel = true;
            tracedPhoto.DateCreated = DateTime.UtcNow;
            tracedPhoto.OpenLips = openLips;
            tracedPhoto.UserId = _webContext.CurrentUserId;

            var image = Image.FromFile(Helper.GetFullFilePath(fileName, "jpg", true, null));
            var filePath = Helper.ConstructFilePath(fileName, Helper.ModelPhotoeFilePath, false);
            string directory = Path.GetDirectoryName(filePath + ".jpg");

            Helper.CreateDirectoryIfNotExist(directory);

            var thumb = image.GetThumbnail(95, 120, ThumbnailMethod.Pad, Color.White);

            image.SaveJpg(filePath + ".jpg", 100);
            thumb.SaveJpg(filePath + "_t.jpg", 90);

            _tracedPhotoService.Save(tracedPhoto, null);
            _cache.DeleteItems("tracedphotos___true_");

            _webContext.RemoveCookie(CookieKeys.NewModelTrace);

            return Json(new
                {
                    Success = true,
                    Data = this.Partial("PartialModelPhoto", fileName),
                    Message = "Trace was successfully saved"
                });
        }

        [HttpPost, AllowAnonymous]
        public ActionResult Update(string data, bool? openLips)
        {
            try
            {
                var coords = JObject.Parse(data);
                var fileName = (coords["fileName"]).ToString();
                long traceId = 0;

                if (_tracedPhotoService.IsModel(fileName, out traceId) && !User.IsInRole("Admin"))
                {
                    return Json(new {Success = false});
                }

                var trace = new TracedPhoto {TraceId = traceId, FileName = fileName};
                var props = new List<Expression<Func<TracedPhoto, object>>>();

                if (coords["face"] != null)
                {
                    trace.FaceCoordinates =
                        (coords["face"]["coords"]).ToString().Replace(Environment.NewLine, "").Trim();
                    props.Add(x => x.FaceCoordinates);
                }

                if (coords["leftEye"] != null)
                {
                    trace.LeftEyeCoordinates =
                        (coords["leftEye"]["coords"]).ToString().Replace(Environment.NewLine, "").Trim();
                    props.Add(x => x.LeftEyeCoordinates);
                }

                if (coords["rightEye"] != null)
                {
                    trace.RightEyeCoordinates =
                        (coords["rightEye"]["coords"]).ToString().Replace(Environment.NewLine, "").Trim();
                    props.Add(x => x.RightEyeCoordinates);
                }

                if (coords["leftEyeball"] != null)
                {
                    trace.LeftEyeBallPupilCoord =
                        (coords["leftEyeball"]["pupilCoord"]).ToString().Replace(Environment.NewLine, "");
                    trace.LeftEyeballRadius = (int) coords["leftEyeball"]["radius"];
                    props.Add(x => x.LeftEyeBallPupilCoord);
                    props.Add(x => x.LeftEyeballRadius);
                }

                if (coords["rightEyeball"] != null)
                {
                    trace.RightEyeBallPupilCoord =
                        (coords["rightEyeball"]["pupilCoord"]).ToString().Replace(Environment.NewLine, "");
                    trace.RightEyeballRadius = (int) coords["rightEyeball"]["radius"];
                    props.Add(x => x.RightEyeBallPupilCoord);
                    props.Add(x => x.RightEyeballRadius);
                }

                if (coords["glass"] != null)
                {
                    trace.GlassCoordinates =
                        (coords["glass"]["coords"]).ToString().Replace(Environment.NewLine, "").Trim();
                    props.Add(x => x.GlassCoordinates);
                }

                _tracedPhotoService.Save(trace, props);
                _cache.DeleteItems("tracedphoto_" + fileName + "_");

                return Json(new {Success = true});
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
        }

        [ActionName("get-photo-coords"), AllowAnonymous]
        public ActionResult GetPhotoCoords(string fn /*file name*/)
        {
            var tracedPhoto = _tracedPhotoService.GetTracedPhoto(fn);

            if (tracedPhoto != null)
            {
                var path = fn.StartsWith("photo") ? Helper.PhotoFilePath : Helper.ModelPhotoeFilePath;
                var filePath = Helper.ConstructFilePath(fn, path, false);
                var imageSrc = Helper.RelativeFromAbsolutePath(filePath + ".jpg", Request);
                    //Url.RouteUrl(new {action="photo-file", controller = "home", area = "", s = "o", 
                      //  fn = tracedPhoto.FileName});

                return Json(new {Success = true, Data = ConvertTracedPhotoToJson(tracedPhoto), imageSrc,
                    fileName = fn, tracedPhoto.OpenLips});
            }

            return Json(new {Success = false, Message = "No data found"});
        }

        [ActionName("get-default-model-photo"), AllowAnonymous]
        public ActionResult GetDefaultModelPhoto()
        {
            var tracedPhoto = _tracedPhotoService.GetTracedPhotos(null, null, true, null, 1).FirstOrDefault();

            if (tracedPhoto != null)
            {
                var fn = tracedPhoto.FileName;
                var path = Helper.ModelPhotoeFilePath;
                var filePath = Helper.ConstructFilePath(fn, path, false);
                var imageSrc = Helper.RelativeFromAbsolutePath(filePath + ".jpg", Request);

                return Json(new
                {
                    Success = true,
                    Data = ConvertTracedPhotoToJson(tracedPhoto),
                    imageSrc,
                    fileName = fn,
                    tracedPhoto.OpenLips
                });
            }

            return Json(new { Success = false, Message = "No data found" });
        }

        [NonAction]
        private TracedPhoto ParseTraceObject(JObject stringData, bool openLips)
        {
            //var stringData = JObject.Parse(JsonConvert.SerializeObject(data));

            var tracedPhoto = new TracedPhoto();
            tracedPhoto.FaceCoordinates = (stringData["face"]["coords"]).ToString().Replace(Environment.NewLine, "").Trim();
            tracedPhoto.LeftEyeCoordinates = (stringData["leftEye"]["coords"]).ToString().Replace(Environment.NewLine, "");
            tracedPhoto.LeftEyeBallPupilCoord = (stringData["leftEyeball"]["pupilCoord"]).ToString().Replace(Environment.NewLine, "");
            tracedPhoto.LeftEyeballRadius = (int) stringData["leftEyeball"]["radius"];
            tracedPhoto.LeftEyeBrowCoordinates = (stringData["leftEyeBrow"]["coords"]).ToString().Replace(Environment.NewLine, "");
            tracedPhoto.RightEyeCoordinates = (stringData["rightEye"]["coords"]).ToString().Replace(Environment.NewLine, "");
            tracedPhoto.RightEyeBallPupilCoord = (stringData["rightEyeball"]["pupilCoord"]).ToString().Replace(Environment.NewLine, "");
            tracedPhoto.RightEyeballRadius = (int) stringData["rightEyeball"]["radius"];
            tracedPhoto.RightEyeBrowCoordinates = (stringData["rightEyeBrow"]["coords"]).ToString().Replace(Environment.NewLine, "");
            tracedPhoto.NoseCoordinates = (stringData["nose"]["coords"]).ToString().Replace(Environment.NewLine, "");
            tracedPhoto.LipsCoordinates = (stringData["lips"]["coords"]).ToString().Replace(Environment.NewLine, "");

            if (openLips)
            {
                tracedPhoto.OpenLipsCoodinate = (stringData["openLips"]["coords"]).ToString().Replace(Environment.NewLine, "");
            }

            return tracedPhoto;
        }

        [NonAction]
        private string ConvertTracedPhotoToJson(TracedPhoto trace)
        {
            var result = new Dictionary<string, object>();
            result.Add(SearchFeatureId.Face, new CvResult{Coords = JsonConvert.DeserializeObject<Coord[]>(trace.FaceCoordinates)});
            result.Add(SearchFeatureId.LeftEye, new CvResult{Coords = JsonConvert.DeserializeObject<Coord[]>(trace.LeftEyeCoordinates)});
            result.Add(SearchFeatureId.LeftEyeball, new CvEyeballResult{Radius=trace.LeftEyeballRadius.Value,
                PupilCoord=JsonConvert.DeserializeObject<Coord>(trace.LeftEyeBallPupilCoord)});
            result.Add(SearchFeatureId.LeftEyeBrow, new CvResult{Coords =JsonConvert.DeserializeObject<Coord[]>(trace.LeftEyeBrowCoordinates)});
            result.Add(SearchFeatureId.RightEye, new CvResult{Coords=JsonConvert.DeserializeObject<Coord[]>(trace.RightEyeCoordinates)});
            result.Add(SearchFeatureId.RightEyeball, new CvEyeballResult{Radius=trace.RightEyeballRadius.Value,
                PupilCoord=JsonConvert.DeserializeObject<Coord>(trace.RightEyeBallPupilCoord)});
            result.Add(SearchFeatureId.RightEyeBrow, new CvResult{Coords=JsonConvert.DeserializeObject<Coord[]>(trace.RightEyeBrowCoordinates)});
            result.Add(SearchFeatureId.Nose, new CvResult{Coords=JsonConvert.DeserializeObject<Coord[]>(trace.NoseCoordinates)});
            result.Add(SearchFeatureId.Lips, new CvResult{Coords=JsonConvert.DeserializeObject<Coord[]>(trace.LipsCoordinates)});
            result.Add(SearchFeatureId.OpenLips, new CvResult());
            result.Add("HasOpenLips", trace.OpenLips);

            if (trace.OpenLips)
            {
                result[SearchFeatureId.OpenLips] = new CvResult
                    {
                        Coords = JsonConvert.DeserializeObject<Coord[]>(trace.OpenLipsCoodinate)
                    };
            }

            if (!string.IsNullOrEmpty(trace.GlassCoordinates))
            {
                result.Add(SearchFeatureId.Glass, new CvResult { Coords = JsonConvert.DeserializeObject<Coord[]>(trace.GlassCoordinates) });
            }

            var data = CamelCaseJsonSerializer.SerializeObject(result);
            
            //data["face"]["coords"] = trace.FaceCoordinates;
            //data["leftEye"]["coords"] =  trace.LeftEyeCoordinates;
            //data["leftEyeball"]["radius"] = trace.LeftEyeballRadius;
            //data["leftEyeball"]["pupilCoord"] = trace.LeftEyeBallPupilCoord;
            //data["leftEyeBrow"]["coords"] = trace.LeftEyeBrowCoordinates;

            //data["rightEye"]["coords"] = trace.RightEyeCoordinates;
            //data["rightEyeball"]["radius"] = trace.RightEyeballRadius;
            //data["rightEyeball"]["pupilCoord"] = trace.RightEyeBallPupilCoord;
            //data["rightEyeBrow"]["coords"] = trace.RightEyeBrowCoordinates;
            //data["nose"]["coords"] = trace.NoseCoordinates;
            //data["lips"]["coords"] = trace.LipsCoordinates;
            //data["openLips"]["coords"] = trace.OpenLipsCoodinate;

            return data;
        }
    }
}
