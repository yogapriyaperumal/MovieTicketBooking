# Phase 14 – User Registration

## Objective
Implement customer registration using ASP.NET Core Identity.

## Implementation
- Created RegisterRequest and RegisterResponse DTOs.
- Updated ApplicationUser with FullName.
- Implemented registration using UserManager.
- Added email and username duplicate checks.
- Used Identity password hashing.
- Automatically assigned the Customer role.
- Created AuthController registration endpoint.
- Registered AuthService using dependency injection.

## API Endpoint
POST /api/auth/register

## Database Changes
Added FullName to AspNetUsers using an EF Core migration.

## Testing
Tested valid registration, duplicate email, duplicate username, invalid email and weak password.

## Outcome
Customers can register and are automatically assigned the Customer role.

## Next Phase
JWT Login and Token Generation.
