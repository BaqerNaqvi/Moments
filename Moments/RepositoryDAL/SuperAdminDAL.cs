using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moments.Model.EF;

namespace Moments.RepositoryDAL
{
    public class SuperAdminDAL
    {
        MomentsEntities _db = new MomentsEntities();
        #region Customer
        public int ChangeCustomerBlockStatus(int Id, bool ActiveStatus)
        {
            try
            {
                int status = 0;
                List<UserProfile> rec = _db.UserProfiles.Where(x => x.UserRegistrationId == Id).ToList();
                if (rec.Count > 0)
                {
                    for (int i = 0; i < rec.Count; i++)
                    {
                        rec[i].IsBlocked = ActiveStatus;
                        status = 1;
                    }
                }
                _db.SaveChanges();
                return status;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<sp_GetAllUsers_Result> GetUsersList(string RequestStatus, string type, bool isactive, bool isblocked, string startdate, string enddate)
        {
            try
            {
                List<sp_GetAllUsers_Result> vendors = new List<sp_GetAllUsers_Result>();

                DateTime? datestart = null;
                DateTime? dateend = null;
                if (startdate != null && enddate != null)
                {
                    datestart = Convert.ToDateTime(startdate);
                    dateend = Convert.ToDateTime(enddate);
                }
                if (RequestStatus == "All")
                {
                    vendors = _db.sp_GetAllUsers(type, isactive, null, null, null,null).ToList();
                }
                else if (RequestStatus == "DropdownCustomerList")
                {
                    vendors = _db.sp_GetAllUsers(type, isactive, isblocked, null, null,null).ToList();
                }
                else
                {
                    vendors = _db.sp_GetAllUsers(type, isactive, isblocked, datestart, dateend,null).ToList();
                }
                return vendors;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<sp_GetUserProfileAndUserRegistrationRecord_Result> GetUserProfileAndVendorRegistrationRecord(int? CustomerProfileId, string UserType)
        {
            try
            {
                List<sp_GetUserProfileAndUserRegistrationRecord_Result> user = new List<sp_GetUserProfileAndUserRegistrationRecord_Result>();
                if (UserType == "SuperAdmin")
                {
                    user = _db.sp_GetUserProfileAndUserRegistrationRecord(null).Where(x => x.Type != "SuperAdmin").ToList();
                }
                else if (UserType == "Admin")
                {
                    user = _db.sp_GetUserProfileAndUserRegistrationRecord(CustomerProfileId).Where(x => x.Type != "SuperAdmin").ToList();
                }
                else
                {
                    user = _db.sp_GetUserProfileAndUserRegistrationRecord(CustomerProfileId).Where(x => x.Type != "SuperAdmin").ToList();

                }

                return user;
            }
            catch (Exception ex)
            {
                throw;
                
            }
        }
        public sp_GetUserProfileAndUserRegistrationRecord_Result GetCustomerAgainstCustomerIdForUpdate(int? UserProfileId)
        {
            try
            {
                sp_GetUserProfileAndUserRegistrationRecord_Result userrecord = _db.sp_GetUserProfileAndUserRegistrationRecord(UserProfileId).FirstOrDefault();

                return userrecord;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public int SaveCustomers(CustomerProfileAndRegistration CustomerProfileAndRegistration)
        {
            try
            {
                if (CustomerProfileAndRegistration.UserProfile.Id == 0)
                {
                    //CustomerProfileAndRegistration.VendorRegistration.Type = "Customer";
                    if (CustomerProfileAndRegistration.UserRegistration.Type != null)
                    {
                        CustomerProfileAndRegistration.UserRegistration.Type = CustomerProfileAndRegistration.UserRegistration.Type;
                    }
                    else
                    {
                        CustomerProfileAndRegistration.UserRegistration.Type = "Customer";
                    }
                    _db.UserRegistrations.Add(CustomerProfileAndRegistration.UserRegistration);
                    _db.SaveChanges();

                    CustomerProfileAndRegistration.UserProfile.UserRegistrationId = CustomerProfileAndRegistration.UserRegistration.Id;
                    CustomerProfileAndRegistration.UserProfile.Email = CustomerProfileAndRegistration.UserRegistration.Email;
                    CustomerProfileAndRegistration.UserProfile.IsActive = true;

                    _db.UserProfiles.Add(CustomerProfileAndRegistration.UserProfile);
                    _db.SaveChanges();
                }
                else
                {
                    UserRegistration UserRegistrations = new UserRegistration();
                    UserRegistrations = _db.UserRegistrations.Where(x => x.Id == CustomerProfileAndRegistration.UserRegistration.Id).FirstOrDefault();
                    UserRegistrations.UserName = CustomerProfileAndRegistration.UserRegistration.UserName;
                    UserRegistrations.ConfirmPassword = CustomerProfileAndRegistration.UserRegistration.ConfirmPassword;
                    UserRegistrations.Password = CustomerProfileAndRegistration.UserRegistration.Password;
                    UserRegistrations.Email = CustomerProfileAndRegistration.UserRegistration.Email;
                    if (CustomerProfileAndRegistration.UserRegistration.Type != null)
                    {
                        UserRegistrations.Type = CustomerProfileAndRegistration.UserRegistration.Type;
                    }
                    else
                    {
                        UserRegistrations.Type = "Customer";
                    }

                    _db.SaveChanges();

                    UserProfile UserProfile = new UserProfile();
                    UserProfile = _db.UserProfiles.Where(x => x.Id == CustomerProfileAndRegistration.UserProfile.Id).FirstOrDefault();
                    UserProfile.FirstName = CustomerProfileAndRegistration.UserProfile.FirstName;
                    UserProfile.MiddleName = CustomerProfileAndRegistration.UserProfile.MiddleName;
                    UserProfile.LastName = CustomerProfileAndRegistration.UserProfile.LastName;
                    UserProfile.Email = CustomerProfileAndRegistration.UserRegistration.Email;
                    UserProfile.PhoneNo = CustomerProfileAndRegistration.UserProfile.PhoneNo;
                    UserProfile.MobileNo = CustomerProfileAndRegistration.UserProfile.MobileNo;
                    UserProfile.PrimaryAddress = CustomerProfileAndRegistration.UserProfile.PrimaryAddress;
                    UserProfile.DOB = CustomerProfileAndRegistration.UserProfile.DOB;
                    UserProfile.CNIC = CustomerProfileAndRegistration.UserProfile.CNIC;
                    _db.SaveChanges();
                }


                return CustomerProfileAndRegistration.UserProfile.Id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public int DeleteUser(CustomerProfileAndRegistration CustomerProfileAndRegistration)
        {
            try
            {
                
                UserProfile UserProfile = new UserProfile();
                UserProfile = _db.UserProfiles.Where(x => x.Id == CustomerProfileAndRegistration.UserProfile.Id).FirstOrDefault();
                _db.UserProfiles.Remove(UserProfile);
                _db.SaveChanges();

                UserRegistration UserRegistration = new UserRegistration();
                UserRegistration = _db.UserRegistrations.Where(x => x.Id == CustomerProfileAndRegistration.UserRegistration.Id).FirstOrDefault();
                _db.UserRegistrations.Remove(UserRegistration);
                _db.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        #region  Lookup
        public List<sp_GetLookupsByLookupsType_Result> GetLookupsByLookupType(bool? IsActive, string LookupType)
        {
            try
            {
                List<sp_GetLookupsByLookupsType_Result> messages = _db.sp_GetLookupsByLookupsType(IsActive, LookupType).ToList();

                return messages;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        #region Invited Users
        public List<PromotionEmail> GetInvitedUsers(int UserProfileId)
        {
            try
            {
                List<PromotionEmail> PromotionEmail = _db.PromotionEmails.Where(x=>x.InvitedBy == UserProfileId).ToList();

                return PromotionEmail;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string SaveInvitedUsers(int UserProfileId, string UserType, PromotionEmail PromotionEmail)
        {
            try
            {
                string status = "";
                if (PromotionEmail.Id == 0)
                {
                    PromotionEmail PromotionEmailsForSave = new PromotionEmail();
                    PromotionEmailsForSave = _db.PromotionEmails.Where(x=>x.Email == PromotionEmail.Email).FirstOrDefault();

                    List<PromotionEmail> PromotionEmailsForSaveList = new List<PromotionEmail>();
                    PromotionEmailsForSaveList = _db.PromotionEmails.Where(x => x.InvitedBy == UserProfileId).ToList();

                    if (PromotionEmailsForSave == null)
                    {
                        if (UserType == "SuperAdmin" && PromotionEmailsForSaveList.Count >= 4000)
                        {
                          status = "InvitationExceeded";
                        } 
                        else if (UserType == "Customer" && PromotionEmailsForSaveList.Count >= 10)
                        {
                            status = "InvitationExceeded";
                        }
                        else if (UserType == "Sub Customer")
                        {
                            status = "NotPermission";
                        }
                        else 
                        {
                            _db.PromotionEmails.Add(PromotionEmail);
                            _db.SaveChanges();
                            status = "Success";
                        }
                        
                    }
                    else
                    {
                        status = "AlreadyExists";
                    }
                   
                }
                else
                {
                    PromotionEmail PromotionEmails = new PromotionEmail();
                    PromotionEmails = _db.PromotionEmails.Where(x => x.Id == PromotionEmail.Id).FirstOrDefault();
                    PromotionEmails.UserFullName = PromotionEmail.UserFullName;
                    PromotionEmails.Email = PromotionEmail.Email;
                    PromotionEmails.Message = PromotionEmail.Message;
                    _db.SaveChanges();
                }


                return status;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }

}