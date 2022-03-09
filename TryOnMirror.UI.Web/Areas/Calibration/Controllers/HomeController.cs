using System.Web.Mvc;
using SymaCord.TryOnMirror.DataService.Services;

namespace SymaCord.TryOnMirror.UI.Web.Areas.Calibration.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private ITracedPhotoService _tracedPhotoService;

        public HomeController(ITracedPhotoService tracedPhotoService)
        {
            _tracedPhotoService = tracedPhotoService;
        }

        public ActionResult Index()
        {
            var tracedModelsPhotos = _tracedPhotoService.GetTracedPhotosFileName(null, null, true, null, int.MaxValue);

            return View(tracedModelsPhotos);
        }
    }
}