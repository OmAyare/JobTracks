﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using JobTracks.Areas.Admin.Data;
using JobTracks.Common;
using JobTracks.Filters;
using PagedList;
using PagedList.Mvc;

namespace JobTracks.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private JobTracksEntities db = new JobTracksEntities();
        // GET: Admin/Admin

        [Route("Admin/Dashboard")]
        public ActionResult Dashboard(int? selectedYear)
        {
            if (Session["UserId"] == null || (int)Session["RoleId"] != 1)
                return RedirectToAction("Index", "Home");

            int currentYear = DateTime.Now.Year;
            int year = selectedYear ?? currentYear;

            // Fetch records for the selected year
            var jobs = db.Job_Master
                .Where(j => j.CreatedDate.Year == year)
                .ToList();

            // Get all years with job postings
            var years = db.Job_Master
                .Select(j => j.CreatedDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            // Prepare 12 months
            var months = Enumerable.Range(1, 12)
                .Select(m => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m))
                .ToList();

            // Prepare counts with zeros for all months
            var counts = new int[12];

            foreach (var job in jobs)
            {
                int monthIndex = job.CreatedDate.Month - 1;
                counts[monthIndex]++;
            }

            ViewBag.Months = months;
            ViewBag.Counts = counts;
            ViewBag.Years = years;
            ViewBag.SelectedYear = year;

            return View();
        }
        /************************************** Users ********************************************/
        #region Users
        [AuthorizeRoles(1)] // 1 = Admin
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
        [AuthorizeRoles(1)] // 1 = Admin
        public ActionResult Create()
        {
            ViewBag.RoleList = new SelectList(db.Roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(1)]
        public ActionResult Create(User user, HttpPostedFileBase UploadPhoto)
        {
            if (db.Users.Any(x => x.Username == user.Username))
                ModelState.AddModelError("Username", "This username is already in use");

            if (db.Users.Any(x => x.Email == user.Email))
                ModelState.AddModelError("Email", "This email is already in use");

            if (ModelState.IsValid)
            {
                if (UploadPhoto != null && UploadPhoto.ContentLength > 0)
                {
                    string[] allowedExt = { ".jpg", ".jpeg", ".png", ".gif" };
                    string ext = Path.GetExtension(UploadPhoto.FileName).ToLower();

                    if (!allowedExt.Contains(ext))
                    {
                        ModelState.AddModelError("UploadPhoto", "Only JPG, PNG, and GIF formats are allowed.");
                        ViewBag.RoleList = new SelectList(db.Roles, "Id", "Name", user.Role_id);
                        return View(user);
                    }

                    if (UploadPhoto.ContentLength > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("UploadPhoto", "File size must be under 2MB.");
                        ViewBag.RoleList = new SelectList(db.Roles, "Id", "Name", user.Role_id);
                        return View(user);
                    }

                    string filename = Guid.NewGuid().ToString() + ext;
                    string path = Path.Combine(Server.MapPath("~/EmployeePhotos"), filename);
                    Directory.CreateDirectory(Path.GetDirectoryName(path)); 
                    UploadPhoto.SaveAs(path);

                    user.Employee_Photo = filename;
                }
                else
                {
                    user.Employee_Photo = "default.jpg"; 
                }

                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("User", new { Role_id = user.Role_id });
            }

            ViewBag.RoleList = new SelectList(db.Roles, "Id", "Name", user.Role_id);
            return View(user);
        }

        public ActionResult ViewProfile()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            var user = db.Users.Find(userId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewProfile(User model)
        {
            var user = db.Users.Find(model.User_id);

            if (user != null)
            {
                // Update only allowed fields
                user.FullName = model.FullName;
                user.FatherName = model.FatherName;
                user.MotherName = model.MotherName;
                user.Gender = model.Gender;
                user.DateOfBirth = model.DateOfBirth;
                user.PhoneNumber = model.PhoneNumber;
                user.AadharNumber = model.AadharNumber;
                user.BloodGroup = model.BloodGroup;
                user.UANNumber = model.UANNumber;

                db.SaveChanges();
                ViewBag.Message = "Profile updated successfully!";
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                ViewBag.Error = "User not found!";
            }

            return View("ViewProfile", user);
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
            return RedirectToAction("Dashboard", "Admin");
        }

        [HttpGet]
        [Route("Admin/User/Edit")]
        [AuthorizeRoles(1)] // 1 = Admin
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            ViewBag.RoleList = new SelectList(db.Roles, "Id", "Name");
            return View(user);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [AuthorizeRoles(1)] // 1 = Admin
        public ActionResult Edit([Bind(Include = "User_id,Username,Email,PhoneNumber,Role_id,FullName,FatherName,MotherName,Gender,DateOfBirth,JoiningDate,Branch,AadharNumber,UANNumber,BloodGroup,BankAccount_1,BankAccount_2,Employee_Photo,Password")] User tblemployee, HttpPostedFileBase UploadPhoto)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Error: " + error.ErrorMessage);
                }

                ViewBag.RoleList = new SelectList(db.Roles, "Id", "Name", tblemployee.Role_id);
                return View(tblemployee);
            }

            var userInDb = db.Users.Find(tblemployee.User_id);
            if (userInDb == null)
                return HttpNotFound();

            // Update all properties
            userInDb.Username = tblemployee.Username;
            userInDb.Email = tblemployee.Email;
            userInDb.PhoneNumber = tblemployee.PhoneNumber;
            userInDb.Role_id = tblemployee.Role_id;
            userInDb.FullName = tblemployee.FullName;
            userInDb.FatherName = tblemployee.FatherName;
            userInDb.MotherName = tblemployee.MotherName;
            userInDb.Gender = tblemployee.Gender;
            userInDb.Password = tblemployee.Password;
            userInDb.DateOfBirth = tblemployee.DateOfBirth;
            userInDb.JoiningDate = tblemployee.JoiningDate;
            userInDb.Branch = tblemployee.Branch;
            userInDb.AadharNumber = tblemployee.AadharNumber;
            userInDb.UANNumber = tblemployee.UANNumber;
            userInDb.BloodGroup = tblemployee.BloodGroup;
            userInDb.BankAccount_1 = tblemployee.BankAccount_1;
            userInDb.BankAccount_2 = tblemployee.BankAccount_2;
            if (UploadPhoto != null && UploadPhoto.ContentLength > 0)
            {
                string[] allowedExt = { ".jpg", ".jpeg", ".png", ".gif" };
                string ext = Path.GetExtension(UploadPhoto.FileName).ToLower();

                if (!allowedExt.Contains(ext))
                {
                    ModelState.AddModelError("UploadPhoto", "Only JPG, PNG, and GIF formats are allowed.");
                    ViewBag.RoleList = new SelectList(db.Roles, "Id", "Name", tblemployee.Role_id);
                    return View(tblemployee);
                }

                if (UploadPhoto.ContentLength > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("UploadPhoto", "File size must be under 2MB.");
                    ViewBag.RoleList = new SelectList(db.Roles, "Id", "Name", tblemployee.Role_id);
                    return View(tblemployee);
                }

                string filename = Guid.NewGuid().ToString() + ext;
                string path = Path.Combine(Server.MapPath("~/EmployeePhotos"), filename);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                UploadPhoto.SaveAs(path);

                // Set new filename in the DB
                userInDb.Employee_Photo = filename;
            }

            if (ModelState.IsValid)
            {
                db.Entry(userInDb).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("user", new { Role_id = tblemployee.Role_id });
            }
            ViewBag.RoleList = new SelectList(db.Roles, "Id", "Name", tblemployee.Role_id);
            return View(tblemployee);
        }

        [HttpGet]
        [Route("Admin/User/Delete")]
        [AuthorizeRoles(1)] // 1 = Admin
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
        [AuthorizeRoles(1)] // 1 = Admin
        public ActionResult Delete(int id)
        {
            User tblEmployee = db.Users.Find(id);
            db.Users.Remove(tblEmployee);
            db.SaveChanges();
            return RedirectToAction("User");
        }

        [HttpGet]
        [ActionName("Create_Role")]
        [Route("Admin/Role/Create")]
        [AuthorizeRoles(1)] // 1 = Admin
        public ActionResult Create_Role()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create_Role")]
        [AuthorizeRoles(1)] // 1 = Admin
        public ActionResult Create_Role(Role rol)
        {
            if (db.Roles.Any(x => x.Name == rol.Name))
            {
                ModelState.AddModelError("Name", "This role is already in use");
            }
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
        [ValidateInput(false)]
        [AuthorizeRoles(1)] // 1 = Admin
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
        [AuthorizeRoles(1)] // 1 = Admin
        public ActionResult CreateCompany() 
        {
            return View();
        }

        [HttpPost]
        [AuthorizeRoles(1)] // 1 = Admin
        public ActionResult CreateCompany(Company_Master com)
        {
            if (db.Company_Master.Any(x => x.Company_Name == com.Company_Name))
            {
                ModelState.AddModelError("Company_Name", "This Company Name is already in use");
            }
            if (ModelState.IsValid)
            {
                db.Company_Master.Add(com);
                db.SaveChanges();
                return RedirectToAction("Company");
            }
            return View(com);
        }

        [HttpGet]
        [AuthorizeRoles(1)] // 1 = Admin
        public ActionResult AssignWork()
        {
            ViewBag.CompanyList = new SelectList(db.Company_Master, "Company_id", "Company_Name");
            ViewBag.TLList = new SelectList(db.Users.Where(x => x.Role_id == 2), "User_id", "Username");
            ViewBag.RecruiterList = new SelectList(db.Users.Where(x => x.Role_id == 3), "User_id", "Username");
            return View();
        }

        [HttpPost]
        [AuthorizeRoles(1)] // 1 = Admin
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
        [AuthorizeRoles(1)] // 1 = Admin
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
        [AuthorizeRoles(1)] // 1 = Admin
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
        [AuthorizeRoles(1)] // 1 = Admin
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
        [AuthorizeRoles(1)] // 1 = Admin
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
        /************************************* Validation *******************************************/
        [HttpGet]
        [AuthorizeRoles(1)] // 1 = Admin
        public JsonResult IsRoleAvailable(string Name)
        {
            return Json(!db.Roles.Any(x => x.Name == Name), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AuthorizeRoles(1)] // 1 = Admin
        public JsonResult IsUsernameAvailable(string Username)
        {
            return Json(!db.Users.Any(u => u.Username == Username), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AuthorizeRoles(1)] // 1 = Admin
        public JsonResult IsEmailAvailable(string Email)
        {
            return Json(!db.Users.Any(u => u.Email == Email), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AuthorizeRoles(1)] // 1 = Admin
        public JsonResult IsCompanynameAvailable(string Company_Name)
        {
            return Json(!db.Company_Master.Any(u => u.Company_Name == Company_Name), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

    }
}

