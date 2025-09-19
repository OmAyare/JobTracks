# Job Tracks

Job Tracks is a web-based application built using **ASP.NET MVC 5** that manages recruitment workflows. It allows **Admins** to manage users and companies, assign work, and track recruiter activity. The system includes role-based access for Admins, Team Leaders, and Recruiters.

---

## 🔍 Features

- 👤 User Management: Create, edit, delete users with roles (Admin, Team Leader, Recruiter)
- 🏢 Company Management: Add and manage company details
- 📋 Vacancy Assignment: Assign job posts to Team Leaders and Recruiters
- 🔐 Secure Login System: Role-based redirection after login
- 📂 Profile and Password Management
- 📸 Upload and display user profile images
- 📊 Dashboard with visual segregation based on role

---

## 🚀 How to Run the Project

### 1. 🧱 Prerequisites

- Visual Studio 2019 or later
- SQL Server 2016 or later
- .NET Framework 4.7.2
- NuGet Package Restore enabled

---

### 2. 📥 Clone the Repository

<connectionStrings>
  <add name="DefaultConnection" connectionString="Data Source=.;Initial Catalog=JobTracks;Integrated Security=True" providerName="System.Data.SqlClient"/>
</connectionStrings>

5. ▶️ Run the Application
     .Press F5 or click Start Debugging in Visual Studio.
     .The default route will redirect you to the login page.

👤 Demo Users (Preloaded)

| Role       | Username | Password |
| ---------- | -------- | -------- |
| Admin      | admin    | 123      |
| TeamLeader | leader1  | 123      |
| Recruiter  | recruit1 | 123      |

👨‍💼 Admin
Dashboard: 
     .See system overview
     .User: Create/edit/delete users
     .Company: Add/edit company and vacancy info
     .Assign Work: Assign vacancies to team leaders

🧑‍💼 Team Leader
     .Can view assigned companies and vacancies
     .Assign recruiters to individual vacancies
     .Monitor recruiter progress

👷 Recruiter
     .View only assigned job postings
     .Upload applicant data
     .Mark completion status

🛡️ Security & Validation
     .Input validation using Data Annotations
     .Role-based redirection and session management
     .File upload restriction on images
     
📁 Folder structure
    App_Start/
    ├── BundleConfig.cs
    ├── FilterConfig.cs
    └── RouteConfig.cs
Areas/
    ├── Admin/
        ├── Controllers/
            └── AdminController.cs
        ├── Data/
            ├── Applicant_Master.cs
            ├── Applicant_Masters.cs
            ├── Company_Master.cs
            ├── Company_Masters.cs
            ├── Job_Applicant_Master.cs
            ├── Job_Applicant_Masters.cs
            ├── Job_Master.cs
            ├── Job_Mastesr.cs
            ├── JobTracksEntities.Context.cs
            ├── JobTracksEntities.Context.tt
            ├── JobTracksEntities.cs
            ├── JobTracksEntities.Designer.cs
            ├── JobTracksEntities.edmx
            ├── JobTracksEntities.edmx.diagram
            ├── JobTracksEntities.tt
            ├── Role.cs
            ├── Roles.cs
            ├── User.cs
            └── Users.cs
        ├── Views/
            ├── Admin/
                ├── AssignWork.cshtml
                ├── AssignWorkDelete.cshtml
                ├── AssignWorkEdit.cshtml
                ├── ChangePassword.cshtml
                ├── Company.cshtml
                ├── Create_Role.cshtml
                ├── Create.cshtml
                ├── CreateCompany.cshtml
                ├── Dashboard.cshtml
                ├── Delete.cshtml
                ├── Edit.cshtml
                ├── User.cshtml
                └── ViewProfile.cshtml
            ├── Shared/
                ├── _Layout.cshtml
                └── _Layout1.cshtml
            ├── _ViewStart.cshtml
            └── web.config
        └── AdminAreaRegistration.cs
    ├── Recruiter/
        ├── Controllers/
            └── RecruiterController.cs
        ├── Data/
            ├── AssignedJobViewModel.cs
            ├── EditApplicantStatusViewModel.cs
            └── RecruiterApplicantListViewModel.cs
        ├── Views/
            ├── Recruiter/
                ├── Add_Applicants.cshtml
                ├── AssignApplicants.cshtml
                ├── ChangePassword.cshtml
                ├── Dashboard.cshtml
                ├── Delete.cshtml
                ├── EditApplicantStatus.cshtml
                ├── View_Applcants.cshtml
                └── ViewProfile.cshtml
            ├── Shared/
                ├── _Layout.cshtml
                └── _Layout1.cshtml
            ├── _ViewStart.cshtml
            └── web.config
        └── RecruiterAreaRegistration.cs
    └── TeamLeader/
        ├── Controllers/
            └── TeamLeaderController.cs
        ├── Data/
            └── RecruiterSummaryViewModel.cs
        ├── Views/
            ├── Shared/
                ├── _Layout.cshtml
                └── _Layout1.cshtml
            ├── TeamLeader/
                ├── AssignWork.cshtml
                ├── AssignWorkDelete.cshtml
                ├── AssignWorkEdit.cshtml
                ├── ChangePassword.cshtml
                ├── Company.cshtml
                ├── CreateCompany.cshtml
                ├── Dashboard.cshtml
                ├── RecruiterWorkDetail.cshtml
                └── ViewProfile.cshtml
            ├── _ViewStart.cshtml
            └── web.config
        └── TeamLeaderAreaRegistration.cs
