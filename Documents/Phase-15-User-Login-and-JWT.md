# Phase 15 – User Login and JWT Token Generation

## Objective
Implement user login and JWT access token generation.

## Implementation
- Created LoginRequest and AuthResponse DTOs.
- Validated username/email and password using ASP.NET Core Identity.
- Retrieved the user's assigned roles.
- Generated JWT access tokens.
- Added user ID, username, email and role claims.
- Configured token issuer, audience, signing key and expiration.
- Added the login endpoint to AuthController.

## API Endpoint
POST /api/auth/login

## Testing
Tested valid login, invalid password, unregistered username and email-based login.

## Outcome
Registered users can log in and receive JWT access tokens.

## Next Phase
Role-based Authorization and protection of existing API endpoints.
