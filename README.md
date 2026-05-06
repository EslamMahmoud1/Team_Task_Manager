 Team Task Manager is an ASP.NET Core MVC web application for creating and tracking tasks inside a team. It provides
  authentication/authorization using ASP.NET Core Identity , role + permission management, and a user
  dashboard showing created vs assigned tasks, completion stats, and task details.

  ## Tech Stack

  - .NET: ASP.NET Core MVC (net8.0)
  - Database: SQL Server + Entity Framework Core (code-first migrations + auto-migrate on startup)
  - Auth: ASP.NET Core Identity (cookie auth)
  - Mapping: Mapster
  - UI: MVC Views + Kendo UI (NuGet: KendoUIProfessional)
  - Email: FluentEmail (SMTP sender)

  ## Main Features

  - Authentication: Login/Register views + password reset (email-based token flow)
  - Tasks:
      - Create task (title, description, due date, priority, assign to user)
      - View task details
      - Mark complete / uncomplete
      - Delete task
  - Dashboard:
      - Shows created tasks and assigned tasks
      - Summary stats (completed/pending, totals)
  - Roles & Permissions:
      - UserRoles + Permission + many-to-many via RolePermission
      - Permission-based MVC authorization via PermissionAuthFilterAttribute
      - Seeded default permissions + an “Admin” role
