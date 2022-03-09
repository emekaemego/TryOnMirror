using System.Web.Mvc;

namespace SymaCord.TryOnMirror.UI.Web.Areas.Calibration
{
    public class CalibrationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Calibration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("Calibration_home", "calibration",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );

            context.MapRoute(
                "Calibration_default",
                "calibration/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
