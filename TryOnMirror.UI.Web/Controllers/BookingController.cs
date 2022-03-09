using System;
using System.IO;
using System.Web.Mvc;
using AutoMapper;
using Kaliko.ImageLibrary;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.Utils;
using SymaCord.TryOnMirror.UI.Web.ViewModels;

namespace SymaCord.TryOnMirror.UI.Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private IHairstyleBookingService _bookingService;
        private ICache _cache;
        private IWebContext _webContext;
        private ISalonService _salonService;
        private ICommentService _commentService;
        private IUserService _userService;

        public BookingController(IHairstyleBookingService bookingService, ICache cache, IWebContext webContext,
            ISalonService salonService, ICommentService commentService, IUserService userService)
        {
            _bookingService = bookingService;
            _cache = cache;
            _webContext = webContext;
            _salonService = salonService;
            _commentService = commentService;
            _userService = userService;
        }

        public ActionResult Index(int? p)
        {
            var model = _bookingService.GetHairstyleCustomBookings(_webContext.CurrentUserId, null, p, 25);
            ViewBag.Statuses = _bookingService.GetBookingStatuses();

            return View(model);
        }

        public ActionResult Detail(Guid id)
        {
            var model = _bookingService.GetHairstyleBooking(id);
            ViewBag.CurrentUserId = _webContext.CurrentUserId;

            return View(model);
        }

        [ActionName("new-hairstyle-booking")]
        public ActionResult NewHairstyleBooking(string hook)
        {
            var model = new NewHairstyleBookingModel {Hook = hook};

            return Json(new { Success = true, Html = this.Partial("PartialNewHairstyleBooking", model) }, 
                JsonRequestBehavior.AllowGet);
        }

        [ActionName("new-hairstyle-booking"), HttpPost]
        public ActionResult NewHairstyleBooking(NewHairstyleBookingModel model)
        {
            if(ModelState.IsValid)
            {
                var userId = _webContext.CurrentUserId;
                var salonId = _salonService.GetSalonId(model.Hook);
                var date = DateTime.UtcNow;
                var booking = Mapper.Map<NewHairstyleBookingModel, HairstyleBooking>(model);
                booking.DateCreated = DateTime.UtcNow;
                booking.UserId = userId;
                booking.SalonId = salonId;
                booking.Identifier = Guid.NewGuid();
                booking.StatusId = (int) HairBookingStatus.Pending;

                using (var fileStream = new MemoryStream(model.ImageData))
                {
                    var image = new KalikoImage(fileStream);
                    string fileName = "makeover_" + Guid.NewGuid().ToString().ToLower().Replace("-", "_");
                    var filePath = Helper.ConstructFilePath(fileName, Helper.MakeoverImageFilePath, false);
                    string directory = Path.GetDirectoryName(filePath + ".jpg");

                    Helper.CreateDirectoryIfNotExist(directory);

                    var thumb50 = image.GetThumbnailImage(50, 50, ThumbnailMethod.Crop);
                    var thumb150 = image.GetThumbnailImage(150, 150, ThumbnailMethod.Crop);
                    var thumb250 = image.GetThumbnailImage(250, 250, ThumbnailMethod.Crop);

                    image.SaveJpg(filePath + "_o.jpg", 90);
                    thumb50.SaveJpg(filePath + "_50.jpg", 90);
                    thumb150.SaveJpg(filePath + "_150.jpg", 90);
                    thumb250.SaveJpg(filePath + "_250.jpg", 90);

                    booking.ImageFileName = fileName;
                }

                _bookingService.Save(booking, null);
                _cache.DeleteItems("hairstylebookings_" + _salonService.GetSalonUserId(model.Hook));

                return Json(new {Success = true, Message = "Booking was successful"});
            }

            return Json(new {Success = false, Message = "Booking was unsuccessful"});
        }

        [HttpPost, ActionName("add-comment")]
        public ActionResult AddComment(NewBookingCommentModel model)
        {
            if(ModelState.IsValid)
            {
                var userId = _webContext.CurrentUserId;
                if(_bookingService.CanModify(userId, model.BookingId))
                {
                    var booking = new HairstyleBooking { BookingId = _bookingService.GetHairstyleBookingId(model.BookingId) };

                    var comment = new Comment
                    {
                        CommentTypeId = 1,
                        Message = model.Message,
                        DateCreated = DateTime.UtcNow,
                        UserId = userId
                    };

                    comment.HairstyleBookings.Add(booking);

                    comment.CommentId = _commentService.Save(comment, null);

                    comment.UserProfile = _userService.GetUserProfile(userId);

                    _cache.DeleteItems("hairstylebooking_" + model.BookingId + "_");
                    _cache.DeleteItems("hairstylebookings_" + userId + "_");

                    ViewBag.CurrentUserId = _webContext.CurrentUserId;

                    return Json(new
                    {
                        Success = true,
                        Html = this.Partial("PartialComment", comment),
                        Message = "Comment added successfully"
                    });
                }

                return Json(new {Success = false, Message = "You do not have the priviledge to perform this action"});
            }

            return Json(new {Success = false, Message = "Comment not posted, error occured"});
        }
    }
}
