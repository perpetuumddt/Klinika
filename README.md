# Klinika - E-Medical Management System

An ASP.NET Core web application for managing medical system and databases including: patients data and medical records, doctor information, and appointment scheduling in healthcare facilities.

## 📋 Overview

**Klinika** is a full-featured medical information system designed to streamline healthcare facility operations. The system provides an intuitive interface for managing patient records, scheduling appointments, tracking medical history, and coordinating healthcare provider activities.

### Purpose

- Digitalize and centralize medical facility management
- Simplify appointment scheduling and patient tracking
- Provide healthcare providers with quick access to patient information
- Maintain medical histories

### Key Features

- **Patient Management**: Complete patient profile management with demographic information, medical history, and insurance details
- **Doctor Management**: Healthcare provider profiles with specializations, qualifications, and availability tracking
- **Appointment Scheduling**: Intelligent scheduling system with conflict detection and status tracking
- **Medical Records**: Comprehensive medical documentation including diagnoses, treatments, prescriptions, and recommendations
- **Cross-platform Testing**: Multi-OS deployment testing using Vagrant (Ubuntu, Debian, Windows)
- **NuGet Package Distribution**: Internal package repository using BaGet for application deployment

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) / [Rider](https://www.jetbrains.com/rider/) / [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/klinika.git
   cd klinika
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string**
   
   Edit `Klinika/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=KlinikaDb;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Apply database migrations**
   ```bash
   cd Klinika
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the application**
   
   Open your browser and navigate to:
   - HTTP: `http://localhost:5042`
   - HTTPS: `https://localhost:7273`

## 📦 Dependencies

### Core Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.0 | SQL Server database provider |
| `Microsoft.EntityFrameworkCore.Tools` | 9.0.0 | EF Core migration tools |
| `Microsoft.EntityFrameworkCore.Design` | 9.0.0 | Design-time EF Core components |
| `Microsoft.AspNetCore.Mvc.NewtonsoftJson` | 9.0.10 | JSON serialization |

### Testing Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `xUnit` | 2.9.3 | Unit testing framework |
| `Microsoft.NET.Test.Sdk` | 18.0.0 | Test SDK |
| `FluentAssertions` | 8.7.1 | Fluent assertion library |
| `Moq` | 4.20.72 | Mocking framework |
| `Microsoft.EntityFrameworkCore.InMemory` | 9.0.10 | In-memory database for testing |
| `Microsoft.AspNetCore.Mvc.Testing` | 9.0.10 | Integration testing |

## 🧪 Testing

The project includes unit and integration tests using xUnit.

### Running Tests

**Run all tests:**
```bash
dotnet test
```

**Run tests with detailed output:**
```bash
dotnet test --verbosity detailed
```

### Test Coverage

- **Controllers**: CRUD operations, validation, error handling
- **Models**: Data annotations, properties, business logic

## 🔧 Vagrant Multi-OS Testing

The project includes Vagrant configuration for cross-platform testing and deployment project as NuGet package via BaGet server.

### Virtual Machines

| VM | OS | Port |
|----|----|----|---------|
| `ubuntu_vm` | Ubuntu 22.04 | 5555 |
| `debian` | Debian 12 | 5556 |
| `windows` | Windows Server 2022 | 5557 |

### Vagrant Commands


**Start all VMs:**
```bash
vagrant up
```

**Start specific VM:**
```bash
vagrant up ubuntu_vm
# or
vagrant up debian
# or
vagrant up windows
```

**Check VM status:**
```bash
vagrant status
```

**SSH into VM:**
```bash
vagrant ssh ubuntu_vm
```

**Stop VMs:**
```bash
vagrant halt
```

**Destroy VMs:**
```bash
vagrant destroy -f
```

### BaGet NuGet Server

Each VM runs a local BaGet server for testing NuGet package distribution:

### What Vagrant Does

1. **Installs .NET SDK** (.NET 3.1 for BaGet, .NET 9.0 for client)
2. **Downloads and configures BaGet** NuGet server
3. **Creates test NuGet package** (KlinikaApp v0.0.1)
4. **Publishes package** to local BaGet repository
5. **Tests package installation** in a sample console application
6. **Configures NuGet sources** with HTTP support for local development

## 📁 Project Structure

```
Klinika/
├── Controllers/          # MVC Controllers
├── Data/                # Database context and configurations
├── Models/              # Entity models
├── Views/               # Razor views
├── Migrations/          # EF Core migrations
├── wwwroot/            # Static files
└── appsettings.json    # Application configuration

Klinika.Tests/
├── Controllers/         # Controller tests
├── Models/             # Model tests
└── Data/               # Test utilities

Vagrantfile             # Multi-OS testing configuration
.gitignore             # Git ignore rules
```

## 🔐 Database

### Entities

- **Patients**: Patient information
- **Doctors**: Healthcare provider profiles
- **Appointments**: Scheduled patient-doctor meetings
- **MedicalRecords**: Patient medical card

### Key Relationships

- `Patient` 1:N `Appointment` (one patient, many appointments)
- `Doctor` 1:N `Appointment` (one doctor, many appointments)
- `Patient` 1:N `MedicalRecord` (one patient, many records)
- `Doctor` 1:N `MedicalRecord` (one doctor, many records)

## API Endpoints

### Patients
- `GET /Patients` - List all patients
- `GET /Patients/Details/{id}` - View patient details
- `GET /Patients/Create` - Create patient form
- `POST /Patients/Create` - Submit new patient
- `GET /Patients/Edit/{id}` - Edit patient form
- `POST /Patients/Edit/{id}` - Update patient
- `GET /Patients/Delete/{id}` - Delete confirmation
- `POST /Patients/Delete/{id}` - Delete patient

### Doctors
- `GET /Doctors` - List all doctors
- `GET /Doctors/Details/{id}` - View doctor details
- `GET /Doctors/Create` - Create doctor form
- `POST /Doctors/Create` - Submit new doctor
- `GET /Doctors/Edit/{id}` - Edit doctor form
- `POST /Doctors/Edit/{id}` - Update doctor
- `GET /Doctors/Delete/{id}` - Delete confirmation
- `POST /Doctors/Delete/{id}` - Delete doctor

### Appointments
- `GET /Appointments` - List all appointments
- `GET /Appointments/Details/{id}` - View appointment details
- `GET /Appointments/Create` - Create appointment form
- `POST /Appointments/Create` - Submit new appointment
- `GET /Appointments/Edit/{id}` - Edit appointment form
- `POST /Appointments/Edit/{id}` - Update appointment
- `GET /Appointments/Delete/{id}` - Delete confirmation
- `POST /Appointments/Delete/{id}` - Delete appointment

### Medical Records
- `GET /MedicalRecords` - List all records
- `GET /MedicalRecords/Index?patientId={id}` - Filter by patient
- `GET /MedicalRecords/Details/{id}` - View record details
- `GET /MedicalRecords/Create?patientId={id}` - Create record form
- `POST /MedicalRecords/Create` - Submit new record
- `GET /MedicalRecords/Edit/{id}` - Edit record form
- `POST /MedicalRecords/Edit/{id}` - Update record
- `GET /MedicalRecords/Delete/{id}` - Delete confirmation
- `POST /MedicalRecords/Delete/{id}` - Delete record

## 👤 Authors

**Danylo Tielnoi**, **Ivan Voitenko**
