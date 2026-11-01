# Theatres Module – Implementation Documentation

# 1. Overview
using Microsoft.AspNetCore.Http.HttpResults;
using MovieTicketBooking.Models;
using System.Numerics;

The Theatres module manages theatre information in the Movie Ticket Booking System.

## 2. Architecture
Controller → Service → Repository → ApplicationDbContext → SQL Server

## 3. Files Created
- Models/Theatre.cs
- DTOs/Theatres/CreateTheatreRequest.cs
- DTOs/Theatres/UpdateTheatreRequest.cs
- DTOs/Theatres/TheatreResponse.cs
- Repositories/Interfaces/ITheatreRepository.cs
- Repositories/TheatreRepository.cs
- Services/Interfaces/ITheatreService.cs
- Services/TheatreService.cs
- Services/TheatreDeleteResult.cs
- Controllers/TheatresController.cs

## 4. Features
- Create a theatre
- Retrieve all theatres
- Retrieve a theatre by ID
- Update theatre information
- Delete a theatre
- Prevent deletion when screens are associated with the theatre

## 5. Validation
- Theatre name is required and limited to 150 characters.
- Location is required and limited to 200 characters.
- Theatre ID must be greater than zero.

## 6. API Endpoints
- POST /api/theatres
- GET /api/theatres
- GET /api/theatres/{id}
-PUT / api / theatres /{ id}
-DELETE / api / theatres /{ id}

## 7. HTTP Status Codes
-200 OK – Successful retrieval or update.
- 201 Created – Theatre created.
- 204 No Content – Theatre deleted.
- 400 Bad Request – Invalid input.
- 404 Not Found – Theatre does not exist.
- 409 Conflict – Theatre has associated screens.

## 8. Database
The module uses the Theatres table through ApplicationDbContext. The Theatre-to-Screens relationship is configured as one-to-many.

## 9. Testing
Test all CRUD endpoints using Postman.

## 10. Completion
The module is complete after successful build, API testing, and Git commit.