using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Moments.Model.EF;
using Newtonsoft.Json;
using System.Text;
namespace Moments.RepositoryDAL
{
    public class LoginDAL
    {
        private MomentsEntities _db = new MomentsEntities();

        public int SaveRegistration(UserRegistration UserRegistration, UserProfile UserProfile)
        {
            try
            {
                int status = 0;
                int profileid = 0;
                if (UserRegistration != null)
                {
                    UserRegistration.CreatedDate = DateTime.Now;
                    UserRegistration.ModifedDate = DateTime.Now;
                    UserRegistration.UserName = UserRegistration.Email;
                    _db.UserRegistrations.Add(UserRegistration);
                    _db.SaveChanges();
                    status = UserRegistration.Id;

                    if (UserProfile != null)
                    {
                        UserProfile.Email = UserRegistration.Email;
                        UserProfile.UserRegistrationId = status;
                        UserProfile.IsActive = true;
                        UserProfile.IsFeatured = false;
                        UserProfile.CreatedDate = DateTime.Now;
                        UserProfile.ModifiedDate = DateTime.Now;
                        _db.UserProfiles.Add(UserProfile);
                        _db.SaveChanges();
                        profileid = UserProfile.Id;


                    }
                }

                return profileid;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public sp_UserLogin_Result LoginUser(Login Login)
        {
            try
            {
                sp_UserLogin_Result User = new sp_UserLogin_Result();
                if (Login != null)
                {
                    User = _db.sp_UserLogin(Login.Email, Login.Password, Login.Type).FirstOrDefault();

                }

                return User;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public bool IsUserExist(string userName)
        {
            UserRegistration user = new UserRegistration();
            try
            {
                if (!String.IsNullOrEmpty(userName))
                {
                    user = _db.UserRegistrations.Where(x => x.Email.ToLower().Equals(userName.ToLower())).FirstOrDefault();
                    if (user == null)
                    {
                        return false;
                    }
                    return true;
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public PromotionEmail CheckUserIsAuhenticatedForRegistration(string Email, string AuthToken)
        {
            try
            {
                PromotionEmail PromotionEmail = new PromotionEmail();
                if (!String.IsNullOrEmpty(Email) && !String.IsNullOrEmpty(AuthToken))
                {
                     PromotionEmail = _db.PromotionEmails.Where(x => x.Email.ToLower().Equals(Email.ToLower()) && x.AuthToken == AuthToken).FirstOrDefault();
                    
                }
                return PromotionEmail;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}