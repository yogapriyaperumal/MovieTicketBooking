# Phase 16 – Role-Based Authorization

## Objective
Implement role-based access control in the Movie Ticket Booking API.

## Roles
- Admin
- Customer

## Implementation
- Configured AdminOnly, CustomerOnly and AdminOrCustomer policies.
- Protected movie, theatre, screen, seat and show endpoints.
- Restricted catalogue modifications to Admin.
- Allowed Customers and Admins to view catalogue information.
- Restricted booking management endpoints.
- Enabled JWT-based role validation.

## Testing
- Tested requests without tokens.
- Tested requests with Customer tokens.
- Tested role-restricted endpoints.
- Verified 401 Unauthorized and 403 Forbidden responses.

## Outcome
The API now supports role-based authorization.

## Next Phase
Customer-specific booking ownership and access control.
