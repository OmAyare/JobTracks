using JobTracks.Areas.Admin.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace JobTracks.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private JobTracksEntities db = new JobTracksEntities();
        // GET: Admin/Admin

        [Route("Admin/Dashboard")]
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult User()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        public ActionResult Company()
        {
          var company = db.Company_Master.ToList();
            return View(company);
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

            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", user.Role_id.ToString()); // Preserve role dropdown

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
                return RedirectToAction("Index" ,new { Role_id = tblemployee.Role_id.ToString()});
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
            return RedirectToAction("Index");
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
    }
}

