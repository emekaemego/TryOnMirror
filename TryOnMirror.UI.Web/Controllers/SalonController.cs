using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.DataService.Services;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.Utils;
using SymaCord.TryOnMirror.UI.Web.ViewModels;

namespace SymaCord.TryOnMirror.UI.Web.Controllers
{
    [Authorize]
    public class SalonController : Controller
    {
        //
        // GET: /Salon/
        private ISalonService _salonService;
        private ICache _cache;
        private IWebContext _webContext;
        private IHairstyleBookingService _bookingService;
        private IAddressService _addressService;
        private IEmailService _emailService;

        public SalonController(ISalonService salonService, ICache cache, IWebContext webContext, 
            IHairstyleBookingService bookingService, IAddressService addressService, IEmailService emailService)
        {
            _salonService = salonService;
            _cache = cache;
            _webContext = webContext;
            _bookingService = bookingService;
            _addressService = addressService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        public ActionResult Index(string id)
        {
            /*var salon = _salonService.GetSalonWithContactInfo(id);
            ViewBag.CanModifySalon = _salonService.CanModify(_webContext.CurrentUserId, id);*/

            //ViewBag.RecentBookings = _bookingService.GetHairstyleCustomBookings(null, id, null, 8);

            return View(new EmailModel());
        }

        [HttpPost, AllowAnonymous]
        public ActionResult Index(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                _emailService.SendEmail(model.Email, "info@tryonmirror.com", "Salon Notification", model.Email);

                TempData["Message"] = "Your request was submitted successfully";

                return RedirectToAction("index");
            }

            return View(model);
        }

        [ActionName("makeover-salons")]
        public ActionResult MakeoverSalons(string s, int? p)
        {
            var model = _salonService.GetSalons(s, null, p, 15);

            return Json(new {Success = true, Html = this.Partial("PartialMakeoverSalonList", model)},
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            return View(new NewSalonModel());
        }

        [HttpPost]
        public ActionResult Create(NewSalonModel model)
        {
            if(ModelState.IsValid)
            {
                var salon = Mapper.Map<NewSalonModel, Salon>(model);
                var date = DateTime.UtcNow;
                var userId = _webContext.CurrentUserId;

                salon.DateCreated = date;
                salon.Identifier = Guid.NewGuid();
                salon.UserId = userId;
                salon.Address.DateCreated = date;
                salon.Address.UserId = userId;

               _salonService.Save(salon, null);

                _cache.DeleteItems("salons_");

                return RedirectToAction("index", new {id = salon.Identifier});
            }

            return View();
        }

        public ActionResult Edit(string hook)
        {
            var salon = _salonService.GetSalonWithContactInfo("@" + hook);

            var model = Mapper.Map<Salon, EditSalonModel>(salon);

            ViewBag.AddressModel = (salon.Address!=null) ? Mapper.Map<Address, AddressModel>(salon.Address):
                new AddressModel();
            ViewBag.PhoneAndWebContacts = Mapper.Map<Salon, PhoneAndWebContactModel>(salon);

            return View(model);
        }

        [ActionName("edit-info"), HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInfo(EditSalonModel model)
        {
            if (ModelState.IsValid)
            {
                var salon = Mapper.Map<EditSalonModel, Salon>(model);
                salon.SalonId = _salonService.GetSalonId(model.Hook);

                if (_salonService.CanModify(_webContext.CurrentUserId, salon.Hook))
                {
                    _salonService.Save(salon, new Expression<Func<Salon, object>>[]
                        {
                            x => x.SalonName, x => x.About
                        });

                    return Json(new {Success = true, Message = "Salon saved successfully"});
                }

                return Json(new {Success = false, Message = "You do not have the priviledge to perform this action"});
            }

            return Json(new {Success = false, Html = this.Partial("PartialEditSalon", model)});
        }

        [ActionName("edit-address"), HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAddress(AddressModel model)
        {
            if (ModelState.IsValid)
            {
                var address = Mapper.Map<AddressModel, Address>(model);
                var addressId = _addressService.GetAddressId(model.Hook);
                address.AddressId = addressId;

                if (_salonService.CanModify(_webContext.CurrentUserId, model.Hook))
                {
                    _addressService.Save(address, new Expression<Func<Address, object>>[]
                        {
                            x => x.AddressLine1, x=>x.City, x=>x.State
                        });

                    return Json(new { Success = true, Message = "Address saved successfully" });
                }

                return Json(new { Success = false, Message = "You do not have the priviledge to perform this action" });
            }

            return Json(new { Success = false, Html = this.Partial("PartialEditAddress", model) });
        }

        [ActionName("edit-phone-web-contact"), HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPhoneWebContact(PhoneAndWebContactModel model)
        {
            if (ModelState.IsValid)
            {
                //var salon = Mapper.Map<PhoneAndWebContactModel, Salon>(model);
                var oldSalon = _salonService.GetSalon("@" + model.Hook);
                oldSalon.Website = model.Website;
                oldSalon.PhoneNumbers = model.PhoneNumber;
                oldSalon.Email = model.Email;

                if (_salonService.CanModify(_webContext.CurrentUserId, oldSalon.Hook))
                {
                    _salonService.Save(oldSalon, new Expression<Func<Salon, object>>[]
                        {
                            x => x.PhoneNumbers, x => x.Email, x=>x.Website
                        });

                    return Json(new { Success = true, Message = "Contacts saved successfully" });
                }

                return Json(new { Success = false, Message = "You do not have the priviledge to perform this action" });
            }

            return Json(new { Success = false, Html = this.Partial("PartialEditPhoneAndWebContact", model) });
        }

        public PartialViewResult UserSalonMenu(int currentUserId)
        {
            var model = _salonService.GetSalonsLite(null, currentUserId, null, int.MaxValue);
            
            return PartialView("PartialUserSalonMenu", model);
        }

        public ActionResult List(string s, int?p)
        {
            return View();
        }
    }

    public class EmailModel
    {
        [Required(ErrorMessage="Email is required"), Display(Name="Email address")]
        public string Email { get; set; }
    }
}
