using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobTracks.Areas.Admin.Data;
using JobTracks.Areas.Recruiter.Data;

namespace JobTracks.Areas.Recruiter.Controllers
{
    public class RecruiterController : Controller
    {
        JobTracksEntities db = new JobTracksEntities();
        public ActionResult Dashboard()
        {
            int recruiterId = (int)Session["UserId"];

            var jobs = db.Job_Master
                .Where(j => j.Recruiter_Id == recruiterId)
                .Select(j => new AssignedJobViewModel
                {
                    JobId = j.Job_id,
                    Title = j.Title,
                    TechStack = j.Tech_Stack,
                    Status = j.status,
                    CompanyName = j.Company_Master.Company_Name,
                    CreatedDate = j.CreatedDate,
                    TentativeDate = j.TentativeDate
                })
                .ToList();

            return View(jobs);
        }
        public ActionResult AssignApplicants()
        {
            int recruiterId = (int)Session["UserId"];

            var data = db.Job_Applicant_Master
                .Include("Applicant")
                .Include("Job_Ref.Company_Master")
                .Where(j => j.Recuriter_ID == recruiterId)
                .Select(j => new RecruiterApplicantListViewModel
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
                })
                .ToList();

            return View(data);
        }

        // GET: Edit Applicant Status
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
        public ActionResult Add_Applicants()
        { 
              return View();
        }

        //[HttpPost]
        //public ActionResult Add_Applicants([Bind(Include = "AppLicant_id,FirstName,LastName,Last_Qualification,PassOutYear,YearOfExperience,Resume,Status")] 
        //Applicant_Master applicant ,HttpPostedFileBase ResumeFile) 
        //{

        //    if (ModelState.IsValid)
        //    {
        //        if (ResumeFile != null && ResumeFile.ContentLength > 0)
        //        {
        //            var fileName = Path.GetFileName(ResumeFile.FileName);
        //            var savePath = Path.Combine(Server.MapPath("~/Resumes"), fileName);
        //            ResumeFile.SaveAs(savePath);

        //            applicant.Resume = fileName; // store only the file name in DB
        //        }


        //        db.Applicant_Master.Add(applicant);
        //        db.SaveChanges();
        //        TempData["Success"] = "Applicant added successfully!";
        //        return RedirectToAction("View_Applcants");
        //    }
        //    return View(applicant);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        public ActionResult View_Applcants()
        {
            var applicants = db.Applicant_Master.ToList();
            return View(applicants);
        }


    }
}