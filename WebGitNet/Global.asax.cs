//-----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

using System;
using System.Web.Configuration;

using Castle.MicroKernel;

using WebGitNet.Authorization;

namespace WebGitNet
{
    using System.Linq;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    public partial class WebGitNetApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer container;

        private static string[] supportedAuthorizationProviders =
        {
            "Permissive",
            "Gitolite"
        };

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomAuthorization());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var routeRegisterers = container.ResolveAll<IRouteRegisterer>();
            foreach (var registerer in routeRegisterers)
            {
                registerer.RegisterRoutes(routes);
            }

            routes.MapRoute(
                "Default",
                "{controller}/{action}",
                new { controller = "Browse", action = "Index" });
        }

        protected void Application_Start()
        {
            Bootstrap();

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            ViewEngines.Engines.Add(new ResourceRazorViewEngine());

            HostingEnvironment.RegisterVirtualPathProvider(new ResourceVirtualPathProvider());
        }

        protected void Application_End()
        {
            container.Dispose();
        }

        private static void Bootstrap()
        {
            var authorizationProviderName = DetermineAuthorizationProvider();

            var directoryFilter = new AssemblyFilter(HostingEnvironment.MapPath("~/Plugins"));

            container = new WindsorContainer()
                        .Install(new AssemblyInstaller(authorizationProviderName))
                        .Install(FromAssembly.InDirectory(directoryFilter));

            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        private static string DetermineAuthorizationProvider()
        {
            string authorizationProviderName = WebConfigurationManager.AppSettings["AuthorizationProvider"];
            if (!supportedAuthorizationProviders.Contains(authorizationProviderName))
            {
                authorizationProviderName = "Permissive";
            }
            authorizationProviderName = string.Format("{0}{1}", authorizationProviderName, "AuthorizationProvider");
            return authorizationProviderName;
        }

        private class AssemblyInstaller : IWindsorInstaller
        {
            private string authorizationProviderName;

            public AssemblyInstaller(string authorizationProviderName)
            {
                this.authorizationProviderName = authorizationProviderName;
            }

            public void Install(IWindsorContainer container, IConfigurationStore configurationStore)
            {
                var directoryFilter = new AssemblyFilter(HostingEnvironment.MapPath("~/Plugins"));
                
                container.Register(Component.For<IWindsorContainer>().Instance(container));

                container.Register(AllTypes.FromThisAssembly()
                                           .BasedOn<IRouteRegisterer>()
                                           .WithService.FromInterface());
                container.Register(AllTypes.FromThisAssembly()
                                           .BasedOn<IController>()
                                           .Configure(c => c.Named(c.Implementation.Name))
                                           .LifestyleTransient());
                container.Register(AllTypes.From(typeof(PluginContentController))
                                           .BasedOn<IController>()
                                           .Configure(c => c.Named(c.Implementation.Name))
                                           .LifestyleTransient());
                container.Register(AllTypes.FromAssemblyInDirectory(directoryFilter)
                                           .BasedOn<IAuthorizationProvider>()
                                           .If(SelectAuthorizationProvider)
                                           .WithService.FromInterface()
                                           .LifestyleSingleton());

            }

            private bool SelectAuthorizationProvider(Type type)
            {
                if (type.Name == authorizationProviderName)
                    return true;

                return false;
            }
        }

        public static IAuthorizationProvider GetAuthorizationProvider()
        {
            IAuthorizationProvider provider = container.Resolve<IAuthorizationProvider>();

            provider.ApplicationPath = HostingEnvironment.ApplicationPhysicalPath;

            return provider;
        }
    }
}
