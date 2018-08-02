using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moments.Model.EF;
using System.Web.Mvc;

namespace Moments.ViewModels
{
    public class SuperAdminViewModel
    {
        public UserRegistration UserRegistration { get; set; }
        public string LimitationMessage { get; set; }
        public UserProfile UserProfile { get; set; }
        public PromotionEmail PromotionEmail { get; set; }
        public List<PromotionEmail> PromotionEmailList { get; set; }
        public sp_GetUserProfileAndUserRegistrationRecord_Result GetUserProfileAndUserRegistration { set; get; }
        public List<sp_GetUserProfileAndUserRegistrationRecord_Result> GetUserProfileAndUserRegistrationRecord { set; get; }
        public List<sp_GetAllUsers_Result> GetAllUsers { set; get; }

        public IEnumerable<SelectListItem> DepartmentList { set; get; }
        public IEnumerable<SelectListItem> UserType { set; get; }
    }
}