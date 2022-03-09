using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using SymaCord.TryOnMirror.CV;
using SymaCord.TryOnMirror.CV.Impl;
using SymaCord.TryOnMirror.Core.Util;
using SymaCord.TryOnMirror.Core.Util.Impl;
using SymaCord.TryOnMirror.UI.Web.Utils;
using SymaCord.TryOnMirror.UI.Web.Utils.Impl;

namespace SymaCord.TryOnMirror.UI.Web.App_Start
{
    public static class WindsorBootstrapper
    {
        public static IWindsorContainer Container { get; private set; }

        public static void Initialize()
        {
            Container = new WindsorContainer();
            RegisterControllers();

            Container.Register(Component.For<IConfiguration>().ImplementedBy<AppConfiguration>().LifeStyle.Transient,
                               Component.For<ICache>().ImplementedBy<Cache>().LifeStyle.Transient,
                               Component.For<ICvDetection>().ImplementedBy<CvDetection>().LifeStyle.Transient,
                               Component.For<ICvStasmDetection>().ImplementedBy<CvStasmDetection>().LifeStyle.Transient,
                               Component.For<IWebContext>().ImplementedBy<WebContext>().LifeStyle.Transient,
                               Component.For<IEmailService>().ImplementedBy<EmailService>().LifeStyle.Transient,
                               Component.For<IActionInvoker>().ImplementedBy<WindsorActionInvoker>().LifeStyle.Singleton);

            DataService.WindsorBootstrapper.Initialize(Container);
        }

        private static void RegisterControllers()
        {
            var controllerTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
                                  where typeof(IController).IsAssignableFrom(t)
                                  select t;
            foreach (Type t in controllerTypes)
                Container.AddComponentLifeStyle(t.FullName, t, LifestyleType.Transient);
        }
    }

}