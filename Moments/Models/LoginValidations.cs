using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Moments.Model.EF;

namespace Moments.Model.EF
{
    public static class CurrentSessionData
    {
        public static int UserProfileId { set; get; }
        public static int UserRegistrationId { set; get; }
        public static string UserType { set; get; }
        public static string UserName { set; get; }
        public static string UserEmail { set; get; }
        public static string UserFullName { set; get; }
        public static string RegistrationDate { set; get; }
        public static int CurrentSelectedPortalId { set; get; }
        public static string CurrentSelectedPortalName { set; get; }
    }
    public class Login
    {
        [Required(ErrorMessage="This Field Is Required.")]
        public string Email { get; set; }

        [Required(ErrorMessage="This Field Is Required.")]
        public string Password { get; set; }

        public string Type { get; set; }
    }
    #region Login
    [MetadataType(typeof(UserRegistration.Metadata))]
    public partial class UserRegistration
    {
        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm Password*")]
        public string ConfirmPassword { get; set; }

        public sealed class Metadata
        {
            public int Id { get; set; }

            //[Required(ErrorMessage="This Field Is Required.")]
            public string UserName { get; set; }

            [Required(ErrorMessage="This Field Is Required.")]
            [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
            [DataType(DataType.Password)]
            [Compare("ConfirmPassword")]
            [Display(Name = "Password*")]
            public string Password { get; set; }

            //[Required(ErrorMessage="This Field Is Required.")]
            [Display(Name = "Type")]
            public string Type { get; set; }

            [Required(ErrorMessage="This Field Is Required.")]
            [Display(Name = "Email*")]
            public string Email { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public Nullable<int> CreatedBy { get; set; }
            public Nullable<System.DateTime> ModifedDate { get; set; }
            public Nullable<int> ModifiedBy { get; set; }

        }
    }
    [MetadataType(typeof(UserProfile.Metadata))]
    public partial class UserProfile
    {
        public sealed class Metadata
        {
            public int Id { get; set; }
            public Nullable<int> UserRegistrationId { get; set; }

            [Required(ErrorMessage="This Field Is Required.")]
            [Display(Name = "First Name*")]
            public string FirstName { get; set; }
            public string MiddleName { get; set; }

            [Required(ErrorMessage="This Field Is Required.")]
            [Display(Name = "Last Name*")]
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNo { get; set; }
            public string MobileNo { get; set; }
            public string DOB { get; set; }
            public string Gender { get; set; }
            public string PrimaryAddress { get; set; }
            public string SecondryAddress { get; set; }
            public string CNIC { get; set; }
            public string Status { get; set; }
            public bool IsActive { get; set; }
            public Nullable<bool> IsFeatured { get; set; }
            public bool IsBlocked { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public Nullable<int> CreatedBy { get; set; }
            public Nullable<System.DateTime> ModifiedDate { get; set; }
            public Nullable<int> ModifiedBy { get; set; }

        }
    }
    [MetadataType(typeof(sp_UserLogin_Result.Metadata))]
    public partial class sp_UserLogin_Result
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public sealed class Metadata
        {


        }
    }
    public class CustomerProfileAndRegistration
    {
        public UserProfile UserProfile { set; get; }
        public UserRegistration UserRegistration { set; get; }
    }
    #endregion
}