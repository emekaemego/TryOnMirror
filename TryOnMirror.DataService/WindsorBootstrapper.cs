using Castle.Windsor;
using Castle.MicroKernel.Registration;

namespace SymaCord.TryOnMirror.DataService
{
    public class WindsorBootstrapper
    {
        public static void Initialize(IWindsorContainer container)
        {
            container.Register(AllTypes.Pick().FromAssemblyNamed("TryOnMirror.DataAccess")
                .If(s => s.Name.EndsWith("Repository"))
                                   .WithService.FirstInterface().Configure(c => c.LifeStyle.Transient));

            container.Register(AllTypes.Pick().FromAssemblyNamed("TryOnMirror.DataService").If(s => s.Name.EndsWith("Service"))
                                   .WithService.FirstInterface().Configure(c => c.LifeStyle.Transient));
        }
    }
}