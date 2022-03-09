using System.Collections.Generic;
using System.Web.Mvc;
using Castle.Windsor;
using SymaCord.TryOnMirror.UI.Web.Utils;

namespace SymaCord.TryOnMirror.UI.Web.App_Start
{
    public class WindsorActionInvoker : ControllerActionInvoker
    {
        readonly IWindsorContainer container = WindsorBootstrapper.Container;
        
        protected override ActionExecutedContext InvokeActionMethodWithFilters(ControllerContext controllerContext,
                IList<IActionFilter> filters, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters)
        {
            foreach (IActionFilter actionFilter in filters)
            {
                container.Kernel.InjectProperties(actionFilter);
            }

            return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
        }
    }

}