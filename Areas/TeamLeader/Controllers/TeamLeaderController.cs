using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using JobTracks.Areas.Admin.Data;
using JobTracks.Areas.TeamLeader.Data;
using JobTracks.Common;
using JobTracks.Filters;
using Newtonsoft.Json;
using PagedList;
using PagedList.Mvc;

namespace JobTracks.Areas.TeamLeader.Controllers
{
    public class TeamLeaderController : Controller
    {
        JobTracksEntities db = new JobTracksEntities();

        //Get : TeamLeader/Dashboard


        [ParitalCache("5minutescache")]
        [Route("TeamLeader/Dashboard")]
        [AuthorizeRoles(2)]
        public ActionResult Dashboard(int? selectedYear)
        {
            if (Session["UserId"] == null || (int)Session["RoleId"] != 2)
                return RedirectToAction("Index", "Home");

            int teamLeaderId = (int)Session["UserId"];
            int year = selectedYear ?? DateTime.Now.Year;

            // 1. Years
            var years = db.Job_Master
                .Where(j => j.TeamLeader_Id == teamLeaderId)
                .Select(j => j.CreatedDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            ViewBag.Years = years;
            ViewBag.SelectedYear = year;

            // 2. Month labels
            var months = Enumerable.Range(1, 12)
                .Select(m => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m))
                .ToList();
            ViewBag.Months = months;

            // 3. Jobs Posted
            var jobCounts = new int[12];
            var jobList = db.Job_Master
                .Where(j => j.TeamLeader_Id == teamLeaderId && j.CreatedDate.Year == year)
                .ToList();

            foreach (var job in jobList)
            {
                int month = job.CreatedDate.Month;
                if (month >= 1 && month <= 12)
                {
                    jobCounts[month - 1]++;
                }
            }
            ViewBag.JobCounts = jobCounts;

            // 4. Applicant Status Summary (case-insensitive + trimmed)
            var statusMap = new Dictionary<string, int[]>
    {
        { "Placed", new int[12] },
        { "Shortlisted", new int[12] },
        { "Rejected", new int[12] }
    };

            var applicantData = from ja in db.Job_Applicant_Master
                                join jm in db.Job_Master on ja.JobRef_Id equals jm.Job_id
                                where jm.TeamLeader_Id == teamLeaderId
                                      && jm.CreatedDate.Year == year
                                      && ja.Status != null
                                select new
                                {
                                    Status = ja.Status.Trim().ToLower(),
                                    Month = jm.CreatedDate.Month
                                };

            foreach (var record in applicantData)
            {
                string statusKey = null;

                if (record.Status == "placed") statusKey = "Placed";
                else if (record.Status == "shortlisted") statusKey = "Shortlisted";
                else if (record.Status == "rejected") statusKey = "Rejected";

                if (statusKey != null && record.Month >= 1 && record.Month <= 12)
                {
                    statusMap[statusKey][record.Month - 1]++;
                }
            }

            var statusDatasets = new List<object>();
            foreach (var pair in statusMap)
            {
                string color = "rgba(200, 200, 200, 0.7)";
                if (pair.Key == "Placed") color = "rgba(75, 192, 192, 0.7)";
                else if (pair.Key == "Shortlisted") color = "rgba(255, 206, 86, 0.7)";
                else if (pair.Key == "Rejected") color = "rgba(255, 99, 132, 0.7)";

                statusDatasets.Add(new
                {
                    label = pair.Key,
                    data = pair.Value,
                    backgroundColor = color
                });
            }

            ViewBag.StatusDatasets = statusDatasets;

            return View();
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
        /************************************** Company ***************************************/
        #region Company
        [HttpGet]
        [Route("TeamLeader/Company")]
        [AuthorizeRoles(2)]
        public ActionResult Company(int? page, string searchBy, string search)
        {
            var jobQuery = db.Job_Master.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                switch (searchBy)
                {
                    case "TeamLeader_Id":
                        jobQuery = jobQuery.Where(x => x.User1.Username.StartsWith(search));
                        break;
                    case "Recruiter_Id":
                        jobQuery = jobQuery.Where(x => x.User.Username.StartsWith(search));
                        break;
                    case "Tech_Stack":
                        jobQuery = jobQuery.Where(x => x.Tech_Stack.Contains(search));
                        break;
                    case "Title":
                        jobQuery = jobQuery.Where(x => x.Title.StartsWith(search));
                        break;
                    case "status":
                        jobQuery = jobQuery.Where(x => x.status.StartsWith(search));
                        break;
                    default:
                        jobQuery = jobQuery.Where(x => x.Company_Master.Company_Name.StartsWith(search));
                        break;
                }
            }

            var pagedList = jobQuery.OrderByDescending(x => x.Job_id).ToList().ToPagedList(page ?? 1, 5);
            return View(pagedList);
        }


