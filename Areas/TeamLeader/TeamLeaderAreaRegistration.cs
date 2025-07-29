using System.Web.Mvc;

namespace JobTracks.Areas.TeamLeader
{
    public class TeamLeaderAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TeamLeader";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
              "TeamLeader_ViewProfile",
              "TeamLeader/Profile",
              new { controller = "TeamLeader", action = "ViewProfile" }
        );
            context.MapRoute(
                 "TeamLeader_ChangePassword",
                 "TeamLeader/ChangePassword",
                 new { controller = "TeamLeader", action = "ChangePassword" }
           );
            context.MapRoute(
                 "TeamLeader_AssignWorkDelete",
                 "TeamLeader/Company/Delete",
                 new { controller = "TeamLeader", action = "AssignWorkDelete" }
           );
            context.MapRoute(
                 "TeamLeader_AssignWorkEdit",
                 "TeamLeader/Company/Edit",
                 new { controller = "TeamLeader", action = "AssignWorkEdit" }
           );

            context.MapRoute(
              "TeamLeader_RecruiterWorkDetail",
              "TeamLeader/Recruter_Work",
              new { controller = "TeamLeader", action = "RecruiterWorkDetail" }
        );

            context.MapRoute(
              "TeamLeader_Company",
              "TeamLeader/Company",
              new { controller = "TeamLeader", action = "Company" }
            );

            context.MapRoute(
            "TeamLeader_CreateCompany",
            "TeamLeader/Company/Create",
            new { controller = "TeamLeader", action = "CreateCompany" }
           );

            context.MapRoute(
               "TeamLeader_AssignWork",
               "TeamLeader/Company/AssignWork",
               new { controller = "TeamLeader", action = "AssignWork" }
         );

            context.MapRoute(
            name: "TeamLeader_Dashboard",
            url: "TeamLeader/Dashboard",
            defaults: new { controller = "TeamLeader", action = "Dashboard" }
           );

            context.MapRoute(
                "TeamLeader_default",
                "TeamLeader/{controller}/{action}/{id}",
                new { action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}