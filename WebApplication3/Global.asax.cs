﻿using Castle.Windsor;
using Castle.Windsor.Installer;
using ReservationSystem.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using log4net.Config;

namespace ReservationSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer container;
        private static readonly ILog log = LogManager.GetLogger(typeof(Startup));


        private static void BootstrapContainer()
        {
            container = new WindsorContainer().Install(FromAssembly.This());

            var contollerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(contollerFactory);
        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MvcApplication.BootstrapContainer();
            XmlConfigurator.Configure();
            log.Info("Application running....");

        }

        protected void Application_End()
        {
            container.Dispose();
        }
    }
}
