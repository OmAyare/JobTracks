using System.Web.Mvc;

namespace JobTracks.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                  "CreateUser",
                  "Admin/User/Create",
                  new { controller = "Admin", action = "Create" }
            );

            context.MapRoute(
                "EditUser",
                "Admin/User/Edit",
                new { controller = "Admin", action = "Edit" }
          );

            context.MapRoute(
                "deleteUser",
                "Admin/User/Delete",
                new { controller = "Admin", action = "Delete" }
          );

            context.MapRoute(
                 "CreateRole",
                 "Admin/Role/Create",
                 new { controller = "Admin", action = "Create" }
           );


            context.MapRoute(
                "Admin_default",
                "{controller}/{action}/{id}",
                new { action = "Dashboard", id = UrlParameter.Optional }
            );

            

        }
    }
}