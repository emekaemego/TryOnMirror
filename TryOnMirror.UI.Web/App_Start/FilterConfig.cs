using System.Web.Mvc;
using SymaCord.TryOnMirror.UI.Web.Filters;

namespace SymaCord.TryOnMirror.UI.Web.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ActionParameterFilterAttribute());
        }
    }
}