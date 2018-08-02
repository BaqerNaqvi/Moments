using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class IndexPageModel
    {
        public string Creator { get; set; }
        public string Path { get; set; }

        public bool IsFeatured { get; set; }

        public int Count { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }
    }
}