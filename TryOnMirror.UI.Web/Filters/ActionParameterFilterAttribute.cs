using System.Web.Mvc;
using SymaCord.TryOnMirror.UI.Web.Utils;

namespace SymaCord.TryOnMirror.UI.Web.Filters
{
    public class ActionParameterFilterAttribute : ActionFilterAttribute
    {
        //public IUserService UserService { get; set; }
        public IWebContext WebContext { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var hasCurrentUserId = filterContext.ActionParameters.ContainsKey("currentUserId");

            if (hasCurrentUserId)
            {
                filterContext.ActionParameters["currentUserId"] = WebContext.CurrentUserId;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}