using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Moments.Model.EF;
using Moments.Utility;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using Moments.RepositoryDAL;
using Moments.ViewModels;

namespace Moments.Controllers
{
    public class LoginController : Controller
    {
        // GET: index
        [HttpGet]
        public ActionResult Index()
        {
            return View();
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
        // GET: Login
        [HttpGet]
        public ActionResult Login(string RequestUrl)
        {
            Session["RequestUrl"] = RequestUrl;
            return View();
        }
        // GET: Login
        [HttpPost]
        public ActionResult LoginUser(Login Login)
        {
            LoginDAL dal = new LoginDAL();
            try
            {
                if (Login.Email.ToLower().Contains("superadmin"))
                {
                    Login.Type = "SuperAdmin";
                }
                else 
                {
                    Login.Type = "Customer";
                }
                sp_UserLogin_Result status = dal.LoginUser(Login);
                if (status != null)
                {
                    Session["UserProfileId"] = status.UserProfileId;
                    Session["UserRegistrationId"] = status.UserRegistrationId;
                    Session["UserType"] = status.Type;
                    Session["UserName"] = status.UserName;
                    Session["UserEmail"] = status.Email;
                    Session["UserFullName"] = status.FirstName + ' ' + status.MiddleName + ' ' + status.LastName;
                    if(status.RegistrationDate != null)
                    {
                        Session["RegistrationDate"] = status.RegistrationDate.Value.ToShortDateString();
                    }
                    else
                    {
                        Session["RegistrationDate"] = null;
                    }

                    return Json("SuccessfullLogin", JsonRequestBehavior.AllowGet);
                }

                return Json("EmailPasswordInvalid", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
        //Register User 
        [HttpGet]
        public ActionResult RegisterUser(string Email, string AuthToken)
        {
            Logout(null);

            PromotionEmail PromotionEmail = new PromotionEmail();
            LoginDAL DAL = new LoginDAL();
            LoginViewModel ViewModel = new LoginViewModel();
            PromotionEmail = DAL.CheckUserIsAuhenticatedForRegistration(Email, AuthToken);

            ViewModel.RegisterUserEmail = Email;

            return View(ViewModel);
        }
        [HttpPost]
        public ActionResult SaveUserRegistration(string AuthToken, UserRegistration UserRegistration, UserProfile UserProfile)
        {
            LoginDAL dal = new LoginDAL();
            if (UserRegistration != null && UserProfile != null)
            {

                if (!dal.IsUserExist(UserRegistration.Email.ToLower()))
                {
                    string[] AuthTokenParts;
                    if (AuthToken != "" && AuthToken != null)
                    {
                         AuthTokenParts = AuthToken.Split('~');
                        if (AuthTokenParts.Length != 0)
                        {
                            if (AuthTokenParts[1] == "10")
                            {
                                UserRegistration.Type = "Customer";
                            }
                            else
                            {
                                UserRegistration.Type = "Sub Customer";
                            }

                        }
                    }
                    

                    int status = dal.SaveRegistration(UserRegistration, UserProfile);
                    if (status != 0)
                    {
                        Session["UserProfileId"] = UserProfile.Id;
                        Session["UserRegistrationId"] = UserRegistration.Id;
                        Session["UserType"] = UserRegistration.Type;
                        Session["UserName"] = UserRegistration.UserName;
                        Session["UserEmail"] = UserRegistration.Email;
                        Session["UserFullName"] = UserProfile.FirstName + ' ' + UserProfile.MiddleName + ' ' + UserProfile.LastName;
                        if (UserProfile.CreatedDate != null)
                        {
                            Session["RegistrationDate"] = UserProfile.CreatedDate.Value.ToShortDateString();
                        }
                        else
                        {
                            Session["RegistrationDate"] = null;
                        }
                    }
                    return Json("SuccessFull", JsonRequestBehavior.AllowGet);
                }
                return Json("UserAlreadyExist", JsonRequestBehavior.AllowGet);
            }
            return Json("RegistrationFailed", JsonRequestBehavior.AllowGet);
        }
        // GET: Registration
        [HttpGet]
        public ActionResult Registration()
        {
            MVCUtility utility = new MVCUtility();
            LoginViewModel viewmodel = new LoginViewModel();
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult SaveRegistration(UserRegistration UserRegistration, UserProfile UserProfile)
        {
            LoginDAL dal = new LoginDAL();
            if (UserRegistration != null && UserProfile != null)
            {

                if (!dal.IsUserExist(UserRegistration.Email.ToLower()))
                {
                        UserRegistration.Type = "Customer";
                        int status = dal.SaveRegistration(UserRegistration, UserProfile);
                    if (status != 0)
                    {
                        Session["UserProfileId"] = UserProfile.Id;
                        Session["UserRegistrationId"] = UserRegistration.Id;
                        Session["UserType"] = UserRegistration.Type;
                        Session["UserName"] = UserRegistration.UserName;
                        Session["UserEmail"] = UserRegistration.Email;
                        Session["UserFullName"] = UserProfile.FirstName + ' ' + UserProfile.MiddleName + ' ' + UserProfile.LastName;
                        if (UserProfile.CreatedDate != null)
                        {
                            Session["RegistrationDate"] = UserProfile.CreatedDate.Value.ToShortDateString();
                        }
                        else
                        {
                            Session["RegistrationDate"] = null;
                        }
                    }
                    return Json("SuccessFull", JsonRequestBehavior.AllowGet);
                }
                return Json("UserAlreadyExist", JsonRequestBehavior.AllowGet);
            }
            return Json("RegistrationFailed", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Logout(string RequestUrl)
        {
            try
            {
                Session["UserProfileId"] = null;
                Session["UserRegistrationId"] = null;
                Session["UserType"] = null;
                Session["UserName"] = null;
                Session["UserEmail"] = null;
                Session["UserFullName"] = null;
                Session["RegistrationDate"] = null;
                Session["CurrentSelectedPortalId"] = null;
                Session["CurrentSelectedPortalName"] = null;
            }
            catch
            {
                throw;
            }
            if(RequestUrl == "Stories")
            {
                return RedirectToAction("Login", new { RequestUrl = "Stories" });
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }
        [HttpGet]
        public ActionResult isUserNameExist(string userName)
        {
            bool userNameExist = true;
            if (!String.IsNullOrEmpty(userName))
            {
                LoginDAL dal = new LoginDAL();
                userNameExist = dal.IsUserExist(userName);
                if (userNameExist)
                {
                    return Json("AlreadyExist", JsonRequestBehavior.AllowGet);
                }
                return Json("NotExist", JsonRequestBehavior.AllowGet);
            }
            return Json("UserNameEmpty", JsonRequestBehavior.AllowGet);
        }
    }
}