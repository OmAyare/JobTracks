using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using JobTracks.Areas.Admin.Data;
using JobTracks.Areas.Recruiter.Data;
using JobTracks.Common;
using JobTracks.Filters;
using PagedList;
using PagedList.Mvc;

namespace JobTracks.Areas.Recruiter.Controllers
{
    public class RecruiterController : Controller
    {
        JobTracksEntities db = new JobTracksEntities();


        [AuthorizeRoles(3)]
        public ActionResult Dashboard(int? page, string searchBy, string search)
        {
            int recruiterId = (int)Session["UserId"];

            var query = db.Job_Master
                .Where(j => j.Recruiter_Id == recruiterId);

            if (!string.IsNullOrEmpty(search))
            {
                switch (searchBy)
                {
                    case "Job Title":
                        query = query.Where(j => j.Title.Contains(search));
                        break;
                    case "Company":
                        query = query.Where(j => j.Company_Master.Company_Name.Contains(search));
                        break;
                    case "Tech Stack":
                        query = query.Where(j => j.Tech_Stack.Contains(search));
                        break;
                    case "Status":
                        query = query.Where(j => j.status.Contains(search));
                        break;
                }
            }

            var report = query.Select(j => new AssignedJobViewModel
            {
                JobId = j.Job_id,
                Title = j.Title,
                TechStack = j.Tech_Stack,
                Status = j.status,
                CompanyName = j.Company_Master.Company_Name,
                CreatedDate = j.CreatedDate,
                TentativeDate = j.TentativeDate
            });

            var pagedResult = report.ToList().ToPagedList(page ?? 1, 5);
            return View(pagedResult);
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

        [AuthorizeRoles(3)]
        public ActionResult AssignApplicants(int? page, string searchBy, string search)
        {
            int recruiterId = (int)Session["UserId"];

            var query = db.Job_Applicant_Master
                .Include("Applicant_Master")
                .Include("Job_Master.Company_Master") // Include "Job_Master", not "Job_Ref"
                .Where(j => j.Recuriter_ID == recruiterId &&
                            j.Job_Master.status != "Archived" &&
                            j.Job_Master.status != "Close");

            if (!string.IsNullOrEmpty(search))
            {
                switch (searchBy)
                {
                    case "Qualification":
                        query = query.Where(j => j.Applicant_Master.Last_Qualification.Contains(search));
                        break;
                    case "Company":
                        query = query.Where(j => j.Job_Master.Company_Master.Company_Name.Contains(search));
                        break;
                    case "Status":
                        query = query.Where(j => j.Status.Contains(search));
                        break;
                }
            }

            // Move projection to in-memory after .ToList()
            var rawList = query.ToList(); // Materialize query first

            var report = rawList.Select(j => new RecruiterApplicantListViewModel
            {
                JobApplicantId = j.Job_Id,
                ApplicantFullName = j.Applicant_Master.FirstName + " " + j.Applicant_Master.LastName,
                LastQualification = j.Applicant_Master.Last_Qualification,
                 JobTitle = j.Job_Master != null ? j.Job_Master.Title : "-",
                CompanyName = j.Job_Master?.Company_Master?.Company_Name ?? "-",
                TentativeDate = j.Job_Master?.TentativeDate,
                ApplicantStatus = j.Status,
                MappedJobStatus =
                        j.Status == "Placed" ? "Close" :
                        j.Status == "Rejected" ? "Open" :
                        string.IsNullOrEmpty(j.Status) ? "Pending" :
                        "In Progress"
            });

            var pagedResult = report.ToList().ToPagedList(page ?? 1, 5);
            return View(pagedResult);
        }

        // GET: Edit Applicant Status
        [AuthorizeRoles(3)]
        public ActionResult EditApplicantStatus(int id)
        {
            int recruiterId = (int)Session["UserId"];

            var record = db.Job_Applicant_Master
                .Include("Applicant_Master")
                .Include("Job_Master.Company_Master")
                .FirstOrDefault(x => x.Job_Id == id);


            if (record == null) return HttpNotFound();

            var model = new EditApplicantStatusViewModel
            {
                JobApplicantId = record.Job_Id,
                ApplicantName = record.Applicant_Master.FirstName + " " + record.Applicant_Master.LastName,
                CurrentJobTitle = record.Job_Master?.Title ?? "Not Assigned",
                SelectedJobId = record.JobRef_Id ?? 0,
                SelectedStatus = record.Status,
                PlacedCount = record.Job_Master?.PlacedCount ?? 0,
                RequiredCount = record.Job_Master?.RequiredCount ?? 0,

                JobOptions = db.Job_Master
                    .Where(j => j.Recruiter_Id == recruiterId && j.status != "Close" && j.status != "Archived")
                    .Select(j => new SelectListItem
                    {
                        Value = j.Job_id.ToString(),
                        Text = j.Title + " - " + j.Company_Master.Company_Name
                    }).ToList(),

                StatusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Placed", Value = "Placed" },
                new SelectListItem { Text = "Shortlisted", Value = "Shortlisted" },
                new SelectListItem { Text = "Rejected", Value = "Rejected" },
                new SelectListItem { Text = "Interview Scheduled", Value = "Interview Scheduled" }
            }
             
            };

            return View(model);
        }

