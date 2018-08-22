using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class NodeModel
    {
        public string id { get; set; }
        public string parentId { get; set; }
        public string storyId { get; set; }
        public string label { get; set; }
        public string image { get; set; }
        public string Contents { get; set; }

        public int size { get; set; }
        public int borderWidth { get; set; }

        public string shape { get; set; }
        public string type { get; set; }

        public string NodeAuthor { get; set; }

        public string group { get; set; }
        public string FriendlinessWeightColor { get; set; }

    }
}