# Construction Management System - Project Structure

## 📁 Directory Organization

This document outlines the project structure following ASP.NET Core best practices.

### Root Level Files
```
├── Program.cs                          # Application entry point and configuration
├── Constants.cs                        # Application constants
├── SessionExtensions.cs                # Session extension methods
├── SeedData.cs                         # Database seed data
├── ConstructionMS.csproj              # Project file
├── ConstructionMS.sln                 # Solution file
├── appsettings.json                   # Application settings
├── appsettings.Development.json       # Development settings
└── README.md                          # Project documentation
```

### 📂 Core Directories

#### `/Areas`
Identity and authentication related pages
```
Areas/
└── Identity/
    └── Pages/
        └── Account/
            ├── Login.cshtml           # Login page
            ├── Register.cshtml        # Registration page
            └── ...                    # Other identity pages
```

#### `/Controllers`
MVC Controllers handling business logic
```
Controllers/
├── HomeController.cs                  # Home page controller
├── CurrentOwnersController.cs         # Current owners management
├── ProjectSitesController.cs          # Project sites management
├── SiteWorkersController.cs           # Site workers management
├── WorkOrdersController.cs            # Work orders management
└── HistoryController.cs               # History tracking
```

#### `/Models`
Data models and entities
```
Models/
├── Person.cs                          # Base person model
├── CurrentOwner.cs                    # Current owner entity
├── ProjectSite.cs                     # Project site entity
├── SiteWorker.cs                      # Site worker entity
├── WorkOrder.cs                       # Work order entity
├── WorkAllocation.cs                  # Work allocation entity
└── ErrorViewModel.cs                  # Error view model
```

#### `/Data`
Database context and migrations
```
Data/
├── ApplicationDbContext.cs            # EF Core DbContext
└── Migrations/                        # Database migrations
    └── [timestamp]_Initial.cs         # Initial migration
```

#### `/Views`
Razor view templates organized by controller
```
Views/
├── Shared/
│   ├── _Layout.cshtml                 # Master layout
│   ├── _SidebarMenu.cshtml            # Sidebar navigation
│   ├── _LoginPartial.cshtml           # Login partial
│   └── _Layout.cshtml.css             # Layout styles
├── Home/
│   ├── Index.cshtml                   # Home page
│   ├── Privacy.cshtml                 # Privacy page
│   └── Help.cshtml                    # Help page
├── CurrentOwners/
│   ├── Index.cshtml                   # List view
│   ├── Create.cshtml                  # Create form
│   ├── Edit.cshtml                    # Edit form
│   └── Delete.cshtml                  # Delete confirmation
├── ProjectSites/
│   ├── Index.cshtml                   # List view
│   ├── Create.cshtml                  # Create form
│   ├── Edit.cshtml                    # Edit form
│   ├── Delete.cshtml                  # Delete confirmation
│   └── Search.cshtml                  # Search page
├── SiteWorkers/
│   ├── Index.cshtml                   # List view
│   ├── Create.cshtml                  # Create form
│   ├── Edit.cshtml                    # Edit form
│   └── Delete.cshtml                  # Delete confirmation
├── WorkOrders/
│   ├── Index.cshtml                   # List view
│   ├── Create.cshtml                  # Create form
│   ├── Edit.cshtml                    # Edit form
│   └── Delete.cshtml                  # Delete confirmation
├── History/
│   └── Index.cshtml                   # History view
├── _ViewStart.cshtml                  # View start configuration
└── _ViewImports.cshtml                # View imports
```

#### `/ViewModels`
View-specific models for data transfer
```
ViewModels/
├── ProjectSiteSearchViewModel.cs      # Search view model
├── SiteWorker_AddWorkOrder_ViewModel.cs
└── WorkOrder_AddSiteWorker_ViewModel.cs
```

#### `/wwwroot`
Static files (CSS, JavaScript, images)
```
wwwroot/
├── css/
│   └── site.css                       # Main stylesheet
├── js/
│   └── site.js                        # Main JavaScript
├── lib/
│   ├── bootstrap/                     # Bootstrap framework
│   ├── jquery/                        # jQuery library
│   └── ...                            # Other libraries
└── favicon.ico                        # Site favicon
```

#### `/Properties`
Project properties and launch settings
```
Properties/
├── launchSettings.json                # Launch configuration
├── serviceDependencies.json           # Service dependencies
└── serviceDependencies.local.json     # Local service dependencies
```

#### `/docs`
Project documentation (organized by category)
```
docs/
├── INDEX.md                           # Documentation index
├── setup/                             # Setup and installation
├── features/                          # Feature documentation
├── fixes/                             # Bug fixes and improvements
├── guides/                            # Implementation guides
└── reference/                         # Quick references
```

#### `/bin` and `/obj`
Build output directories (auto-generated)
```
bin/
├── Debug/                             # Debug build output
└── Release/                           # Release build output

obj/
└── [build artifacts]                  # Intermediate build files
```

## 🏗️ Architecture Overview

### Layered Architecture
```
┌─────────────────────────────────────┐
│         Views (Razor)               │
│    (User Interface Layer)           │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Controllers                    │
│   (Business Logic Layer)            │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│    Models & ViewModels              │
│   (Data Transfer Layer)             │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   ApplicationDbContext              │
│   (Data Access Layer)               │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      SQL Server Database            │
│   (Persistence Layer)               │
└─────────────────────────────────────┘
```

## 📋 Key Files and Their Purposes

| File | Purpose |
|------|---------|
| Program.cs | Application startup and configuration |
| Constants.cs | Application-wide constants |
| SessionExtensions.cs | Session management utilities |
| SeedData.cs | Initial database population |
| ApplicationDbContext.cs | Entity Framework Core context |
| _Layout.cshtml | Master page template |
| site.css | Global stylesheet |
| site.js | Global JavaScript |

## 🔄 Data Flow

1. **User Request** → Browser sends HTTP request
2. **Routing** → ASP.NET Core routes to appropriate Controller
3. **Controller** → Processes request, interacts with Models
4. **Data Access** → ApplicationDbContext queries database
5. **Response** → Controller returns View with data
6. **Rendering** → Razor engine renders HTML
7. **Response** → Browser receives and displays page

## 📦 Dependencies

- **ASP.NET Core 9.0** - Web framework
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **Bootstrap 5** - UI framework
- **Bootstrap Icons** - Icon library
- **jQuery** - JavaScript library

---

**Last Updated**: October 2025
**Developer**: Srikhar Dogiparthy

