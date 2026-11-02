# Phase 11 – Identity Foundation

## Objective
Integrate ASP.NET Core Identity into the existing Movie Ticket Booking System.

## Implementation
- Installed Microsoft.AspNetCore.Identity.EntityFrameworkCore.
- Created the ApplicationUser model.
- Updated ApplicationDbContext to inherit from IdentityDbContext.
- Preserved existing entity configurations and DbSets.
- Configured Identity password requirements and unique email validation.
- Connected Identity to SQL Server using Entity Framework Core.

## Database Changes
Added ASP.NET Core Identity tables to the existing MovieTicketBookingDB.

## Outcome
The project now has the foundation for user registration, login, role management and authentication.

## Next Phase
Admin and Customer role creation and seeding.