Common/
    ├── CurrentDateAttribute.cs
    ├── DateRangeAttribute.cs
    ├── ParitalCacheAttribute.cs
    └── RemoteClientServer.cs
Content/
    ├── Animations/
        └── Welcome.mp4
    ├── bootstrap-grid.css
    ├── bootstrap-grid.css.map
    ├── bootstrap-grid.min.css
    ├── bootstrap-grid.min.css.map
    ├── bootstrap-grid.rtl.css
    ├── bootstrap-grid.rtl.css.map
    ├── bootstrap-grid.rtl.min.css
    ├── bootstrap-grid.rtl.min.css.map
    ├── bootstrap-reboot.css
    ├── bootstrap-reboot.css.map
    ├── bootstrap-reboot.min.css
    ├── bootstrap-reboot.min.css.map
    ├── bootstrap-reboot.rtl.css
    ├── bootstrap-reboot.rtl.css.map
    ├── bootstrap-reboot.rtl.min.css
    ├── bootstrap-reboot.rtl.min.css.map
    ├── bootstrap-utilities.css
    ├── bootstrap-utilities.css.map
    ├── bootstrap-utilities.min.css
    ├── bootstrap-utilities.min.css.map
    ├── bootstrap-utilities.rtl.css
    ├── bootstrap-utilities.rtl.css.map
    ├── bootstrap-utilities.rtl.min.css
    ├── bootstrap-utilities.rtl.min.css.map
    ├── bootstrap.css
    ├── bootstrap.css.map
    ├── bootstrap.min.css
    ├── bootstrap.min.css.map
    ├── bootstrap.rtl.css
    ├── bootstrap.rtl.css.map
    ├── bootstrap.rtl.min.css
    ├── bootstrap.rtl.min.css.map
    ├── PagedList.css
    └── Site.css
Controllers/
    ├── ErrorController.cs
    └── HomeController.cs
EmployeePhotos/
    └── 561129d2-9c3d-4035-b6ac-bb0faf0a2c66.png
Filters/
    └── AuthorizeRolesAttribute.cs
image/
    ├── employeelogo.jpg
    ├── logo.png
    ├── logo1.png
    └── SquidGame.webp
Models/
    ├── ForgotPassword.cs
    └── Login.cs
Properties/
    └── AssemblyInfo.cs
Resumes/
    ├── January.pdf
    ├── march.pdf
    └── May.pdf
Scripts/
    ├── bootstrap.bundle.js
    ├── bootstrap.bundle.js.map
    ├── bootstrap.bundle.min.js
    ├── bootstrap.bundle.min.js.map
    ├── bootstrap.esm.js
    ├── bootstrap.esm.js.map
    ├── bootstrap.esm.min.js
    ├── bootstrap.esm.min.js.map
    ├── bootstrap.js
    ├── bootstrap.js.map
    ├── bootstrap.min.js
    ├── bootstrap.min.js.map
    ├── jquery-3.7.1.intellisense.js
    ├── jquery-3.7.1.js
    ├── jquery-3.7.1.min.js
    ├── jquery-3.7.1.min.map
    ├── jquery-3.7.1.slim.js
    ├── jquery-3.7.1.slim.min.js
    ├── jquery-3.7.1.slim.min.map
    ├── jquery.validate-vsdoc.js
    ├── jquery.validate.js
    ├── jquery.validate.min.js
    ├── jquery.validate.unobtrusive.js
    ├── jquery.validate.unobtrusive.min.js
    └── modernizr-2.8.3.js
Views/
    ├── Error/
        └── NotFound.cshtml
    ├── Home/
        ├── About.cshtml
        ├── Contact.cshtml
        ├── ForgotPassword.cshtml
        └── Index.cshtml
    ├── Shared/
        ├── _Layout.cshtml
        ├── Error.cshtml
        └── NotFound.cshtml
    ├── _ViewStart.cshtml
    └── Web.config
.gitattributes
.gitignore
favicon.ico
Global.asax
Global.asax.cs
Jobtracks_Updated_Query.sql
JobTracks.csproj
JobTracks.sln
packages.config
README.md
Web.config
Web.Debug.config
Web.Release.config
📄 License
     .This project is licensed for educational and organizational use.


```bash
git clone https://github.com/yourusername/JobTracks.git

