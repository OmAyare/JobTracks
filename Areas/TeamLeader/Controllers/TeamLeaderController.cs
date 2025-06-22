using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JobTracks.Areas.Admin.Data;

namespace JobTracks.Areas.TeamLeader.Controllers
{
    public class TeamLeaderController : Controller
    {
        JobTracksEntities db = new JobTracksEntities();

        //Get : TeamLeader/Dashboard

        [Route("TeamLeader/Dashboard")]
        public ActionResult Dashboard()
        {
           // int tlId = (int)Session["UserId"];
            if (Session["UserId"] == null || (int)Session["RoleId"] != 2)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            int tlId = (int)Session["UserId"];

            //// Job count per month (raw SQL-safe)
            //var jobData = db.Job_Master
            //    .Where(j => j.TeamLeader_Id == tlId)
            //    .GroupBy(j => j.CreatedDate.Month)
            //    .Select(g => new {
            //        MonthNum = g.Key, // this is int like 1, 2, 3
            //        Count = g.Count()
            //    })
            //    .ToList() // switch to LINQ-to-Objects (safe)
            //    .Select(x => new {
            //        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.MonthNum),
            //        Count = x.Count
            //    })
            //    .ToList();

            //ViewBag.Months = jobData.Select(x => x.Month).ToList();
            //ViewBag.JobCounts = jobData.Select(x => x.Count).ToList();
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


           var applicantData = (from ja in db.Job_Applicant_Master
                     join u in db.Users on ja.Recuriter_ID equals u.User_id
                     group ja by u.Username into g
                     select new {
                         Recruiter = g.Key,
                         Count = g.Count()
                     }).ToList();


            ViewBag.Recruiters = applicantData.Select(x => x.Recruiter).ToList();
            ViewBag.ApplicantCounts = applicantData.Select(x => x.Count).ToList();




            return View();

        }

        /************************************** Company ***************************************/
        #region Company
        [HttpGet]
        [Route("TeamLeaderr/Compnay")]
        public ActionResult Company()
        {
            var JobList = db.Job_Master.ToList();
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
            ViewBag.TLList = new SelectList(db.Users.Where(x => x.User_id == 2), "User_id", "Username");
            ViewBag.RecruiterList = new SelectList(db.Users.Where(x => x.User_id == 3), "User_id", "Username");
            return View();
        }

        [HttpPost]
        public ActionResult AssignWork([Bind(Include = "status,Company_Id,TeamLeader_Id,Recruiter_Id,Tech_Stack,Description,Title,TentativeDate")] Job_Master job)
        {
            if (ModelState.IsValid)
            {
                job.TentativeDate = Convert.ToDateTime(Request["TentativeDate"]);
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
        [Route("TeamLeaderr/Compnay/Delete")]
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
        [Route("TeamLeaderr/Company/Edit")]
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
            ViewBag.TLList = new SelectList(db.Users.Where(x => x.User_id == 2), "User_id", "Username");
            ViewBag.RecruiterList = new SelectList(db.Users.Where(x => x.User_id == 3), "User_id", "Username");
            return View(tblAssign);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public ActionResult AssignWorkEdit([Bind(Include = "Job_id,status,Company_Id,TeamLeader_Id,Recruiter_Id,Tech_Stack,Description,Title,TentativeDate")] Job_Master tblAssign)
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