        // POST: Edit Applicant Status
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(3)]
        public ActionResult EditApplicantStatus(EditApplicantStatusViewModel model)
        {
            int recruiterId = (int)Session["UserId"]; 

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var applicantRecord = db.Job_Applicant_Master
                .FirstOrDefault(x => x.Job_Id == model.JobApplicantId);

            if (applicantRecord == null)
                return HttpNotFound();

            // Update applicant status and job reference
            applicantRecord.Status = model.SelectedStatus;
            applicantRecord.JobRef_Id = model.SelectedJobId;

            // Now update Job_Master.status based on new status
            var job = db.Job_Master.FirstOrDefault(j => j.Job_id == model.SelectedJobId);
            if (job != null)
            {
                if (model.SelectedStatus == "Placed")
                {
                    if (job.PlacedCount >= job.RequiredCount)
                    {
                        ModelState.AddModelError("", "Cannot mark as Placed. Required count has already been fulfilled.");
                        model.StatusOptions = GetStatusOptions(); // helper method
                        model.JobOptions = GetJobOptions(recruiterId); // helper method
                        return View(model);
                    }

                    job.PlacedCount++;
                    job.status = job.PlacedCount >= job.RequiredCount ? "Close" : "In Progress";
                }
                else if (model.SelectedStatus == "Rejected")
                {
                    job.PlacedCount--;
                    job.status = (job.PlacedCount < job.RequiredCount && job.PlacedCount > 0) ? "In Progress" : "Open";
                }
            }
            applicantRecord.Status = model.SelectedStatus;
            applicantRecord.JobRef_Id = model.SelectedJobId;

            var applicant = db.Applicant_Master.FirstOrDefault(a => a.AppLicant_id == applicantRecord.Applicant_ID);
            if (applicant != null)
            {
                applicant.Status = model.SelectedStatus;
            }

            db.SaveChanges();

            TempData["Success"] = "Applicant status and job updated successfully!";
            return RedirectToAction("AssignApplicants");
        }

        [HttpGet]
        [AuthorizeRoles(3)]
        public ActionResult Add_Applicants()
        { 
              return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AuthorizeRoles(3)]
        public ActionResult Add_Applicants(Applicant_Master applicant, HttpPostedFileBase ResumeFile)
        {
            if (ModelState.IsValid)
            {
                // Save PDF
                if (ResumeFile != null && ResumeFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ResumeFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Resumes"), fileName);
                    ResumeFile.SaveAs(path);
                    applicant.Resume = fileName;
                }

                // Save applicant first
                db.Applicant_Master.Add(applicant);
                db.SaveChanges();

                // Get newly added applicant's ID
                int applicantId = applicant.AppLicant_id;

                // Get recruiter ID from session
                int recruiterId = (int)Session["UserId"];

                // Create Job_Applicant_Master entry (empty status, JobRefId null for now)
                Job_Applicant_Master jobApplicant = new Job_Applicant_Master
                {
                    Applicant_ID = applicantId,
                    Recuriter_ID = recruiterId,
                    Status = null,
                    JobRef_Id = null // will be selected in Assign view
                };

                db.Job_Applicant_Master.Add(jobApplicant);
                db.SaveChanges();

                TempData["Success"] = "Applicant added and assigned successfully!";
                return RedirectToAction("View_Applcants"); // or your appropriate list view
            }

            return View(applicant);
        }

        [AuthorizeRoles(3)]
        public ActionResult View_Applcants(int? page,string searchBy, string search)
        {
            if( searchBy == "Status")
            {
                return View(db.Applicant_Master.Where(x => x.Status.StartsWith(search) || search == null).ToList().ToPagedList(page ?? 1, 5));

            }
            else if( searchBy == "Last_Qualification")
            {
                return View(db.Applicant_Master.Where(x => x.Last_Qualification.StartsWith(search) || search == null).ToList().ToPagedList(page ?? 1, 5));
            }
            else if (searchBy == "PassOutYear")
            {
                if (int.TryParse(search, out int passOutYear))
                {
                    return View(db.Applicant_Master.Where(x => x.PassOutYear == passOutYear || search == null).ToList().ToPagedList(page ?? 1, 5));
                }
            }
            else if (searchBy == "YearOfExperience")
            {
                if (int.TryParse(search, out int experience))
                {
                    return View(db.Applicant_Master.Where(x => x.YearOfExperience == experience || search == null).ToList().ToPagedList(page ?? 1, 5));
                }
            }
            else 
            {
                return View(db.Applicant_Master.Where(x => x.FirstName.StartsWith(search) || search == null).ToList().ToPagedList(page ?? 1, 5));
            }
            var applicants = db.Applicant_Master.AsQueryable().ToList().ToPagedList(page ?? 1, 5);
            return View(applicants);
        }

        [HttpGet]
        [AuthorizeRoles(3)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Applicant_Master applicant = db.Applicant_Master.Find(id);
            if (applicant == null)
            {
                return HttpNotFound();
            }
            return View(applicant);
        }

        [HttpPost]
        [AuthorizeRoles(3)]
        public ActionResult Delete(int id)
        {
            Applicant_Master applicant = db.Applicant_Master.Find(id);
            db.Applicant_Master.Remove(applicant);
            db.SaveChanges();
            return RedirectToAction("View_Applcants");
        }

        private List<SelectListItem> GetStatusOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Placed", Value = "Placed" },
                new SelectListItem { Text = "Shortlisted", Value = "Shortlisted" },
                new SelectListItem { Text = "Rejected", Value = "Rejected" },
                new SelectListItem { Text = "Interview Scheduled", Value = "Interview Scheduled" }
            };
        }

        private List<SelectListItem> GetJobOptions(int recruiterId)
        {
            return db.Job_Master
                .Where(j => j.Recruiter_Id == recruiterId && j.status != "Close" && j.status != "Archived")
                .Select(j => new SelectListItem
                {
                    Value = j.Job_id.ToString(),
                    Text = j.Title + " - " + j.Company_Master.Company_Name
                }).ToList();
        }


    }
}