USE [master]
GO
/****** Object:  Database [JobTracks]    Script Date: 29-07-2025 20:39:15 ******/
CREATE DATABASE [JobTracks]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'JobTracks', FILENAME = N'C:\MSSQL\SQL_DATA\MSSQL16.SQLEXPRESS\MSSQL\DATA\JobTracks.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'JobTracks_log', FILENAME = N'C:\MSSQL\SQL_DATA\MSSQL16.SQLEXPRESS\MSSQL\DATA\JobTracks_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [JobTracks] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [JobTracks].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [JobTracks] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [JobTracks] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [JobTracks] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [JobTracks] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [JobTracks] SET ARITHABORT OFF 
GO
ALTER DATABASE [JobTracks] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [JobTracks] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [JobTracks] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [JobTracks] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [JobTracks] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [JobTracks] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [JobTracks] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [JobTracks] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [JobTracks] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [JobTracks] SET  ENABLE_BROKER 
GO
ALTER DATABASE [JobTracks] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [JobTracks] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [JobTracks] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [JobTracks] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [JobTracks] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [JobTracks] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [JobTracks] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [JobTracks] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [JobTracks] SET  MULTI_USER 
GO
ALTER DATABASE [JobTracks] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [JobTracks] SET DB_CHAINING OFF 
GO
ALTER DATABASE [JobTracks] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [JobTracks] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [JobTracks] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [JobTracks] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [JobTracks] SET QUERY_STORE = ON
GO
ALTER DATABASE [JobTracks] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [JobTracks]
GO
/****** Object:  Table [dbo].[Applicant_Master]    Script Date: 29-07-2025 20:39:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Applicant_Master](
	[AppLicant_id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[Last_Qualification] [varchar](100) NULL,
	[PassOutYear] [int] NULL,
	[YearOfExperience] [int] NULL,
	[Resume] [text] NULL,
	[Status] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[AppLicant_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Company_Master]    Script Date: 29-07-2025 20:39:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company_Master](
	[Company_id] [int] IDENTITY(1,1) NOT NULL,
	[Company_Name] [varchar](100) NOT NULL,
	[Description] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[Company_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Job_Applicant_Master]    Script Date: 29-07-2025 20:39:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Job_Applicant_Master](
	[Job_Id] [int] IDENTITY(1,1) NOT NULL,
	[Recuriter_ID] [int] NOT NULL,
	[Applicant_ID] [int] NOT NULL,
	[Status] [varchar](50) NULL,
	[JobRef_Id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Job_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Job_Master]    Script Date: 29-07-2025 20:39:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Job_Master](
	[Job_id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](20) NOT NULL,
	[Description] [text] NULL,
	[Tech_Stack] [varchar](150) NULL,
	[status] [varchar](50) NULL,
	[Company_Id] [int] NULL,
	[TeamLeader_Id] [int] NULL,
	[Recruiter_Id] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[TentativeDate] [datetime] NULL,
	[RequiredCount] [int] NOT NULL,
	[PlacedCount] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Job_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 29-07-2025 20:39:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 29-07-2025 20:39:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[User_id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[Email] [varchar](75) NOT NULL,
	[Password] [varchar](55) NOT NULL,
	[Role_id] [int] NULL,
	[FullName] [nvarchar](100) NULL,
	[FatherName] [nvarchar](100) NULL,
	[MotherName] [nvarchar](100) NULL,
	[Gender] [nvarchar](10) NULL,
	[DateOfBirth] [date] NULL,
	[JoiningDate] [date] NULL,
	[Branch] [nvarchar](50) NULL,
	[AadharNumber] [nvarchar](20) NULL,
	[UANNumber] [nvarchar](20) NULL,
	[BloodGroup] [nvarchar](10) NULL,
	[BankAccount_1] [nvarchar](30) NULL,
	[BankAccount_2] [nvarchar](30) NULL,
	[PhoneNumber] [nvarchar](15) NULL,
	[Employee_Photo] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[User_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Applicant_Master] ON 

INSERT [dbo].[Applicant_Master] ([AppLicant_id], [FirstName], [LastName], [Last_Qualification], [PassOutYear], [YearOfExperience], [Resume], [Status]) VALUES (1, N'John', N'Doe', N'B.Tech', 2019, 3, N'john_resume.pdf', N'Placed')
INSERT [dbo].[Applicant_Master] ([AppLicant_id], [FirstName], [LastName], [Last_Qualification], [PassOutYear], [YearOfExperience], [Resume], [Status]) VALUES (2, N'Priya', N'Sharma', N'MBA', 2020, 2, N'priya_resume.pdf', N'Interview Scheduled')
INSERT [dbo].[Applicant_Master] ([AppLicant_id], [FirstName], [LastName], [Last_Qualification], [PassOutYear], [YearOfExperience], [Resume], [Status]) VALUES (3, N'Raj', N'Kumar', N'B.Sc', 2018, 4, N'raj_resume.pdf', N'Rejected')
INSERT [dbo].[Applicant_Master] ([AppLicant_id], [FirstName], [LastName], [Last_Qualification], [PassOutYear], [YearOfExperience], [Resume], [Status]) VALUES (4, N'Aisha', N'Khan', N'MCA', 2021, 1, N'aisha_resume.pdf', N'Shortlisted')
INSERT [dbo].[Applicant_Master] ([AppLicant_id], [FirstName], [LastName], [Last_Qualification], [PassOutYear], [YearOfExperience], [Resume], [Status]) VALUES (5, N'David', N'Smith', N'BE', 2017, 5, N'david_resume.pdf', N'Placed')
INSERT [dbo].[Applicant_Master] ([AppLicant_id], [FirstName], [LastName], [Last_Qualification], [PassOutYear], [YearOfExperience], [Resume], [Status]) VALUES (11, N'om', N'ayare', N'commom', 123, 123, N'march.pdf', NULL)
INSERT [dbo].[Applicant_Master] ([AppLicant_id], [FirstName], [LastName], [Last_Qualification], [PassOutYear], [YearOfExperience], [Resume], [Status]) VALUES (12, N'om', N'ayare', N'commom', 123, 156, N'May.pdf', NULL)
INSERT [dbo].[Applicant_Master] ([AppLicant_id], [FirstName], [LastName], [Last_Qualification], [PassOutYear], [YearOfExperience], [Resume], [Status]) VALUES (13, N'dasdsad', N'ssdsd', N'444', 2015, 25, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Applicant_Master] OFF
GO
SET IDENTITY_INSERT [dbo].[Company_Master] ON 

INSERT [dbo].[Company_Master] ([Company_id], [Company_Name], [Description]) VALUES (1, N'TCS ', N'jfjlfalifjlkj lkfjlasjlksajlajdljladjajdoaiaasjajfkajio')
INSERT [dbo].[Company_Master] ([Company_id], [Company_Name], [Description]) VALUES (2, N'ITC', N'ededdadad')
INSERT [dbo].[Company_Master] ([Company_id], [Company_Name], [Description]) VALUES (3, N'Amazon', N'dadadeadeada')
SET IDENTITY_INSERT [dbo].[Company_Master] OFF
GO
SET IDENTITY_INSERT [dbo].[Job_Applicant_Master] ON 

INSERT [dbo].[Job_Applicant_Master] ([Job_Id], [Recuriter_ID], [Applicant_ID], [Status], [JobRef_Id]) VALUES (1, 3, 1, N'Shortlisted', 7)
INSERT [dbo].[Job_Applicant_Master] ([Job_Id], [Recuriter_ID], [Applicant_ID], [Status], [JobRef_Id]) VALUES (2, 3, 2, N'Shortlisted', 3)
INSERT [dbo].[Job_Applicant_Master] ([Job_Id], [Recuriter_ID], [Applicant_ID], [Status], [JobRef_Id]) VALUES (3, 3, 3, N'Placed', 8)
INSERT [dbo].[Job_Applicant_Master] ([Job_Id], [Recuriter_ID], [Applicant_ID], [Status], [JobRef_Id]) VALUES (4, 3, 4, N'Placed', 8)
INSERT [dbo].[Job_Applicant_Master] ([Job_Id], [Recuriter_ID], [Applicant_ID], [Status], [JobRef_Id]) VALUES (5, 3, 5, N'Placed', 8)
INSERT [dbo].[Job_Applicant_Master] ([Job_Id], [Recuriter_ID], [Applicant_ID], [Status], [JobRef_Id]) VALUES (6, 3, 12, NULL, NULL)
INSERT [dbo].[Job_Applicant_Master] ([Job_Id], [Recuriter_ID], [Applicant_ID], [Status], [JobRef_Id]) VALUES (7, 3, 13, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Job_Applicant_Master] OFF
GO
SET IDENTITY_INSERT [dbo].[Job_Master] ON 

INSERT [dbo].[Job_Master] ([Job_id], [Title], [Description], [Tech_Stack], [status], [Company_Id], [TeamLeader_Id], [Recruiter_Id], [CreatedDate], [TentativeDate], [RequiredCount], [PlacedCount]) VALUES (3, N'jr.dev', N'Its Simple Booking Site with Pure Html&Css', N' .net WPF', N'Open', 1, 2, 3, CAST(N'2025-06-21T20:53:38.213' AS DateTime), CAST(N'2025-07-23T00:00:00.000' AS DateTime), 0, 0)
INSERT [dbo].[Job_Master] ([Job_id], [Title], [Description], [Tech_Stack], [status], [Company_Id], [TeamLeader_Id], [Recruiter_Id], [CreatedDate], [TentativeDate], [RequiredCount], [PlacedCount]) VALUES (6, N'Jr.Dev', N'its simple web app were admin can add update delete and edit the product ', N' .net WPF', N'Open', 1, 2, 3, CAST(N'2025-06-21T20:53:38.213' AS DateTime), CAST(N'2025-07-18T00:00:00.000' AS DateTime), 0, 0)
INSERT [dbo].[Job_Master] ([Job_id], [Title], [Description], [Tech_Stack], [status], [Company_Id], [TeamLeader_Id], [Recruiter_Id], [CreatedDate], [TentativeDate], [RequiredCount], [PlacedCount]) VALUES (7, N'.sr. dev', N'Its Simple Booking Site with Pure Html&Css', N' .net WPF', N'Open', 2, 2, 3, CAST(N'2025-06-22T21:22:49.623' AS DateTime), CAST(N'2025-07-21T00:00:00.000' AS DateTime), 0, 0)
INSERT [dbo].[Job_Master] ([Job_id], [Title], [Description], [Tech_Stack], [status], [Company_Id], [TeamLeader_Id], [Recruiter_Id], [CreatedDate], [TentativeDate], [RequiredCount], [PlacedCount]) VALUES (8, N'Assiant', N'Its Simple Booking Site with Pure Html&Css', N'Java', N'In Progress', 3, 2, 3, CAST(N'2025-07-03T20:07:55.857' AS DateTime), CAST(N'2025-07-20T00:00:00.000' AS DateTime), 5, 4)
SET IDENTITY_INSERT [dbo].[Job_Master] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([Id], [Name]) VALUES (1, N'Admin')
INSERT [dbo].[Roles] ([Id], [Name]) VALUES (2, N'TeamLeader')
INSERT [dbo].[Roles] ([Id], [Name]) VALUES (3, N'Recruiter')
INSERT [dbo].[Roles] ([Id], [Name]) VALUES (4, N'Manager ')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([User_id], [Username], [Email], [Password], [Role_id], [FullName], [FatherName], [MotherName], [Gender], [DateOfBirth], [JoiningDate], [Branch], [AadharNumber], [UANNumber], [BloodGroup], [BankAccount_1], [BankAccount_2], [PhoneNumber], [Employee_Photo]) VALUES (1, N'om', N'admin12@gmail.com', N'123', 1, N'Om Sanjay Ayare', N'Sanjay Govind Ayare', N'Snehal Sanjay Ayare ', N'Male', CAST(N'2002-10-16' AS Date), CAST(N'2025-07-21' AS Date), NULL, N'assaass2226666', NULL, N'O+', N'dfgdfgghghhfghsdggfg', N'sdadfdfghjkjfxcdg', N'9933665544', N'561129d2-9c3d-4035-b6ac-bb0faf0a2c66.png')
INSERT [dbo].[Users] ([User_id], [Username], [Email], [Password], [Role_id], [FullName], [FatherName], [MotherName], [Gender], [DateOfBirth], [JoiningDate], [Branch], [AadharNumber], [UANNumber], [BloodGroup], [BankAccount_1], [BankAccount_2], [PhoneNumber], [Employee_Photo]) VALUES (2, N'Tlom', N'tladi@gmail.com', N'123', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([User_id], [Username], [Email], [Password], [Role_id], [FullName], [FatherName], [MotherName], [Gender], [DateOfBirth], [JoiningDate], [Branch], [AadharNumber], [UANNumber], [BloodGroup], [BankAccount_1], [BankAccount_2], [PhoneNumber], [Employee_Photo]) VALUES (3, N'RecOm', N'recadi@gmail.com', N'123', 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([User_id], [Username], [Email], [Password], [Role_id], [FullName], [FatherName], [MotherName], [Gender], [DateOfBirth], [JoiningDate], [Branch], [AadharNumber], [UANNumber], [BloodGroup], [BankAccount_1], [BankAccount_2], [PhoneNumber], [Employee_Photo]) VALUES (4, N'TLWork', N'employee@gmail.com', N'1122', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Users] ([User_id], [Username], [Email], [Password], [Role_id], [FullName], [FatherName], [MotherName], [Gender], [DateOfBirth], [JoiningDate], [Branch], [AadharNumber], [UANNumber], [BloodGroup], [BankAccount_1], [BankAccount_2], [PhoneNumber], [Employee_Photo]) VALUES (5, N'Manage1', N'manage@gmail.com', N'123456', 4, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D1053487BA00D3]    Script Date: 29-07-2025 20:39:15 ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Job_Master] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Job_Master] ADD  DEFAULT ((0)) FOR [RequiredCount]
GO
ALTER TABLE [dbo].[Job_Master] ADD  DEFAULT ((0)) FOR [PlacedCount]
GO
ALTER TABLE [dbo].[Job_Applicant_Master]  WITH CHECK ADD  CONSTRAINT [FK_JobApplicant_Applicant] FOREIGN KEY([Applicant_ID])
REFERENCES [dbo].[Applicant_Master] ([AppLicant_id])
GO
ALTER TABLE [dbo].[Job_Applicant_Master] CHECK CONSTRAINT [FK_JobApplicant_Applicant]
GO
ALTER TABLE [dbo].[Job_Applicant_Master]  WITH CHECK ADD  CONSTRAINT [FK_JobApplicant_JobRef] FOREIGN KEY([JobRef_Id])
REFERENCES [dbo].[Job_Master] ([Job_id])
GO
ALTER TABLE [dbo].[Job_Applicant_Master] CHECK CONSTRAINT [FK_JobApplicant_JobRef]
GO
ALTER TABLE [dbo].[Job_Applicant_Master]  WITH CHECK ADD  CONSTRAINT [FK_JobApplicant_Recruiter] FOREIGN KEY([Recuriter_ID])
REFERENCES [dbo].[Users] ([User_id])
GO
ALTER TABLE [dbo].[Job_Applicant_Master] CHECK CONSTRAINT [FK_JobApplicant_Recruiter]
GO
ALTER TABLE [dbo].[Job_Master]  WITH CHECK ADD FOREIGN KEY([Company_Id])
REFERENCES [dbo].[Company_Master] ([Company_id])
GO
ALTER TABLE [dbo].[Job_Master]  WITH CHECK ADD FOREIGN KEY([Recruiter_Id])
REFERENCES [dbo].[Users] ([User_id])
GO
ALTER TABLE [dbo].[Job_Master]  WITH CHECK ADD FOREIGN KEY([TeamLeader_Id])
REFERENCES [dbo].[Users] ([User_id])
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK__Users__Role_id__4CA06362] FOREIGN KEY([Role_id])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK__Users__Role_id__4CA06362]
GO
USE [master]
GO
ALTER DATABASE [JobTracks] SET  READ_WRITE 
GO
