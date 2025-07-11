
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
    }
}