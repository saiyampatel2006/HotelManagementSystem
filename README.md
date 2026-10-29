# The Aurelia Grand — Hotel Management System
**Academic Capstone / Semester Project for 3rd-Year B.Tech Computer Science & Engineering (AIML)**

---

## 🏛️ Project Overview

**The Aurelia Grand** is an enterprise-grade Hotel Management System built with **ASP.NET Core 8 MVC**, **C# 12**, **Entity Framework Core 8**, **Bootstrap 5**, and **SQL Server**. 

Designed with an aesthetic reminiscent of Forbes 5-Star European luxury resorts, the system features a light ivory, crisp white, warm beige, and royal gold visual identity. Beyond standard room booking, this project incorporates an **AI & Machine Learning-inspired Dynamic Pricing & Demand Forecasting Engine** tailored to showcase both advanced software engineering and AIML domain capabilities during semester evaluations and viva voce examinations.

---

## 💎 Design & Visual Philosophy

- **Color Palette**: Light Ivory (`#FCFBF7`), Crisp White (`#FFFFFF`), Warm Sand Beige (`#EFE9DF`), and Royal Gold Accents (`#B89354`). **No dark theme** — designed specifically for pristine luxury ambiance.
- **Typography**: Google Fonts — *Playfair Display* for classical serif headlines and titles; *Plus Jakarta Sans* for clean, modern readability.
- **Visuals**: Real high-resolution curated photography of presidential suites, infinity plunge villas, skyline balconies, and Michelin dining.
- **Micro-Interactions**: Dynamic room card hover elevates, ribbon status badges, interactive date pickers, real-time price calculators, and non-intrusive Bootstrap 5 toast notifications.

---

## 🚀 Key Functional Modules

### 1. Public & Guest Experience
- **Interactive Homepage**: Hero banner with live availability search bar, luxury distinction metrics, curated suites catalog, hotel amenities, and verified guest reviews.
- **Suite Search & Filtering**: Filter by check-in / check-out dates, suite categories, guest party size, maximum nightly tariff, specific facilities, and price sorting.
- **Suite Details**: Full gallery, suite specifications (floor level, maximum occupancy, current operational status), complete list of amenities with icons, and guest review submission.
- **Intelligent Booking Wizard**:
  - Live JavaScript date validation (check-in $\ge$ today, check-out $>$ check-in).
  - Dynamic night calculation and tax breakdown (12% State Luxury Cess / GST).
- **Overlapping Reservation Prevention**: Server-side LINQ check ensures no two reservations can overlap for the same suite:
  ```csharp
  // Overlap condition: (checkIn < b.CheckOutDate && checkOut > b.CheckInDate)
  bool isAvailable = await _bookingService.IsRoomAvailableAsync(model.RoomId, model.CheckInDate, model.CheckOutDate);
  ```
- **Simulated Payment Gateway**: Supports Demo Credit/Debit Card (with 1-click test fill helper), UPI / QR Code transfer, and Pay on Arrival at the Concierge desk.
- **Guest Portal (My Reservations)**: View active and past stays, inspect official tax invoices, and cancel upcoming reservations with automated refund tracking.
- **Patron Profile**: Edit contact details, view registration date, and monitor reservation count.

### 2. Management & Concierge Operations (Role-Based Admin Access)
- **Executive Dashboard**:
  - Real-time KPI counters: Gross Revenue, Active In-House Guests, Total Reservations, Available Suites, and Occupancy Rate.
  - **Chart.js Visualizations**: 6-month revenue trend bar chart and suite category demand doughnut distribution.
- **Room Inventory CRUD**:
  - Create new luxury suites with custom numbers, categories, tariffs, floor level, photo URLs, and multi-select amenities.
  - Edit suite specifications.
  - Soft/Hard delete protection (safeguards rooms with active reservations by shifting them to `Maintenance`).
  - **1-Click Status Toggles**: Immediately switch suite status between `Available`, `Booked`, `Maintenance`, and `Housekeeping/Cleaning`.
- **Desk Operations & Booking Management**:
  - Live table of all reservations with search by guest, room, or booking code (`AUR-XXXXXX`).
  - **Check-In Guest Action**: Issues digital keycards and marks suite as `Booked`.
  - **Check-Out Guest Action**: Completes stay and automatically routes suite to `Housekeeping/Cleaning`.
  - **Management Override Cancellation**: Permits admin cancellation with recorded justification.
