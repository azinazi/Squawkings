using System.Web;
using SchoStack.Web.Conventions.Core;

namespace Squawkings.Models
{
    public class UploadFileConventions : HtmlConvention
    {
        public UploadFileConventions()
        {
            Inputs.If<HttpPostedFileBase>().Modify((h, r) => h.Attr("type", "file"));
        }
    }
}