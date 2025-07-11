using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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


        [ParitalCache("5minutescache")]
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
        [AuthorizeRoles(3)]
        public ActionResult AssignApplicants(int? page, string searchBy, string search)
        {
            int recruiterId = (int)Session["UserId"];

            var query = db.Job_Applicant_Master
                .Include("Applicant_Master")
                .Include("Job_Ref.Company_Master")
                .Where(j => j.Recuriter_ID == recruiterId);

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

            var report = query.Select(j => new RecruiterApplicantListViewModel
            {
                JobApplicantId = j.Job_Id,
                ApplicantFullName = j.Applicant_Master.FirstName + " " + j.Applicant_Master.LastName,
                LastQualification = j.Applicant_Master.Last_Qualification,
                JobTitle = j.Job_Master.Title,
                CompanyName = j.Job_Master.Company_Master.Company_Name,
                TentativeDate = j.Job_Master.TentativeDate,
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
                CurrentJobTitle = record.Job_Master.Title,
                SelectedJobId = record.JobRef_Id ?? 0,
                SelectedStatus = record.Status,

                JobOptions = db.Job_Master
                    .Where(j => j.Recruiter_Id == recruiterId)
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
                switch (model.SelectedStatus)
                {
                    case "Placed":
                        job.status = "Close";
                        break;
                    case "Rejected":
                        job.status = "Open";
                        break;
                    case "Shortlisted":
                    case "Interview Scheduled":
                        job.status = "In Progress";
                        break;
                }
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

        [ParitalCache("5minutescache")]
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


    }
}