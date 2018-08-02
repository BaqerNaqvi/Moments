using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class Relation
    {
        public string from { get; set; }
        public bool dashes { get; set; }
        
        public string to { get; set; }

        public string color { get; set; }
    }
}