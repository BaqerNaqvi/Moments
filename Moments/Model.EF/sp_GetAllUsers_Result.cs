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
    
    public partial class sp_GetAllUsers_Result
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
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
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }
        public int UserProfileId { get; set; }
        public int Id { get; set; }
        public string Type { get; set; }
        public Nullable<bool> IsFeatured { get; set; }
    }
}
