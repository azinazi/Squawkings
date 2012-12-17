using System.Web;
using System.Web.Mvc;
using SchoStack.Web.Html;

namespace Squawkings.Models
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString MenuLink(
            this HtmlHelper htmlHelper,
            string linkText,
            string actionName,
            string controllerName,
            object routValues
            )
        {
            var tag = new TagBuilder("li");
            string currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
            string currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");

            if (linkText == "Logon" && HttpContext.Current.User.Identity.Name != "")
            {
                linkText = "Logoff";
                actionName = "Logoff";
            }
            else if (linkText == "Logon" && HttpContext.Current.User.Identity.Name == "")
            {
                linkText = "Logon";
                actionName = "Index";
            }

            if (currentController == "Profile")
                currentAction = "IndexById";

            if (actionName == currentAction && controllerName == currentController)
            {
                tag.AddCssClass("menu-item active");
            }
            else
            {
                tag.AddCssClass("menu-item inactive");
            }

            tag.InnerHtml = TagExtensions.Link(htmlHelper, linkText, actionName, controllerName, routValues).ToString();

            return MvcHtmlString.Create(tag.ToString());
        }
    }
}