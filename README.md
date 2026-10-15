Management Utility
Management Utility is a Windows desktop application for tracking construction company assets. It provides a central list of equipment and other assets, with inventory details, status, location, purchase information, and responsible person.
Demo
<!-- Replace this placeholder with a short GIF or a link to a demo video. Put the GIF in docs/management-utility-demo.gif. -->


Features
- View assets in a searchable table.
- Add, edit, and delete assets.
- Search by inventory number, name, or category.
- Track category, location, responsible person, and status.
- Store optional purchase date, price, and notes.
- Validate required fields and prevent duplicate inventory numbers.
- Persist data locally in SQLite; EF Core applies database migrations at startup.
  Technology
- C# and .NET 10
- Windows Forms
- Entity Framework Core 10
- SQLite
- Dependency injection
- Layered solution with Core, Infrastructure, and WinForms projects
  Architecture
  ManagementUtility.sln
  └── src
  ├── ManagementUtility.Core
  │   ├── DTOs
  │   ├── Entities
  │   ├── Enums
  │   ├── Interfaces
  │   └── Services
  ├── ManagementUtility.Infrastructure
  │   ├── Persistence
  │   │   ├── Configurations
  │   │   └── Migrations
  │   └── Repositories
  └── ManagementUtility.WinForms
  ├── Forms
  └── Program.cs
  Core contains the asset model, DTOs, service contracts, and asset business logic. Infrastructure implements data access with EF Core and SQLite. WinForms is the desktop UI and composition root where dependencies are registered.
  Requirements
- Windows
- .NET 10 SDK
  Run locally
  Clone the repository, open a terminal in the solution directory, then run:
  dotnet restore
  dotnet build ManagementUtility.sln
  dotnet run --project src/ManagementUtility.WinForms/ManagementUtility.WinForms.csproj
  The application creates its SQLite database and applies migrations on startup. The database is stored at:
  %LOCALAPPDATA%\ManagementUtility\management-utility.db
  Database migrations
  The EF Core design-time factory is in the Infrastructure project. To create a new migration from the solution directory, run:
  dotnet ef migrations add MigrationName --project src/ManagementUtility.Infrastructure/ManagementUtility.Infrastructure.csproj --startup-project src/ManagementUtility.WinForms/ManagementUtility.WinForms.csproj --output-dir Persistence/Migrations
  Asset statuses
  Status	Meaning
  InUse	The asset is currently in use.
  InRepair	The asset is being repaired.
  InStorage	The asset is in storage.
  WrittenOff	The asset has been written off.


Current scope
This version focuses on the core single-user asset tracking workflow. Authentication, employee management, reporting, and multi-user synchronization are outside the current MVP scope.