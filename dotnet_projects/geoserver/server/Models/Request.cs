using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;

namespace server
{
    public class BBox
    {
        public double minx { get; set; }
        public double miny { get; set; }
        public double maxx { get; set; }
        public double maxy { get; set; }
    }

    public class Request
    {
        public int width { get; set; }
        public int height { get; set; }
        public string srs { get; set; }
        public string layers { get; set; }
        public string styles { get; set; }
        public string format { get; set; }
        public string request { get; set; }
        public string bbox { get; set; }
    }
}
