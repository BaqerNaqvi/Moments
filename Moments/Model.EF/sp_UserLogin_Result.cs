//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Moments.Model.EF
{
    using System;
    
    public partial class sp_UserLogin_Result
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string Email { get; set; }
        public Nullable<int> RegistrationBy { get; set; }
        public int UserRegistrationId { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<int> UserProfileId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNo { get; set; }
        public Nullable<bool> ProfileIsActive { get; set; }
        public Nullable<bool> IsBlocked { get; set; }
        public Nullable<int> ProfileCreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}