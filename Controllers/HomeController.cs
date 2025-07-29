
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using JobTracks.Areas.Admin.Data;
using JobTracks.Models;


namespace JobTracks.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            
        }

        private JobTracksEntities db = new JobTracksEntities();

        [HttpGet]
        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public ActionResult Index(Login adlo)
        {
            if (ModelState.IsValid)
            {
                var username = adlo.Username?.Trim().ToLower();
                var password = adlo.Password?.Trim();

                var us = db.Users.ToList()
                    .SingleOrDefault(x => x.Username.Trim().ToLower() == username && x.Password.Trim() == password);

                if (us != null)
                {
                    Session["UserId"] = us.User_id;
                    Session["Username"] = us.Username;
                    Session["RoleId"] = us.Role_id;

                    Session["FullName"] = us.FullName;
                    Session["UserCode"] = us.Username;       
                    Session["Email"] = us.Email;
                    if (us.Employee_Photo != null)
                    {
                        Session["PhotoPath"] = us.Employee_Photo ?? "default.jpg"; 

                    }
                    else
                    {
                        Session["PhotoBase64"] = null;
                    }
                    ViewBag.ShowAnimation = true;
                    ViewBag.RoleId = us.Role_id;
                   
                    return View(adlo);
                }

                ViewBag.Error = "Invalid username or password.";
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ViewBag.Error = string.Join("<br/>", errors);
                return View("NotFound", "Error");
            }

            return View(adlo);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear(); 
            Session.Abandon();
            return RedirectToAction("Index", "Home"); 
        }


        /**************************************  ********************************************/
        public ActionResult ViewProfile()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            var user = db.Users.Find(userId);
            return View(user);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string currentPassword, string newPassword)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            var user = db.Users.Find(userId);

            if (user.Password != currentPassword.Trim())
            {
                ViewBag.Error = "Old password does not match.";
                return View();
            }

            user.Password = newPassword.Trim();
            db.SaveChanges();
            ViewBag.Message = "Password updated successfully!";
            return View();
        }

        /**************************************  ********************************************/

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Username == model.Username && u.PhoneNumber == model.PhoneNumber);
                if (user == null)
                {
                    ModelState.AddModelError("", "Username or Phone Number is incorrect.");
                    return View(model);
                }

                string otp = new Random().Next(1000, 9999).ToString();
                TempData["OTP"] = otp;
                TempData["Username"] = model.Username;

                System.Diagnostics.Debug.WriteLine($"OTP for {model.Username}: {otp}");

                return RedirectToAction("VerifyOtp");
            }

            return View(model);
        }

    }
}