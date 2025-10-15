🧩 PermohonanSystem-MVC

PermohonanSystem-MVC is an online application management system built with ASP.NET Core MVC.
It supports user registration, login, application submission with file upload, and automatic email notification to administrators.
The system also includes role-based access control (RBAC) to differentiate between normal users and admins.

⚙️ Technologies Used
Component	Technology
Framework	ASP.NET Core MVC (.NET 7 / .NET 8 compatible)
Language	C#
Database	Microsoft SQL Server (LocalDB for development)
ORM	Entity Framework Core
Authentication	Session-based (manual, without ASP.NET Identity)
Email Service	SMTP (Gmail App Password) via custom IEmailService
Frontend	Razor View + Bootstrap 5
IDE	Visual Studio 2019 / 2022
Version Control	Git + GitHub
📦 Project Structure
PermohonanSystemMVC/
│
├── Controllers/
│   ├── AccountController.cs      # Handles Login, Register, Logout
│   ├── PermohonanController.cs   # Application CRUD + Email Notification
│
├── Models/
│   ├── User.cs                   # User model
│   ├── Permohonan.cs             # Application model
│
├── Data/
│   └── AppDbContext.cs           # Entity Framework Core context
│
├── Services/
│   ├── IEmailService.cs          # Email service interface
│   └── EmailService.cs           # Gmail SMTP implementation
│
├── Views/
│   ├── Account/                  # Register & Login pages
│   ├── Permohonan/               # Index, Create, Edit pages
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── uploads/ (🛑 ignored in Git)
│
├── appsettings.json              # Configuration (connection string & email)
├── .gitignore                    # Ignore rules
└── Program.cs / Startup.cs       # Application entry point

🗄️ Database

Default connection string:

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PermohonanCoreDB;Trusted_Connection=True;TrustServerCertificate=True"
}

Main Entities:

User

Id, Name, Email, Password (hashed via BCrypt), Role

Permohonan (Application)

Id, Title, Description, Date, UserId (FK), DocumentPath

📧 Email Notification

When a user submits an application, the system automatically:

Sends an email notification to the admin (EmailSettings:AdminTo)

Includes basic application details

Attaches the uploaded document (if available)

Example configuration:

"EmailSettings": {
  "Smtp": "smtp.gmail.com",
  "Port": 587,
  "User": "youremail@domain.com",
  "Pass": "your_app_password",
  "FromName": "Permohonan System",
  "AdminTo": "admin@domain.com"
}

👥 Role-Based Access
Role	Permission
User	Can create, edit, and view their own applications
Admin	Can view all applications from all users
🚀 Local Setup Guide

Clone the repository

git clone https://github.com/khairulhisham90/PermohonanSystem-MVC.git


Open in Visual Studio

Update database connection in appsettings.json

"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PermohonanCoreDB;Trusted_Connection=True;"


Run Entity Framework migrations

Add-Migration InitialCreate
Update-Database


Launch the application

https://localhost:xxxx

🧰 Additional Notes

The wwwroot/uploads/ folder is ignored by Git to prevent storing user files.

Sensitive config files such as appsettings.*.json and secrets.json are excluded from version control.

The public appsettings.json in this repo only contains dummy credentials for safety.



© 2025 Khairul Hisham — ASP.NET Core MVC project for portfolio and learning purposes.