- **Guest Directory**: View registered customers, emails, contact numbers, and stay frequencies.
- **Financial & Audited Reports**:
  - Date-range analytics calculating Gross Revenue, Luxury Taxes Collected, and Net Yield.
  - Category-by-category revenue share percentages.
  - **Export to CSV**: Downloadable spreadsheet ledger for auditing and accounting.
  - **Printable Dossier**: Formatted for physical print / PDF generation.

### 3. AIML Specialization Highlight
- **Algorithmic Suite Recommendation Engine**: Analyzes party composition (adults, children, length of stay) to recommend the optimal accommodation (Family Plunge Villa vs. Couples Penthouse vs. Corporate Skyline Suite).
- **Dynamic Pricing & Demand Index**: Algorithmic multiplier factoring in seasonal velocity, weekend demand, and current hotel occupancy.
- **Occupancy Forecasting Module**: Predictive heuristic alert featured on the Admin Dashboard advising management on rack rate adjustments and promotional upgrades.

---

## 🛠️ Technology Stack

| Layer | Technologies |
|---|---|
| **Framework** | ASP.NET Core 8.0 MVC |
| **Language** | C# 12 (.NET 8 SDK) |
| **Database** | SQL Server / SQLite (Dual-Provider Supported) |
| **ORM** | Entity Framework Core 8.0 (Code-First with Migrations) |
| **Security & Auth** | ASP.NET Core Identity (Role-Based: `Admin` and `Customer`) |
| **Frontend** | Razor Views (`.cshtml`), HTML5, CSS3, JavaScript (ES6) |
| **CSS Framework** | Bootstrap 5.3 + Bootstrap Icons 1.11 |
| **Data Visualization** | Chart.js 4.4 |

---

## 🔑 Default Credentials (Pre-Seeded)

The system automatically initializes and seeds the database upon first startup:

| Role | Email | Password | Permissions |
|---|---|---|---|
| **Administrator** | `admin@grandaurelia.com` | `Admin@123` | Full management dashboard, Room CRUD, Check-in/out, Reports, CSV export |
| **Customer / Patron** | `customer@gmail.com` | `Customer@123` | Browse suites, reserve rooms, simulated checkout, view vouchers, cancel bookings |

> 💡 **Convenience Feature**: The login screen (`/Account/Login`) includes **1-click auto-fill buttons** for both Admin and Customer accounts for effortless evaluation during project presentations!

---

## 🗄️ Database Setup Instructions

