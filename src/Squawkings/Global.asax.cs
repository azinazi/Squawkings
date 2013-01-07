using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation;
using FluentValidation.Mvc;
using SchoStack.Web;
using SchoStack.Web.ActionControllers;
using SchoStack.Web.Conventions;
using SchoStack.Web.Conventions.Core;
using Squawkings.Models;
using FluentValidation.Attributes;
using StructureMap;

namespace Squawkings
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute("logon", "logon", new { controller = "Logon", action = "Index" });
            routes.MapRoute("logoff", "logoff", new { controller = "Logon", action = "logoff" });
            routes.MapRoute("global", "global", new { controller = "Global", action = "Index" });
            routes.MapRoute("home", "", new { controller = "Home", action = "Index" });
            routes.MapRoute("total", "", new { controller = "Home", action = "Total" });
            routes.MapRoute("image", "Image", new { controller = "Profile", action = "Image" });
            routes.MapRoute("profilebyid", "profile/{userId}", new { controller = "Profile", action = "IndexById" });
            routes.MapRoute(
                "profile", 
                "{username}", 
                new { controller = "Profile", action = "Index", id = UrlParameter.Optional } 
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            HtmlConventionFactory.Add(new DefaultHtmlConventions());
            HtmlConventionFactory.Add(new DataAnnotationHtmlConventions());
            HtmlConventionFactory.Add(new DataAnnotationValidationHtmlConventions());
            HtmlConventionFactory.Add(new UploadFileConventions());

            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider( new AttributedValidatorFactory()));
        }
    }

    public class StructureMapValidatorFactory : ValidatorFactoryBase
    {
        public override IValidator CreateInstance(Type validatorType)
        {
            return ObjectFactory.TryGetInstance(validatorType) as IValidator;
        }
    }
  
}