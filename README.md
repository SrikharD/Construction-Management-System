# Construction Management System

A modern, professional ASP.NET Core 9.0 web application for managing construction projects, sites, workers, and work orders with a sleek black and white design theme.

## 🎯 Overview

The Construction Management System (CMS) is a comprehensive solution for managing construction projects with features including:
- **Project Site Management** - Create and manage construction sites
- **Worker Management** - Track site workers and their availability
- **Work Order System** - Assign and track work orders
- **Owner Management** - Manage current property owners
- **History Tracking** - Complete audit trail of all changes
- **Role-Based Access Control** - Admin and User roles with different permissions

## 🚀 Quick Start

### Prerequisites
- **.NET 9.0 SDK** or later
- **SQL Server LocalDB** (included with Visual Studio)
- **Visual Studio 2022** or **Visual Studio Code**
- **Git** for version control

### Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/SrikharD/Construction-Management-System.git
   cd Construction-Management-System
   ```

2. **Navigate to Project Directory**
   ```bash
   cd "Construction Management System"
   ```

3. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

4. **Apply Database Migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

6. **Access the Application**
   - Open your browser and navigate to: `https://localhost:5001`
   - Or: `http://localhost:5000`

### Default Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@admin.com | Password1! |
| User | user@demo.com | Password1! |

## 📁 Project Structure

```
Construction Management System/
├── Areas/                          # ASP.NET Core Identity areas
│   └── Identity/                   # Authentication & Authorization
├── Controllers/                    # MVC Controllers (6 total)
│   ├── HomeController.cs
│   ├── ProjectSitesController.cs
│   ├── WorkOrdersController.cs
│   ├── SiteWorkersController.cs
│   ├── CurrentOwnersController.cs
│   └── HistoryController.cs
├── Models/                         # Data Models (7 total)
│   ├── ProjectSite.cs
│   ├── WorkOrder.cs
│   ├── SiteWorker.cs
│   ├── CurrentOwner.cs
│   ├── WorkAllocation.cs
│   ├── Person.cs
│   └── ErrorViewModel.cs
├── Data/                           # Database Context & Migrations
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── Views/                          # Razor Templates
│   ├── Home/
│   ├── ProjectSites/
│   ├── WorkOrders/
│   ├── SiteWorkers/
│   ├── CurrentOwners/
│   ├── History/
│   └── Shared/
├── ViewModels/                     # View-specific Models
├── wwwroot/                        # Static Files
│   ├── css/                        # Stylesheets
│   ├── js/                         # JavaScript
│   ├── lib/                        # Libraries (Bootstrap, jQuery)
│   └── favicon.ico
├── docs/                           # Documentation
│   ├── setup/                      # Setup guides
│   ├── features/                   # Feature documentation
│   ├── fixes/                      # Bug fixes & improvements
│   ├── guides/                     # Implementation guides
│   └── reference/                  # Quick references
├── Properties/                     # Project Properties
├── Program.cs                      # Application Entry Point
├── appsettings.json               # Configuration
├── appsettings.Development.json   # Development Configuration
└── ConstructionMS.csproj          # Project File
```

## ✨ Key Features

### 🎨 Modern Design
- Professional black and white theme with accent blue
- Responsive design for desktop, tablet, and mobile
- Smooth animations and transitions
- Color-coded status indicators

### 🧭 Navigation
- Modern sidebar menu with icons
- Responsive navbar with user menu
- Breadcrumb navigation
- Quick access cards on dashboard

### 📊 Dashboard
- Welcome section with user information
- Quick access cards (4-column grid)
- Role-based badges
- System information panel

### 🔐 Security
- ASP.NET Core Identity integration
- Role-based access control (Admin/User)
- Password hashing and validation
- Secure session management

### 📱 Responsive Design
- Mobile-first approach
- Bootstrap 5 framework
- Touch-friendly interface
- Optimized for all screen sizes

## 🛠️ Technology Stack

| Technology | Version | Purpose |
|-----------|---------|---------|
| ASP.NET Core | 9.0 | Web Framework |
| Entity Framework Core | 9.0 | ORM |
| SQL Server | LocalDB | Database |
| Bootstrap | 5.3 | CSS Framework |
| jQuery | 3.6 | JavaScript Library |
| Bootstrap Icons | 1.11 | Icon Library |

## 📚 Documentation

Comprehensive documentation is available in the `/docs` folder:

- **[docs/INDEX.md](Construction%20Management%20System/docs/INDEX.md)** - Documentation index
- **[docs/setup/README.md](Construction%20Management%20System/docs/setup/README.md)** - Setup guide
- **[PROJECT_STRUCTURE.md](Construction%20Management%20System/PROJECT_STRUCTURE.md)** - Project architecture
- **[GITHUB_PUSH_GUIDE.md](Construction%20Management%20System/GITHUB_PUSH_GUIDE.md)** - GitHub deployment guide

## 🔧 Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ConstructionMS;Trusted_Connection=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Database Connection
The application uses SQL Server LocalDB by default. To use a different database:
1. Update the connection string in `appsettings.json`
2. Run migrations: `dotnet ef database update`

## 🧪 Testing

To run the application in development mode:
```bash
dotnet run --configuration Debug
```

To build in Release mode:
```bash
dotnet build --configuration Release
```

## 🚀 Deployment

### Local Deployment
```bash
dotnet publish --configuration Release
```

### Production Deployment
1. Build in Release mode
2. Publish to hosting environment
3. Update connection strings for production database
4. Run migrations on production database
5. Configure SSL certificates
6. Set appropriate environment variables

## 🐛 Known Issues & Fixes

All known issues have been documented and fixed. See `/docs/fixes/` for details on:
- Entity tracking issues
- Work order and email fixes
- Associated properties and edit pages fixes
- And more...

## 📝 License

This project is part of the Construction Management System course project.

## 👨‍💻 Developer

**Srikhar Dogiparthy**
- Email: dogiparthysrikhar@gmail.com
- GitHub: [@SrikharD](https://github.com/SrikharD)

## 📞 Support

For questions or issues:
1. Check the documentation in `/docs` folder
2. Review the [PROJECT_STRUCTURE.md](Construction%20Management%20System/PROJECT_STRUCTURE.md)
3. Check the code comments and inline documentation
4. Review the [GITHUB_PUSH_GUIDE.md](Construction%20Management%20System/GITHUB_PUSH_GUIDE.md)

## 📋 Project Status

- ✅ **Build**: Successful
- ✅ **Database**: Fully migrated and ready
- ✅ **Documentation**: Complete
- ✅ **Testing**: All features tested
- ✅ **Deployment**: Ready for production

## 🎓 Version History

| Version | Date | Status |
|---------|------|--------|
| 1.0 | October 17, 2025 | Released |

---

**Last Updated**: October 17, 2025

For the latest updates and documentation, visit the [GitHub Repository](https://github.com/SrikharD/Construction-Management-System).

