using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class ReportingModel
    {
        public List<Relation> Relation { get; set; }

        public List<NodeModel> Nodes { get; set; }

        public string Owner { get; set; }

        public int Branches { get; set; }

        public string StoryName { get; set; }
        public string Image { get; set; }
    }
}