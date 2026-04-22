# TechMoves Logistics

A comprehensive logistics management system built with ASP.NET Core 10, Entity Framework Core, and SQL Server. This application enables efficient management of clients, contracts, and service requests with multi-currency support for ZAR (South African Rand) and USD currencies.

## Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Database Configuration](#database-configuration)
- [Running the Application](#running-the-application)
- [Project Architecture](#project-architecture)
- [Key Features Explained](#key-features-explained)
- [Testing](#testing)

## Features

### Client Management
- Create, read, update, and delete client information
- Store client contact details and regional information
- Track multiple contracts per client
- Comprehensive client directory

### Contract Management
- Create and manage service contracts with clients
- Define contract periods (start and end dates)
- Track contract status (Draft, Active, Completed, Expired)
- Specify service levels and terms
- Upload and store signed agreement documents (PDF)
- Search contracts by date range and status

### Service Request Management
- Create service requests under existing contracts
- Track request status (Pending, In Progress, Completed, Cancelled)
- Detailed description of requested services
- Multi-currency cost tracking (ZAR and USD)
- Automatic exchange rate recording at time of request creation
- Timestamp tracking for all requests

### Currency Conversion
- Real-time USD to ZAR exchange rate integration
- External API integration with ExchangeRate-API service
- Fallback rate mechanism for API downtime
- Automatic rate recording for auditing purposes

### File Management
- Secure file upload for signed agreements
- File storage in dedicated upload directory
- File validation and security measures

## Technology Stack

- **Framework**: ASP.NET Core 10
- **Language**: C# 12+
- **Database**: SQL Server (LocalDB for development)
- **ORM**: Entity Framework Core 10.0.6
- **Pattern**: Repository Pattern with Service Layer
- **API Integration**: External Currency Exchange API
- **Testing Framework**: xUnit (with test project structure in place)

### NuGet Packages
```xml
- Microsoft.EntityFrameworkCore (10.0.6)
- Microsoft.EntityFrameworkCore.SqlServer (10.0.6)
- Microsoft.EntityFrameworkCore.Design (10.0.6)
- Microsoft.EntityFrameworkCore.Tools (10.0.6)
- Microsoft.VisualStudio.Web.CodeGeneration.Design (10.0.2)
- NuGet.Protocol (6.12.1)
```

## Project Structure

```
TechMoves Logistics/
├── Controllers/
│   ├── ClientsController.cs           # Client CRUD operations
│   ├── ContractsController.cs         # Contract management endpoints
│   ├── ServiceRequestsController.cs   # Service request management
│   └── HomeController.cs              # Home page and general routing
├── Models/
│   ├── Client.cs                      # Client entity model
│   ├── Contract.cs                    # Contract entity model
│   ├── ServiceRequest.cs              # Service request entity model
│   ├── ErrorViewModel.cs              # Error handling model
│   └── Enums/
│       ├── ContractStatus.cs          # Contract status enumeration
│       └── ServiceRequestStatus.cs    # Service request status enumeration
├── Data/
│   └── ApplicationDbContext.cs        # EF Core database context
├── Repositories/
│   ├── Interfaces/
│   │   ├── IClientRepository.cs
│   │   ├── IContractRepository.cs
│   │   └── IServiceRequestRepository.cs
│   ├── ClientRepository.cs            # Client data access
│   ├── ContractRepository.cs          # Contract data access
│   └── ServiceRequestRepository.cs    # Service request data access
├── Services/
│   ├── Interfaces/
│   │   ├── IClientService.cs          # (if implemented)
│   │   ├── IContractService.cs
│   │   ├── ICurrencyService.cs
│   │   ├── IFileService.cs
│   │   └── IServiceRequestService.cs
│   ├── ContractService.cs             # Contract business logic
│   ├── CurrencyService.cs             # Currency conversion logic
│   ├── FileService.cs                 # File upload/management
│   └── ServiceRequestService.cs       # Service request business logic
├── Views/
│   ├── Clients/                       # Client management UI
│   ├── Contracts/                     # Contract management UI
│   ├── ServiceRequests/               # Service request UI
│   ├── Home/                          # Home page views
│   ├── Shared/                        # Shared layouts and components
│   ├── _ViewStart.cshtml              # View configuration
│   └── _ViewImports.cshtml            # Global view imports
├── Migrations/                        # EF Core database migrations
├── Properties/
│   └── launchSettings.json            # Application launch configuration
├── wwwroot/
│   ├── css/                           # Stylesheets
│   ├── js/                            # JavaScript files
│   ├── lib/                           # Client-side libraries
│   └── uploads/                       # File upload storage
├── appsettings.json                   # Application settings (production)
├── appsettings.Development.json       # Development-specific settings
├── Program.cs                         # Application startup configuration
└── TechMovesLogistics.csproj          # Project file

TechMovesLogistics.Tests/
├── ContractServiceTests.cs            # Contract service unit tests
├── CurrencyServiceTests.cs            # Currency service unit tests
├── FileServiceTests.cs                # File service unit tests
├── ServiceRequestServiceTests.cs      # Service request service unit tests
└── TechMovesLogistics.Tests.csproj    # Test project file
```

## Prerequisites

- **.NET 10 SDK** - Download from [microsoft.com](https://dotnet.microsoft.com/download)
- **SQL Server** - LocalDB comes with Visual Studio or install SQL Server Express
- **Visual Studio 2025** (recommended) or **Visual Studio Code** with C# extension
- **Git** (for version control)

## Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd "TechMoves Logistics"
```

### 2. Restore NuGet Packages
```bash
cd "TechMoves Logistics"
dotnet restore
```

### 3. Configure Database Connection
The default connection string in `appsettings.json` uses LocalDB:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TechMovesLogisticsDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

To use a different SQL Server instance, update the connection string in `appsettings.json` or `appsettings.Development.json`.

## Database Configuration

### Initial Setup

The database schema is configured through Entity Framework Core migrations.

### 1. Apply Migrations
```bash
cd "TechMoves Logistics"
dotnet ef database update
```

### 2. Manual Migration (if needed)
```bash
# Create a new migration
dotnet ef migrations add <MigrationName>

# Apply migration
dotnet ef database update
```

### Database Schema Overview

**Clients Table**
- Stores client information including name, contact details, and region
- One-to-many relationship with Contracts

**Contracts Table**
- Linked to Clients via foreign key
- Tracks contract period, status, and service level
- Stores path to signed agreement documents
- One-to-many relationship with ServiceRequests

**ServiceRequests Table**
- Linked to Contracts via foreign key
- Stores request description, cost in ZAR/USD
- Tracks exchange rate at time of creation
- Includes status and creation timestamp

## Running the Application

### Using Visual Studio
1. Open `TechMovesLogistics.slnx` in Visual Studio 2025
2. Set "TechMoves Logistics" project as the startup project
3. Press `F5` or click **Debug > Start Debugging**
4. The application opens in your default browser (typically `https://localhost:5001`)

### Using .NET CLI
```bash
cd "TechMoves Logistics"
dotnet run
```

The application will be available at:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`

## Project Architecture

### Design Patterns Used

**Repository Pattern**
- Data access layer is abstracted through repository interfaces
- Each entity (Client, Contract, ServiceRequest) has a dedicated repository
- Enables easy testing and data access flexibility

**Service Layer Pattern**
- Business logic is separated from controllers
- Services handle complex operations and validation
- Dependency injection manages service lifecycle

**Dependency Injection**
- All dependencies are registered in `Program.cs`
- Constructor injection throughout the application
- Loose coupling between layers

### Layered Architecture

1. **Controller Layer** - Handles HTTP requests/responses
2. **Service Layer** - Implements business logic and validation
3. **Repository Layer** - Manages data access
4. **Data Layer** - Entity Framework Core and database context
5. **Model Layer** - Domain entities and enumerations

## Key Features Explained

### Client Management
- Full CRUD operations for clients
- Store comprehensive contact information
- Regional classification for organizational purposes

### Contract Lifecycle Management
- **Draft**: Initial contract creation
- **Active**: Ongoing service contract
- **Completed**: Contract fulfilled
- **Expired**: Contract term ended

### Service Request Processing
- Link service requests to specific contracts
- Multi-currency support with automatic conversion tracking
- Exchange rate history for audit purposes
- Status tracking throughout request lifecycle:
  - **Pending**: Awaiting processing
  - **In Progress**: Currently being handled
  - **Completed**: Service delivered
  - **Cancelled**: Request cancelled

### Currency Management
- Real-time exchange rate fetching from ExchangeRate-API
- Stores both ZAR and USD amounts
- Records exchange rate at time of transaction
- Fallback mechanism (18.50 ZAR per USD) if API unavailable

### File Management
- Secure file upload for signed agreements
- Storage in dedicated `wwwroot/uploads` directory
- Document association with contracts

## Testing

The project includes a comprehensive test suite in `TechMovesLogistics.Tests/`:

### Test Classes
- `ContractServiceTests.cs` - Tests for contract business logic
- `CurrencyServiceTests.cs` - Tests for currency conversion functionality
- `FileServiceTests.cs` - Tests for file upload/management
- `ServiceRequestServiceTests.cs` - Tests for service request operations

### Running Tests
```bash
cd TechMovesLogistics.Tests
dotnet test
```

Or run tests through Visual Studio Test Explorer (Test > Test Explorer or Ctrl+E, Ctrl+T)

## Configuration

### Key Configuration Files

**appsettings.json**
```json
{
  "Logging": { /* Logging configuration */ },
  "ConnectionStrings": { /* Database connection */ },
  "CurrencyApi": { /* Exchange rate API configuration */ },
  "AllowedHosts": "*"
}
```

**appsettings.Development.json**
- Development-specific settings override production settings
- Override logging levels, connection strings, or API endpoints as needed

## Common Tasks

### Adding a New Entity
1. Create model class in `Models/`
2. Add DbSet to `ApplicationDbContext`
3. Create repository interface in `Repositories/Interfaces/`
4. Create repository implementation in `Repositories/`
5. Create service interface in `Services/Interfaces/` (if needed)
6. Create service implementation in `Services/`
7. Create or update controller in `Controllers/`
8. Create migration: `dotnet ef migrations add <Name>`
9. Update database: `dotnet ef database update`

### Changing Database
1. Update connection string in `appsettings.json`
2. Ensure SQL Server is accessible
3. Run `dotnet ef database update`

## Troubleshooting

### Database Connection Issues
- Verify SQL Server/LocalDB is running
- Check connection string in `appsettings.json`
- Run `dotnet ef database update` to ensure schema is created

### Port Already in Use
- Change port in `Properties/launchSettings.json`
- Or specify port via command line: `dotnet run --urls "https://localhost:5002"`

### NuGet Package Errors
```bash
dotnet nuget locals all --clear
dotnet restore
```

### Migration Issues
```bash
# Remove last migration if not yet applied to database
dotnet ef migrations remove

# View migration history
dotnet ef migrations list
```

**Built with ❤️ using ASP.NET Core 10**
