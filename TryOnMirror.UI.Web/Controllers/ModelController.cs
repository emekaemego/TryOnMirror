using System.Web.Mvc;
using SymaCord.TryOnMirror.UI.Web.Utils;

namespace SymaCord.TryOnMirror.UI.Web.Controllers
{
    public class ModelController : Controller
    {
        private IWebContext _webContext;

        public ModelController(IWebContext webContext)
        {
            _webContext = webContext;
        }
        
        //public ActionResult UploadFace(UploadCalibrateModel model, HttpPostedFileBase image)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        if(image != null)
        //        {
        //        }
        //    }
        //}
    }
}
