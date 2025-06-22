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
                "TeamLeader_default",
                "TeamLeader/{controller}/{action}/{id}",
                new { action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}