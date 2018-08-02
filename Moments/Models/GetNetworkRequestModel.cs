using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class GetNetworkRequestModel
    {
        public string StoryId { get; set; }
    }

    public class ExportNodesRequestModel
    {
        public List<string> Nodes { get; set; }

        public string StoryId { get; set; }
    }
}