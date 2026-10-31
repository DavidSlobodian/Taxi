# Taxi API

A backend REST API for a taxi booking platform built with **ASP.NET Core and .NET 8**. The application provides functionality for passengers, drivers, and administrators, including ride booking, driver management, ride matching, real-time notifications, and rating management.

## Overview

Taxi API is designed to handle the main processes of a taxi service:

- Passenger registration and authentication.
- Driver registration and application approval.
- Vehicle management.
- Ride creation and cancellation.
- Finding nearby available drivers.
- Real-time ride offers and status updates.
- Ride lifecycle management.
- Passenger and driver ratings.
- Administrative operations and monitoring.

The project follows a layered architecture to separate business logic, data access, domain entities, and API endpoints.

## Tech Stack

### Backend
- **C#**
- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core**

### Database & Storage
- **Microsoft SQL Server** — relational database.
- **Redis** — geospatial driver location storage and fast location-based queries.

### Authentication & Security
- **JWT (JSON Web Tokens)** — authentication and authorization.
- **Role-based authorization** — Passenger, Driver, and Admin roles.
- Password hashing.
- Request validation.

### Real-time Communication
- **SignalR** — real-time communication between the server and connected clients.
- Redis GEO — storing driver coordinates and finding nearby drivers.

### Additional Technologies & Tools
- **FluentValidation** — request and DTO validation.
- **Swagger / OpenAPI** — API documentation and testing.
- **Dependency Injection** — service registration and dependency management.
- **Middleware** — centralized exception handling and standardized API responses.
- **Background Services** — handling background ride-matching processes.

## Architecture

The solution is organized into four main projects:

```text
Taxi/
├── Taxi.Api/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Hubs/
│   └── Program.cs
│
├── Taxi.Application/
│   ├── Services/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Validators/
│
├── Taxi.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── Exceptions/
│
└── Taxi.Infrastructure/
    ├── Data/
    ├── Repositories/
    ├── Redis/
    └── Services/
```

### Project Responsibilities

| Project | Responsibility |
|---|---|
| **Taxi.Api** | HTTP endpoints, controllers, middleware, SignalR hubs |
| **Taxi.Application** | Business logic, application services, DTOs, validation |
| **Taxi.Domain** | Core entities, enums, and domain rules |
| **Taxi.Infrastructure** | Database access, repositories, Redis, external infrastructure |

## Core Features

### 1. Authentication & Authorization
- User registration and login.
- JWT-based authentication.
- Role-based access control.
- Password hashing.
- Separate permissions for passengers, drivers, and administrators.

### 2. Driver Management
- Driver profile management.
- Driver application submission and approval.
- License information management.
- Vehicle registration and management.
- Online and availability status tracking.

### 3. Ride Management
- Creating ride requests.
- Managing ride statuses.
- Accepting and cancelling rides.
- Tracking important ride lifecycle timestamps.
- Passenger and driver-specific ride operations.

### 4. Price Calculation
- Price varies depending on weather conditions, fuel price and distance

### 5. Driver Matching System
- Redis GEO-based driver location storage.
- Searching for nearby available drivers.
- Progressive search radius expansion.
- Preventing repeated ride offers to the same driver.
- Real-time ride offers through SignalR.

### 6. Real-time Communication
- SignalR-based communication.
- Sending ride offers to drivers.
- Updating ride-related information in real time.
- Supporting real-time interactions between the API and connected clients.

### 7. Ratings & Reviews
- Passengers can rate drivers.
- Drivers can rate passengers.
- Ratings are associated with completed rides.
- Rating scores, tags, and comments.
- Preventing duplicate ratings from the same participant for one ride.

### 8. Administration
- Driver and ride management.
- Ride monitoring.
- Complaint management.
- Tariff and pricing management.
- Administrative operations and reporting.

## API Response Handling

The API uses a standardized response structure to keep successful and failed responses consistent.

Example:

```json
{
  "success": true,
  "data": {
    "id": "00000000-0000-0000-0000-000000000000"
  },
  "error": null
}
```

A centralized exception-handling middleware processes application exceptions and returns appropriate HTTP status codes.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Microsoft SQL Server
- Redis
- Visual Studio or another IDE supporting .NET

### Configuration

Configure the required connection strings and authentication settings in `appsettings.json` or through environment variables.

Example:

```json
{
  "ConnectionStrings": {
    "TaxiConStr": "YOUR_SQL_SERVER_CONNECTION_STRING"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY",
    "Issuer": "YOUR_ISSUER",
    "Audience": "YOUR_AUDIENCE"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

Use your own credentials and never commit real secrets to source control.

### Database Setup

Apply Entity Framework Core migrations:

```bash
dotnet ef database update --project Taxi.Infrastructure --startup-project Taxi.Api
```

### Run the Application

```bash
dotnet restore
dotnet build
dotnet run --project Taxi.Api
```

Once the application is running, open the Swagger UI to explore and test available endpoints.

## Project Goals

This project was developed to practice and demonstrate backend development skills using modern .NET technologies, including:

- Building RESTful APIs.
- Designing a layered backend architecture.
- Working with relational databases and Entity Framework Core.
- Implementing JWT authentication and role-based authorization.
- Using Redis for geospatial queries.
- Implementing real-time communication with SignalR.
- Designing business logic for a ride-booking system.
- Handling exceptions and API responses consistently.
- Working with asynchronous operations and background processes.

---

**Taxi API** — a backend project focused on building a scalable taxi booking system with .NET 8.
