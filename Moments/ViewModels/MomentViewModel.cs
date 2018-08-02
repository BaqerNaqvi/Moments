using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moments.Model.EF;
using System.Web.Mvc;

namespace Moments.ViewModels
{
    public class MomentViewModel
    {
        public  List<Sp_GetStories_Result>  GetAllStoriesList { get; set; }
        public Sp_GetStories_Result GetStory { get; set; }
        public UserRegistration UserRegistration { set; get; }
        public UserProfile UserProfile { set; get; }
        public List<StoriesNode> StoriesNodeList { set; get; }
        public Login Login { set; get; }
        public string RegisterUserEmail { set; get; }
        public string AuthToken { set; get; }


        // on network screen
        public int Branches { get; set; }
    }
}