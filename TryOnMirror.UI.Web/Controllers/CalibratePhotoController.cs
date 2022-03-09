using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SymaCord.TryOnMirror.CV;
using SymaCord.TryOnMirror.Core.Imaging;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.ContractResolver;

namespace SymaCord.TryOnMirror.UI.Web.Controllers
{
    public class CalibratePhotoController : Controller
    {
        private ICvDetection _cvDetection;
        private ICvStasmDetection _cvStasmDetection;
        private ITracedPhotoService _tracedPhotoService;
        //private Dictionary<string, Rectangle> rects = new Dictionary<string, Rectangle>();

        public CalibratePhotoController(ICvDetection cvDetection, ICvStasmDetection cvStasmDetection, 
            ITracedPhotoService tracedPhotoService)
        {
            _cvDetection = cvDetection;
            _cvStasmDetection = cvStasmDetection;
            _tracedPhotoService = tracedPhotoService;
        }

        public ActionResult Index()
        {
            string filePath = Server.MapPath("~/assets/images/model-5.jpg");
            string savePath = Server.MapPath("~/assets/images/model-5_2.jpg");

            var image = new Bitmap(filePath);
            
            if (image.Width % 4 != 0)
            {
                image = (Bitmap)image.ResizeByWidth((image.Width / 4) * 4);
                image.SaveJpg(savePath, 99);
            }

            return View();
        }

        //[ActionName("trace-face")]
        //public ActionResult TraceFace(long id)
        //{
        //    var image = new Bitmap(Server.MapPath("~/assets/images/model-6.jpg"));
        //    var faceRect = _cvDetection.DetectFace(image);

        //    var leftEyeRoi = new Rectangle(faceRect.X, faceRect.Y, faceRect.Width/2, faceRect.Height/2);
        //    var rightEyeRoi = new Rectangle(leftEyeRoi.Right, faceRect.Y, faceRect.Width / 2, faceRect.Height / 2);
            
        //    //get the y position where the mouth roi should start
        //    var mouthY = faceRect.Y + (faceRect.Height/2);

        //    var mouthRoi = new Rectangle(faceRect.X, mouthY, faceRect.Width, faceRect.Height / 2);

        //    var leftEyeRect = _cvDetection.DetectLeftEye(image, leftEyeRoi);
        //    var rightEyeRect = _cvDetection.DetectRightEye(image, rightEyeRoi);
        //    var mouthRect = _cvDetection.DetectMouth(image, mouthRoi);

        //    var rects = new List<DetectionFeature>
        //        {
        //            new DetectionFeature {Id = DetectionId.Face, Rect = _cvDetection.DetectFace(image)},
        //            new DetectionFeature {Id = DetectionId.LeftEye, Rect = leftEyeRect},
        //            new DetectionFeature {Id = DetectionId.RightEye, Rect = rightEyeRect},
        //            new DetectionFeature {Id = DetectionId.Mouth, Rect = mouthRect}
        //        };

        //    //Perform facial feature detection for the provided photo and add detected areas to the RectArea list

        //    return Json(new {Success = true, Data = new {rects}}, JsonRequestBehavior.AllowGet);
        //}

        [ActionName("trace-facial-features"), AllowAnonymous]
        public ActionResult TraceFacialFeatures(Guid id)
        {
            string filePath = Server.MapPath("~/assets/images/model-2.jpg");

            var detectedPoints = _cvStasmDetection.SearchFacialFeatures(filePath);
            var data = CamelCaseJsonSerializer.SerializeObject(detectedPoints);

            return Json(new {Success = true, Data = data}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("save-trace")]
        public ActionResult SaveTrace(Dictionary<string, CvResult> data)
        {
            var stringData = JObject.Parse(JsonConvert.SerializeObject(data));

            var tracedPhoto = new TracedPhoto();

            tracedPhoto.FaceCoordinates = (string) stringData["Face"]["Coords"];
            tracedPhoto.LeftEyeCoordinates = (string)stringData["LeftEye"]["Coords"];
            tracedPhoto.LeftEyeBallPupilCoord = (string)stringData["LeftEyeball"]["PupilCoord"];
            tracedPhoto.LeftEyeballRadius = (int) stringData["LeftEyeball"]["Radius"];

            tracedPhoto.RightEyeCoordinates = (string)stringData["RightEye"]["Coords"];
            tracedPhoto.RightEyeBallPupilCoord = (string)stringData["RightEyeball"]["PupilCoord"];
            tracedPhoto.RightEyeballRadius = (int)stringData["RightEyeball"]["Radius"];
            tracedPhoto.NoseCoordinates = (string) stringData["Nose"]["Coords"];
            tracedPhoto.LipsCoordinates = (string)stringData["Lips"]["Coords"];
            tracedPhoto.OpenLipsCoodinate = (string)stringData["OpenLips"]["Coords"];

            tracedPhoto.FileName = Guid.NewGuid().ToString("N").ToLower();
            tracedPhoto.DateCreated = DateTime.UtcNow;

            _tracedPhotoService.Save(tracedPhoto, null);

            return Json(new {Success = true, Data = data});
        }
    }
}
