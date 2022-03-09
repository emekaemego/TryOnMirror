using System.Web.Mvc;

namespace SymaCord.TryOnMirror.UI.Web.Areas.VirtualMakeover
{
    public class VirtualMakeoverAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "VirtualMakeover";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("VirtualMakeover_home", "virtual-makeover",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            context.MapRoute(null, "virtual-makeover/s/{code}",
                             new {controller = "Home", action = "S", id = UrlParameter.Optional});

            context.MapRoute("virtual-makeover", "virtual-makeover",
                             new {controller = "Home", action = "Index", id = UrlParameter.Optional});

            context.MapRoute(
                "VirtualMakeover_default",
                "virtual-makeover/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
