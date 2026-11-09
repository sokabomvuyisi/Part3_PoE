# RaceDay – PROG6212 Part 3 (Final PoE)

ASP.NET Core race event management system with role-based interfaces for Organisers and Participants.

## Solution Structure

| Project | Description |
|---------|-------------|
| **RaceDay.Api** | RESTful API with EF Core, session auth, and Azure Blob Storage |
| **RaceDay.Contracts** | Shared request/response DTOs used by API and MVC |
| **RaceDay.Mvc** | ASP.NET Core MVC front-end with role-based UI |
| **RaceDay.Mvc/Dockerfile** | Multi-stage Docker build for the MVC app |

## Features

### Authentication & Roles
- Register / Login / Logout
- Roles: Organiser and Participant
- Session stores UserId and Role
- Protected pages redirect unauthenticated users

### Organiser
- Dashboard
- Events CRUD (create, edit, delete)
- Event banner upload (Azure Blob)
- Category management
- Enrolment status updates (Pending / Confirmed / Cancelled)
- Results entry (finish time + position)

### Participant
- Browse upcoming events with filters (type, location)
- Event detail and enrolment by category
- My Events (enrolment status)
- My Results
- Profile and profile picture upload

### Azure Blob Storage
- Containers: `event-banners`, `profile-pictures`
- All uploads go through the API only

### UI
- Sporting theme (red / navy)
- Bootstrap 5 + Bootstrap Icons
- Colour-coded enrolment statuses
- Responsive layout

## How to Run Locally

### 1. Start the API
```bash
cd RaceDay.Api
dotnet restore
dotnet run
