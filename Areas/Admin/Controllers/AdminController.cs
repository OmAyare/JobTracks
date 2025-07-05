using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using JobTracks.Areas.Admin.Data;
using PagedList;
using PagedList.Mvc;

namespace JobTracks.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private JobTracksEntities db = new JobTracksEntities();
        // GET: Admin/Admin
        
        [Route("Admin/Dashboard")]
        public ActionResult Dashboard()
        {
            if (Session["UserId"] == null || (int)Session["RoleId"] != 1)
            {
                return RedirectToAction("Index", "Home" , new { area = "" });
            }

            // Simulate fake data if CreatedDate isn't in DB
            var jobList = db.Job_Master.ToList();

            // If you don't have CreatedDate in DB, simulate it
            var report = jobList
                 .GroupBy(j => j.CreatedDate.Month)
                 .Select(g => new {
                     Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                     Count = g.Count()
                 })
                  .ToList();
            ViewBag.Months = report.Select(x => x.Month).ToList();
            ViewBag.Counts = report.Select(x => x.Count).ToList();

            return View();

        }

        /************************************** Users ********************************************/
        #region Users
        public ActionResult User(int? page, string searchBy, string search)
        {
            var users = db.Users.AsQueryable().ToList().ToPagedList(page ?? 1, 5);
            if (searchBy == "Username")
            {
                return View(db.Users.Where(x => x.Username.StartsWith(search) || search == null).ToList().ToPagedList(page ?? 1, 5));
            }
            else
            {
                return View(db.Users.Where(x => x.Role.Name.StartsWith(search) || search == null).ToList().ToPagedList(page ?? 1, 5));
            }
            return View(users);
        }


        [HttpGet]
        [Route("Admin/User/Create")]
        public ActionResult Create()
        {
            ViewBag.RoleList = new SelectList(db.Roles, "Id", "Name"); // Provide a dropdown for selecting 
            return View();
        }

        // POST: Admin/Admin/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "Username,Email,Password,Role_id")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("user", new { Role_id = user.Role_id });
            }

            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", user.Role_id.ToString()); 

            return View(user);
        }

        // GET: Admin/Admin/Edit/5
        [HttpGet]
        [Route("Admin/User/Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id); // Find user by ID
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", user.Role_id); // Dropdown for Roles
            return View(user);
        }

        // POST: Admin/Admin/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "User_id,Username,Email,Password,Role_id")] User tblemployee)
        {

            User EmployeeFromDb = db.Users.Single(x => x.User_id == tblemployee.User_id);

            EmployeeFromDb.Username = tblemployee.Username;
            EmployeeFromDb.Role_id = tblemployee.Role_id;
            EmployeeFromDb.Password = tblemployee.Password;
            EmployeeFromDb.Email = tblemployee.Email;
            if (ModelState.IsValid)
            {
                db.Entry(EmployeeFromDb).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("User" ,new { Role_id = tblemployee.Role_id.ToString()});
            }
            return View(tblemployee);
        }

        // GET: Admin/Admin/Delete/5
        [HttpGet]
        [Route("Admin/User/Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User tblEmployee = db.Users.Find(id);
            if (tblEmployee == null)
            {
                return HttpNotFound();
            }
            return View(tblEmployee);
        }

        // POST: Admin/Admin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            User tblEmployee = db.Users.Find(id);
            db.Users.Remove(tblEmployee);
            db.SaveChanges();
            return RedirectToAction("User");
        }

        [HttpGet]
        public JsonResult IsRoleAvailable(string Name)
        {
            return Json(!db.Roles.Any(x => x.Name == Name),JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionName("Create_Role")]
        [Route("Admin/Role/Create")]
        public ActionResult Create_Role()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create_Role")]
        public ActionResult Create_Role(Role rol)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(rol);
                db.SaveChanges();
                return RedirectToAction("User");
            }
            return View(rol);
        }
        #endregion
        /************************************* Company *******************************************/
        #region Company
        [HttpGet]
        public ActionResult Company(int? page, string searchBy, string search)
        {
            if (searchBy == "TeamLeader_Id")
            {
                return View(db.Job_Master.Where(x => x.User1.Username.StartsWith(search) || search == null).ToList().ToPagedList(page ?? 1, 5));
            }
            else if (searchBy == "Recruiter_Id")
            {
                return View(db.Job_Master.Where(x => x.User.Username.StartsWith(search) || search == null).ToList().ToPagedList(page ?? 1, 5));
            }
            else if (searchBy == "Tech_Stack")
            {
                return View(db.Job_Master.Where(x => x.Tech_Stack.Contains(search) || search == null).ToList().ToPagedList(page ?? 1, 5));
            }
            else if (searchBy == "Title")
            {
                return View(db.Job_Master.Where(x => x.Title.StartsWith(search)|| search == null).ToList().ToPagedList(page ?? 1, 5));
            }
            else if (searchBy == "status")
            {
                return View(db.Job_Master.Where(x => x.status.StartsWith(search) || search == null).ToList().ToPagedList(page ?? 1, 5));
            }
            else
            {
                return View(db.Job_Master.Where(x => x.Company_Master.Company_Name.StartsWith(search) || search == null).ToList().ToPagedList(page ?? 1, 5));
            }
            var JobList = db.Job_Master.AsQueryable().ToList().ToPagedList(page ?? 1, 7);
            return View(JobList);
        }

        [HttpGet]
        public ActionResult CreateCompany() 
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCompany(Company_Master com)
        {
            if (ModelState.IsValid)
            {
                db.Company_Master.Add(com);
                db.SaveChanges();
                return RedirectToAction("Company");
            }
            return View(com);
        }

        [HttpGet]
        public ActionResult AssignWork()
        {
            ViewBag.CompanyList = new SelectList(db.Company_Master, "Company_id", "Company_Name");
            ViewBag.TLList = new SelectList(db.Users.Where(x => x.Role_id == 2), "User_id", "Username");
            ViewBag.RecruiterList = new SelectList(db.Users.Where(x => x.Role_id == 3), "User_id", "Username");
            return View();
        }

        [HttpPost]
        public ActionResult AssignWork([Bind(Include = "status,Company_Id,TeamLeader_Id,Recruiter_Id,Tech_Stack,Description,Title")] Job_Master job)
        {
            if (ModelState.IsValid)
            {
                job.CreatedDate = DateTime.Now;
                db.Job_Master.Add(job);
                db.SaveChanges();
                return RedirectToAction("Company");
            }

            ViewBag.Company_Id = new SelectList(db.Company_Master, "Company_id", "Company_Name", job.Company_Id.ToString());
            ViewBag.User_id = new SelectList(db.Users, "User_id", "Username", job.User.ToString());
            ViewBag.User_id = new SelectList(db.Users, "User_id", "Username", job.User1.ToString());
            return View(job);
        }

        // GET: Admin/Admin/Delete/5
        [HttpGet]
        [Route("Admin/Compnay/Delete")]
        public ActionResult AssignWorkDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job_Master tblAssign = db.Job_Master.Find(id);
            if (tblAssign == null)
            {
                return HttpNotFound();
            }
            return View(tblAssign);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public ActionResult AssignWorkDelete(int id)
        {
            Job_Master tblAssign = db.Job_Master.Find(id);
            db.Job_Master.Remove(tblAssign);
            db.SaveChanges();
            return RedirectToAction("Company");
        }

        // GET: Admin/Edit/5
        [HttpGet]
        [Route("Admin/Company/Edit")]
        public ActionResult AssignWorkEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Job_Master tblAssign = db.Job_Master.Find(id);
            if (tblAssign == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyList = new SelectList(db.Company_Master, "Company_id", "Company_Name");
            ViewBag.TLList = new SelectList(db.Users.Where(x => x.Role_id == 2), "User_id", "Username");
            ViewBag.RecruiterList = new SelectList(db.Users.Where(x => x.Role_id == 3), "User_id", "Username");
            return View(tblAssign);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public ActionResult AssignWorkEdit([Bind(Include = "Job_id,status,Company_Id,TeamLeader_Id,Recruiter_Id,Tech_Stack,Description,Title")] Job_Master tblAssign)
        {

            Job_Master Companyfromdb = db.Job_Master.Single(x => x.Job_id == tblAssign.Job_id);

            Companyfromdb.Title = tblAssign.Title;
            Companyfromdb.Description = tblAssign.Description;
            Companyfromdb.Tech_Stack = tblAssign.Tech_Stack;
            Companyfromdb.status = tblAssign.status;
            Companyfromdb.Company_Id = tblAssign.Company_Id;
            Companyfromdb.TeamLeader_Id = tblAssign.TeamLeader_Id;
            Companyfromdb.Recruiter_Id = tblAssign.Recruiter_Id;
            if (ModelState.IsValid)
            {
                db.Entry(Companyfromdb).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Company", new { Job_id = tblAssign.Job_id.ToString() });
            }
            return View(tblAssign);
        }

        #endregion
    }
}