The application supports **both SQL Server and SQLite** via a single configuration toggle in [appsettings.json](file:///C:/Users/MANAN/.gemini/antigravity/scratch/HotelManagementSystem/appsettings.json).

### Mode A: Zero-Configuration Instant Run (SQLite - Default)
- By default, `"DatabaseProvider": "Sqlite"` is enabled in `appsettings.json`.
- Requires **no local SQL Server installation or database creation**.
- When you run the project, EF Core creates `GrandAureliaHotel.db` and populates all 8 luxury suites, 10 facilities, users, sample reservations, and payments automatically!

### Mode B: Microsoft SQL Server / LocalDB
To connect to Microsoft SQL Server:
1. Open `appsettings.json`.
2. Update `"DatabaseProvider": "SqlServer"`.
3. Verify your connection string:
   ```json
   "ConnectionStrings": {
     "SqlServerConnection": "Server=localhost;Database=GrandAureliaHotelDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```
4. Run EF Core migrations to create the schema:
   ```bash
   dotnet ef database update
   ```
   *(Alternatively, run the pre-generated [sqlserver_schema.sql](file:///C:/Users/MANAN/.gemini/antigravity/scratch/HotelManagementSystem/sqlserver_schema.sql) script in SQL Server Management Studio).*

---

## 💻 How to Run in Visual Studio Code

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed (`dotnet --version` should return `8.0.x`).
- [Visual Studio Code](https://code.visualstudio.com/) with the **C# Dev Kit** or **C# extension** installed.

### Steps
1. Open VS Code and select **File $\rightarrow$ Open Folder...**
2. Choose:
   ```
   C:\Users\MANAN\.gemini\antigravity\scratch\HotelManagementSystem
   ```
3. Open the integrated terminal (`Ctrl + ~` or **Terminal $\rightarrow$ New Terminal**).
4. Build the project:
   ```bash
   dotnet build
   ```
5. Run the web application:
   ```bash
   dotnet run
   ```
6. Open your web browser and navigate to:
   ```
   http://localhost:5172
   ```
   *(Or press `F5` in VS Code to launch with the included `.vscode/launch.json` debugger).*

---

## 📁 Solution Architecture

```
HotelManagementSystem/
├── Controllers/
│   ├── HomeController.cs        # Landing page, About, Concierge contact, Privacy
│   ├── RoomsController.cs       # Suite catalog, date/price filtering, reviews, AI match
│   ├── BookingsController.cs    # Reservation wizard, overlap checks, vouchers, cancellation
│   ├── PaymentsController.cs    # Payment terminal simulator, receipt generation
│   ├── AccountController.cs     # Identity register, login, patron profile, access control
│   ├── AdminController.cs       # Operations dashboard, Room CRUD, Check-in/out desk
│   └── ReportsController.cs     # Ledger analytics, date filters, CSV data exporter
├── Data/
│   ├── ApplicationDbContext.cs  # EF Core context, relationships, precision mappings
│   └── DbInitializer.cs         # Auto-seeding for roles, credentials, suites, bookings
├── Migrations/                  # EF Core database migration snapshots
├── Models/
│   ├── Entities/                # Relational domain entities
│   │   ├── ApplicationUser.cs   # Inherits IdentityUser + FullName, Address
│   │   ├── RoomType.cs          # Categories (Deluxe, Executive, Penthouse, Villa)
│   │   ├── Facility.cs          # Amenities (Wi-Fi 6, OLED TV, Jacuzzi, Butler, etc.)
│   │   ├── Room.cs              # Suites with capacity, floor, tariff, status
│   │   ├── RoomFacility.cs      # Many-to-many join entity
│   │   ├── Booking.cs           # Reservation record with date intervals & statuses
│   │   ├── Payment.cs           # Transaction reference, method, status
│   │   └── Review.cs            # Star rating and feedback commentary
│   └── ViewModels/              # Strongly-typed models with DataAnnotation validation
├── Services/                    # Decoupled business logic layer (Dependency Injection)
│   ├── IBookingService.cs & BookingService.cs
│   ├── IRoomService.cs & RoomService.cs
│   └── IAiPricingService.cs & AiPricingService.cs
├── Views/                       # Clean Razor (.cshtml) presentation templates
│   ├── Shared/                  # _Layout.cshtml (Ivory/Gold theme), _LoginPartial, _Toast
│   ├── Home/                    # Index, About, Contact, Privacy
│   ├── Rooms/                   # Index (Catalogue), Details
│   ├── Bookings/                # Create, Confirmation, MyBookings, Details
│   ├── Payments/                # Checkout, Receipt
│   ├── Account/                 # Login, Register, Profile, AccessDenied
│   ├── Admin/                   # Index (Dashboard), Rooms, CreateRoom, EditRoom, Bookings, Customers
│   └── Reports/                 # Index (Analytics & CSV export)
├── wwwroot/
│   ├── css/luxury-theme.css     # Tailored ivory, beige & gold palette styles
│   └── js/hotel-scripts.js      # Dynamic price calculator, toast init, demo auto-fill
├── .vscode/                     # VS Code launch & task configurations
├── appsettings.json             # Configuration & connection strings
├── sqlserver_schema.sql         # SQL script export of complete relational schema
└── HotelManagementSystem.csproj # .NET 8 Web SDK project configuration
```

---

## 🎓 College Presentation & Viva Voce Guide

### Q1: How does your system prevent double-booking of suites?
> **Answer**: In `BookingService.IsRoomAvailableAsync()`, we evaluate any existing active booking for the target room where `Status != BookingStatus.Cancelled`. An overlap occurs when the requested check-in is before an existing check-out, AND the requested check-out is after the existing check-in:
> ```csharp
> checkIn < b.CheckOutDate.Date && checkOut > b.CheckInDate.Date
> ```
> This check executes both during search filtering and strictly on the server during `POST /Bookings/Create` inside a transactional context.

### Q2: How is role-based authorization implemented?
> **Answer**: We use ASP.NET Core Identity with `IdentityRole`. When users register, they are assigned the `Customer` role by default. Management controllers and action methods are guarded with `[Authorize(Roles = "Admin")]`. Non-authorized attempts are automatically intercepted and redirected to `/Account/AccessDenied`.

### Q3: How does the AIML feature relate to your degree?
> **Answer**: In `AiPricingService`, we demonstrate predictive heuristics for hotel revenue management. It incorporates a **party cluster classifier** (distinguishing between corporate solo travelers, couples on retreat, and multi-guest family parties) to optimize suite allocation, alongside a **dynamic pricing coefficient** based on real-time room inventory occupancy velocity.

---

## 📜 Academic Integrity & License
This project was engineered for academic demonstration purposes in accordance with third-year B.Tech CSE (AIML) university guidelines.
All photographic assets are sourced via Unsplash under royalty-free editorial licenses.

#   H O T E L - - M A N A G E M E N T - S Y S T E M  
 