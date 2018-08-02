using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Moments.RepositoryDAL;
using Moments.Utility;
using Moments.ViewModels;
using Moments.Model.EF;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;

namespace Moments.Controllers
{
    public class SuperAdminController : Controller
    {
        // GET: SuperAdmin
        public ActionResult Index()
        {
            return View();
        }
        #region Customer
        [HttpGet]
        public ActionResult ChangeCustomerBlockStatus(int Id, bool ActiveStatus)
        {
            try
            {
                int status = 0;
                SuperAdminDAL DAL = new SuperAdminDAL();
                status = DAL.ChangeCustomerBlockStatus(Id, ActiveStatus);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        [HttpGet]
        public ActionResult GetCustomerAgainstCustomerIdForUpdate(int? UserProfileId)
        {
            SuperAdminViewModel viewmodel = new SuperAdminViewModel();
            SuperAdminDAL DAL = new SuperAdminDAL();
            viewmodel.GetUserProfileAndUserRegistration = DAL.GetCustomerAgainstCustomerIdForUpdate(UserProfileId);
            return Json(viewmodel.GetUserProfileAndUserRegistration, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Customer()
        {
            try
            {
                int status = 0;
                SuperAdminDAL DAL = new SuperAdminDAL();
                MVCUtility utility = new MVCUtility();
                SuperAdminViewModel viewModel = new SuperAdminViewModel();
                viewModel.DepartmentList = utility.GetLookupsByLookupType(true, "Department");
                viewModel.UserType = utility.GetLookupsByLookupTypeTranslatedAsValueForUserType(true, "UserType", Convert.ToString(Session["UserType"]));
                viewModel.GetUserProfileAndUserRegistrationRecord = DAL.GetUserProfileAndVendorRegistrationRecord(null, Convert.ToString(Session["UserType"]));
                return View(viewModel);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        [HttpPost]
        public ActionResult SaveCustomers(CustomerProfileAndRegistration CustomerProfileAndRegistration)
        {
            try
            {
                int status = 0;
                SuperAdminDAL DAL = new SuperAdminDAL();
                SuperAdminViewModel viewModel = new SuperAdminViewModel();
                status = DAL.SaveCustomers(CustomerProfileAndRegistration);
                viewModel.GetUserProfileAndUserRegistrationRecord = DAL.GetUserProfileAndVendorRegistrationRecord(null, Convert.ToString(Session["UserType"]));
                return PartialView("_ManageCustomer", viewModel);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        [HttpGet]
        public ActionResult GetCustomersList()
        {
            try
            {
                SuperAdminDAL DAL = new SuperAdminDAL();
                SuperAdminViewModel viewModel = new SuperAdminViewModel();
                string RequestStatus = "All";
                viewModel.GetAllUsers = DAL.GetUsersList(RequestStatus, null, true, false, null, null);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        [HttpGet]
        public ActionResult DeleteUser(int UserProfileId, int UserRegistrationId)
        {
            try
            {
                SuperAdminViewModel viewmodel = new SuperAdminViewModel();
                SuperAdminDAL DAL = new SuperAdminDAL();
                CustomerProfileAndRegistration CustomerProfileAndRegistration = new CustomerProfileAndRegistration();
                CustomerProfileAndRegistration.UserRegistration = new UserRegistration();
                CustomerProfileAndRegistration.UserProfile = new UserProfile();
                CustomerProfileAndRegistration.UserRegistration.Id = Convert.ToInt32(UserRegistrationId);
                CustomerProfileAndRegistration.UserProfile.Id = Convert.ToInt32(UserProfileId);
                int status = DAL.DeleteUser(CustomerProfileAndRegistration);
                viewmodel.GetUserProfileAndUserRegistrationRecord = DAL.GetUserProfileAndVendorRegistrationRecord(null, Convert.ToString(Session["UserType"]));
                return PartialView("_ManageCustomer", viewmodel);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        [HttpGet]
        public ActionResult GetCustomersListFilter(bool IsBlocked, string DateRange)
        {
            try
            {
                SuperAdminDAL DAL = new SuperAdminDAL();
                SuperAdminViewModel viewModel = new SuperAdminViewModel();
                string RequestStatus = "Filtered";
                //DateTime? DateStart = null;
                //DateTime? DateEnd = null;
                string DateStart = null;
                string DateEnd = null;
                if (DateRange != "")
                {
                    string[] DateFromTo = DateRange.Split('-');
                    DateStart = DateFromTo[0];
                    DateEnd = DateFromTo[1];
                }

                viewModel.GetAllUsers = DAL.GetUsersList(RequestStatus, null, true, IsBlocked, DateStart, DateEnd);
                return PartialView("_ManageGetCustomerList", viewModel);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        #endregion
        #region Invited Users
        [HttpGet]
        public ActionResult InvitedUsers()
        {
            try
            {
                SuperAdminDAL DAL = new SuperAdminDAL();
                SuperAdminViewModel viewModel = new SuperAdminViewModel();
                int UserProfileId = Convert.ToInt32(Session["UserProfileId"]);
                viewModel.PromotionEmailList = DAL.GetInvitedUsers(UserProfileId);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            controllerContext.Controller.ViewData.Model = model;

            using (var stringWriter = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
        [HttpPost]
        public ActionResult SaveInvitedUsers(PromotionEmail PromotionEmail)
        {
            try
            {
                string status = "";
                string LimitationMessage = "";
                SuperAdminDAL DAL = new SuperAdminDAL();
                SuperAdminViewModel viewModel = new SuperAdminViewModel();
                int UserProfileId = Convert.ToInt32(Session["UserProfileId"]);
                string UserType = Convert.ToString(Session["UserType"]);
                string AuthToken = Guid.NewGuid().ToString();
                if (UserType == "SuperAdmin")
                {
                    AuthToken = AuthToken + "~10";
                    PromotionEmail.AllowedInvitations = 10;
                }
                else
                {
                    AuthToken = AuthToken + "~0";
                    PromotionEmail.AllowedInvitations = 0;
                }
                PromotionEmail.AuthToken = AuthToken;
                PromotionEmail.InvitedBy = UserProfileId;
                PromotionEmail.RegistrationLink = "https://moments.fooddesignbrazil.com/Login/RegisterUser?Email=" + PromotionEmail.Email + "&AuthToken="+AuthToken;
                
                status = DAL.SaveInvitedUsers(UserProfileId,UserType, PromotionEmail);

                if (status == "AlreadyExists")
                {
                    LimitationMessage = "This User Is Already Invited.";
                }
                else if (status == "Success")
                {
                    //sending email 
                       SendInvitationEmail(PromotionEmail.Email, PromotionEmail.RegistrationLink);
                    //sending email end
                    LimitationMessage = "Record Save Successfully.";
                }
                else if (status== "InvitationExceeded")
                {
                    LimitationMessage = "Your Invitation Count Reached To Limit.";
                }
                else if (status == "NotPermission")
                {
                    LimitationMessage = "Your have Not Permission To Invite.";
                }
                else
                {
                    LimitationMessage = "";
                }

                viewModel.PromotionEmailList = DAL.GetInvitedUsers(UserProfileId);
                
                var ManageInvitedUsers = RenderRazorViewToString(this.ControllerContext, "_ManageInvitedUsers", viewModel);

                string data = JsonConvert.SerializeObject(new { LimitationMessage, ManageInvitedUsers });
                return Json(data, JsonRequestBehavior.AllowGet);

              //  return PartialView("_ManageInvitedUsers", viewModel);

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        [HttpPost]
        public ActionResult SendEmailsToUsers(List<PromotionEmail> PromotionEmail)
        {
            try
            {
                if (PromotionEmail.Count > 0)
                {
                    for(int i=0; i < PromotionEmail.Count; i++)
                    {
                        string status = "";
                        string LimitationMessage = "";
                        SuperAdminDAL DAL = new SuperAdminDAL();
                        SuperAdminViewModel viewModel = new SuperAdminViewModel();
                        int UserProfileId = Convert.ToInt32(Session["UserProfileId"]);
                        string UserType = Convert.ToString(Session["UserType"]);
                        string AuthToken = Guid.NewGuid().ToString();
                        if (UserType == "SuperAdmin")
                        {
                            AuthToken = AuthToken + "~10";
                            PromotionEmail[i].AllowedInvitations = 10;
                        }
                        else
                        {
                            AuthToken = AuthToken + "~0";
                            PromotionEmail[i].AllowedInvitations = 0;
                        }
                        PromotionEmail[i].AuthToken = AuthToken;
                        PromotionEmail[i].InvitedBy = UserProfileId;
                        PromotionEmail[i].RegistrationLink = "https://moments.fooddesignbrazil.com/Login/RegisterUser?Email=" + PromotionEmail[i].Email + "&AuthToken=" + AuthToken;

                        status = DAL.SaveInvitedUsers(UserProfileId, UserType, PromotionEmail[i]);

                        if (status == "AlreadyExists")
                        {
                            LimitationMessage = "This User Is Already Invited.";
                        }
                        else if (status == "Success")
                        {
                            //sending email 
                            SendInvitationEmail(PromotionEmail[i].Email, PromotionEmail[i].RegistrationLink);
                            //sending email end
                            LimitationMessage = "Record Save Successfully.";
                        }
                        else if (status == "InvitationExceeded")
                        {
                            LimitationMessage = "Your Invitation Count Reached To Limit.";
                        }
                        else if (status == "NotPermission")
                        {
                            LimitationMessage = "Your have Not Permission To Invite.";
                        }
                        else
                        {
                            LimitationMessage = "";
                        }

                       // viewModel.PromotionEmailList = DAL.GetInvitedUsers(UserProfileId);

                       // var ManageInvitedUsers = RenderRazorViewToString(this.ControllerContext, "_ManageInvitedUsers", viewModel);

                       // string data = JsonConvert.SerializeObject(new { LimitationMessage, ManageInvitedUsers });
                    }
                    
                }
                return Json("Successfully Sent", JsonRequestBehavior.AllowGet);
                //  return PartialView("_ManageInvitedUsers", viewModel);

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public void SendInvitationEmail(string userEmail, string key)
        {
            //string email = "mozam0094@gmail.com"; // Email to send mail
            //string message = "<head><link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css' integrity='sha384-WskhaSGFgHYWDcbwN70/dfYBj47jz9qbsMId/iRN3ewGhXQFZCSftd1LZCfmhktB' crossorigin='anonymous'></head><div class='col-md-12' style='background:#b61628;text-align:center;font-family:sans-serif;min-height: 500px;color:  white; font-size:2rem;' ><h1 style='padding-top: 100px; margin-bottom:  50px;'><strong>Moments Email</strong></h1><p style='margin-bottom:50px;'>Click on button and create your own story, Connect with our global story network.</p><br><button onclick='location.href = '" + key + "'' class='btn btn-warning' style='  background-color: #5a5957; border-radius: 0px; border-color: #928f85; color: white;'>Register Your Account</button></div>";
            string message = "<head><link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css' integrity='sha384-WskhaSGFgHYWDcbwN70/dfYBj47jz9qbsMId/iRN3ewGhXQFZCSftd1LZCfmhktB' crossorigin='anonymous'></head><div class='col-md-12' style='background:#b61628;text-align:center;font-family:sans-serif;min-height: 500px;color:  white; font-size:2rem;' ><h1 style='padding-top: 100px; margin-bottom:  50px;'><strong>Moments Email</strong></h1><p style='margin-bottom:50px;'>Click on button and create your own story, Connect with our global story network.</p><br><a class='btn btn-secondary' style='  background-color: #5a5957; border-radius: 0px; border-color: #928f85; color: white;' href='" + key + "' class='button title-button button-solid'><i class='icon-github-circled'></i>Register Your Account</a></div>";
            
            string subject = "Promotion Emails";
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.fooddesignbrazil.com");

                mail.From = new MailAddress("info@fooddesignbrazil.com");
                mail.To.Add(userEmail);
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = message;
                SmtpServer.Port = 25;
                SmtpServer.Credentials = new System.Net.NetworkCredential("info@fooddesignbrazil.com", "Ahmad-123456");
                SmtpServer.EnableSsl = false;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}