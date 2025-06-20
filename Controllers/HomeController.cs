
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
        // GET: Home
        [HttpPost]
        public ActionResult Index(User adlo)
        {

            if (ModelState.IsValid)
            {
                var us = db.Users.Where(x => x.Username == adlo.Username && x.Password == adlo.Password).SingleOrDefault();
                Session["UserId"] = us.User_id;
                Session["Username"] = us.Username;
                Session["RoleId"] = us.Role_id;

                if (us.Role_id == 1)
                    return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
                else if (us.Role_id == 2)
                    return RedirectToAction("Dashboard", "TeamLeader", new { area = "TeamLeader" }); 
                else if (us.Role_id == 3)
                    return RedirectToAction("Dashboard", "Recruiter", new { area = "Recruiter" }); 
            }


            ViewBag.Error = "Invalid username or password.";
            return View(adlo);
        }
    }
}