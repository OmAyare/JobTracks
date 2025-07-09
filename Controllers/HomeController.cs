
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobTracks.Areas.Admin.Data;
using JobTracks.Models;


namespace JobTracks.Controllers
{
    public class HomeController : Controller
    {
        private JobTracksEntities db = new JobTracksEntities();
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Index(User adlo)
        //{
        //    var username = adlo.Username?.Trim().ToLower();
        //    var password = adlo.Password?.Trim();

        //    if (ModelState.IsValid)
        //    {
        //        var us = db.Users.SingleOrDefault(x => x.Username == adlo.Username && x.Password == adlo.Password);
        //        if (us != null)
        //        {
        //            Session["UserId"] = us.User_id;
        //            Session["Username"] = us.Username;
        //            Session["RoleId"] = us.Role_id;

        //            ViewBag.ShowAnimation = true;

        //            ViewBag.RoleId = us.Role_id;

        //            return View(adlo); 
        //        }
        //    }

        //    ViewBag.Error = "Invalid username or password.";
        //    return View(adlo);
        //}

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

                    ViewBag.ShowAnimation = true;
                    ViewBag.RoleId = us.Role_id;
                    // Redirect based on role
                    //switch (us.Role_id)
                    //{
                    //    case 1:
                    //        return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
                    //    case 2:
                    //        return RedirectToAction("Dashboard", "TeamLeader", new { area = "TeamLeader" });
                    //    case 3:
                    //        return RedirectToAction("Dashboard", "Recruiter", new { area = "Recruiter" });
                    //}
                    return View(adlo);
                }

                ViewBag.Error = "Invalid username or password.";
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ViewBag.Error = string.Join("<br/>", errors);
            }

            return View(adlo);
        }


    }
}