        [HttpGet]
        [AuthorizeRoles(2)]
        public ActionResult CreateCompany()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [AuthorizeRoles(2)]
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
        [ParitalCache("10minutescache")]
        [AuthorizeRoles(2)]
        public ActionResult AssignWork()
        {
            ViewBag.CompanyList = new SelectList(db.Company_Master, "Company_id", "Company_Name");
            ViewBag.TLList = new SelectList(db.Users.Where(x => x.User_id == 2), "User_id", "Username");
            ViewBag.RecruiterList = new SelectList(db.Users.Where(x => x.User_id == 3), "User_id", "Username");
            return View();
        }

        [HttpPost]
        [AuthorizeRoles(2)]
        public ActionResult AssignWork([Bind(Include = "status,Company_Id,TeamLeader_Id,Recruiter_Id,Tech_Stack,Description,Title,TentativeDate,RequiredCount")] Job_Master job)
        {
            if (job.RequiredCount <= 0)
            {
                ModelState.AddModelError("RequiredCount", "Required count must be greater than zero.");
            }
            if (ModelState.IsValid)
            {
                job.TentativeDate = Convert.ToDateTime(Request["TentativeDate"]);
                job.CreatedDate = DateTime.Now;
                job.PlacedCount = 0;
                job.status = "Open"; 
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
        [Route("TeamLeaderr/Compnay/Delete")]
        [AuthorizeRoles(2)]
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
        [AuthorizeRoles(2)]
        public ActionResult AssignWorkDelete(int id)
        {
            Job_Master tblAssign = db.Job_Master.Find(id);
            tblAssign.status = "Archived"; // Or "Deleted" or "Inactive"
            db.Entry(tblAssign).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Company");
        }

        // GET: Admin/Edit/5
        [HttpGet]
        [Route("TeamLeaderr/Company/Edit")]
        [AuthorizeRoles(2)]
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
        [AuthorizeRoles(2)]
        public ActionResult AssignWorkEdit([Bind(Include = "Job_id,status,Company_Id,TeamLeader_Id,Recruiter_Id,Tech_Stack,Description,Title,TentativeDate,RequiredCount")] Job_Master tblAssign)
        {
            var companyFromDb = db.Job_Master.SingleOrDefault(x => x.Job_id == tblAssign.Job_id);
            if (companyFromDb == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                companyFromDb.Title = tblAssign.Title;
                companyFromDb.Description = tblAssign.Description;
                companyFromDb.Tech_Stack = tblAssign.Tech_Stack;
                companyFromDb.status = tblAssign.status;
                companyFromDb.Company_Id = tblAssign.Company_Id;
                companyFromDb.TeamLeader_Id = tblAssign.TeamLeader_Id;
                companyFromDb.Recruiter_Id = tblAssign.Recruiter_Id;
                companyFromDb.TentativeDate = tblAssign.TentativeDate?.Date;
                companyFromDb.RequiredCount = tblAssign.RequiredCount;
                if (companyFromDb.RequiredCount == 0)
                {
                    companyFromDb.status = "Open"; 
                }
                else if (companyFromDb.PlacedCount >= companyFromDb.RequiredCount)
                {
                    companyFromDb.status = "Close";
                }
                else if (companyFromDb.PlacedCount > 0)
                {
                    companyFromDb.status = "In Progress";
                }
                else
                {
                    companyFromDb.status = "Open";
                }

                db.Entry(companyFromDb).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Company");
            }

            ViewBag.CompanyList = new SelectList(db.Company_Master, "Company_id", "Company_Name", tblAssign.Company_Id);
            ViewBag.TLList = new SelectList(db.Users.Where(x => x.Role_id == 2), "User_id", "Username", tblAssign.TeamLeader_Id);
            ViewBag.RecruiterList = new SelectList(db.Users.Where(x => x.Role_id == 3), "User_id", "Username", tblAssign.Recruiter_Id);

            return View(tblAssign);
        }

        [AuthorizeRoles(2)]
        public ActionResult RecruiterWorkDetail(int? recruiterId, int? companyId, int? page, string searchBy, string search)
        {
            int teamLeaderId = (int)Session["UserId"];

            var query = db.Job_Applicant_Master
                .Where(j => j.JobRef_Id != null &&
                            j.Job_Master.TeamLeader_Id == teamLeaderId);

            // Apply filter based on searchBy
            if (!string.IsNullOrEmpty(search))
            {
                switch (searchBy)
                {
                    case "Job Title":
                        query = query.Where(j => j.Job_Master.Title.Contains(search));
                        break;
                    case "Company":
                        query = query.Where(j => j.Job_Master.Company_Master.Company_Name.Contains(search));
                        break;
                    case "Status":
                        query = query.Where(j => j.Status.Contains(search));
                        break;
                }
            }

            var report = query.Select(j => new RecruiterSummaryViewModel
            {
                RecruiterName = j.User.Username,
                ApplicantName = j.Applicant_Master.FirstName + " " + j.Applicant_Master.LastName,
                JobTitle = j.Job_Master.Title,
                CompanyName = j.Job_Master.Company_Master.Company_Name,
                Status = j.Status,
                AssignedDate = j.Job_Master.TentativeDate
            });

            var pagedResult = report.ToList().ToPagedList(page ?? 1, 5);

            return View(pagedResult);
        }

        #endregion


    }
}