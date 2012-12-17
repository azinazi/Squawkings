using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPoco;

namespace Squawkings.Models
{
    public class TemplateBuilder : SqlBuilder
    {

        public Template temp1 { get; set; }
        public Template temp2 { get; set; }

    }
}
