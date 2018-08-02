using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moments.Model.EF;
using System.Web.Mvc;
using System.Collections;

namespace Moments.ViewModels
{
    public class LoginViewModel
    {
        
        public UserRegistration UserRegistration { set; get; }
        public UserProfile UserProfile { set; get; }
        public Login Login { set; get; }
        public string RegisterUserEmail { set; get; }
        public string AuthToken { set; get; }

    }
}