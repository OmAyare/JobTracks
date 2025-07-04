
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobTracks.Areas.Admin.Data;


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
        public ActionResult Index(User adlo)
        {
            if (ModelState.IsValid)
            {
                var us = db.Users.SingleOrDefault(x => x.Username == adlo.Username && x.Password == adlo.Password);
                if (us != null)
                {
                    Session["UserId"] = us.User_id;
                    Session["Username"] = us.Username;
                    Session["RoleId"] = us.Role_id;

                    ViewBag.ShowAnimation = true;

                    ViewBag.RoleId = us.Role_id;

                    return View(adlo); 
                }
            }

            ViewBag.Error = "Invalid username or password.";
            return View(adlo);
        }


    }
}