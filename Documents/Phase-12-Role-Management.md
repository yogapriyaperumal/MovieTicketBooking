# Phase 12 – Role Management

## Objective
Create role-based access management using ASP.NET Core Identity.

## Roles Implemented
1. Admin – Manages movies, theatres, screens, seats and shows.
2. Customer – Views movies and shows and manages bookings.

## Implementation
- Created RoleSeeder.
- Used RoleManager to manage roles.
- Added automatic role creation during application startup.
- Prevented duplicate role creation by checking whether each role already exists.

## Database
Roles are stored in the AspNetRoles table.

## Outcome
The application now supports Admin and Customer roles.

## Next Phase
JWT Authentication Configuration.
