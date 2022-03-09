using System.IO;
using System.Web.Mvc;
using SymaCord.TryOnMirror.UI.Web.Utils;
using SymaCord.TryOnMirror.UI.Web.ViewModels;

namespace SymaCord.TryOnMirror.UI.Web.Controllers
{
    public class HomeController : Controller
    {
        private IEmailService _emailService;

        public HomeController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View(new ContactModel());
        }

        [HttpPost]
        public ActionResult Contact(ContactModel model)
        {
            if(ModelState.IsValid)
            {
                var sr = new StringWriter();
                
                sr.Write("Name: "+ model.Name);
                sr.WriteLine(sr.NewLine);
                sr.WriteLine("Message");
                sr.WriteLine(model.Message);

                _emailService.SendEmail(model.Email, "info@tryonmirror.com", "TryOn Mirror Inquery - Website", sr.ToString());

                TempData["Message"] = "Your request was submitted successfully";

                return RedirectToAction("contact");
            }

            return View(model);
        }

        public ActionResult License()
        {
            return View();
        }
    }
}
