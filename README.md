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
     
📄 License
     .This project is licensed for educational and organizational use.


```bash
git clone https://github.com/yourusername/JobTracks.git

