using System.Web.Mvc;

namespace JobTracks.Areas.Recruiter
{
    public class RecruiterAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Recruiter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
            "Recruiter_ViewProfile",
            "Recruiter/Profile",
            new { controller = "Recruiter", action = "ViewProfile" }
            );

            context.MapRoute(
                 "Recruiter_ChangePassword",
                 "Recruiter/ChangePassword",
                 new { controller = "Recruiter", action = "ChangePassword" }
             );

            context.MapRoute(
            name: "Recruiter_Dashboard",
            url: "Recruiter/Dashboard",
            defaults: new { controller = "Recruiter", action = "Dashboard" }
            );

            context.MapRoute(
            name: "Recruiter_AssignApplicants",
            url: "Recruiter/AssignApplicants",
            defaults: new { controller = "Recruiter", action = "AssignApplicants" }
            );

            context.MapRoute(
            name: "Recruiter_EditApplicantStatus",
            url: "Recruiter/AssignApplicants/Edit",
            defaults: new { controller = "Recruiter", action = "EditApplicantStatus" }
            );

            context.MapRoute(
                name: "Recruiter_Deletes",
            url: "Recruiter/Add_Applicants/Delete",
            defaults: new { controller = "Recruiter", action = "Delete" }
            );

            context.MapRoute(
            name: "Recruiter_Add_Applicants",
             url: "Recruiter/View_Applcants/Add_Applicants",
             defaults: new { controller = "Recruiter", action = "Add_Applicants" }
           );

            context.MapRoute(
            name: "Recruiter_View_Applcants",
            url: "Recruiter/View_Applcants",
            defaults: new { controller = "Recruiter", action = "View_Applcants" }
            );

            context.MapRoute(
                "Recruiter_default",
                "Recruiter/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}