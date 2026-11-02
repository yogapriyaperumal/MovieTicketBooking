# Phase 10 – Exception Handling and Final Testing

## 1. Objective
Implement centralized exception handling and perform end-to-end testing of the Movie Ticket Booking System.

## 2. Exception Handling
Created `GlobalExceptionMiddleware` to handle unexpected exceptions across API requests.

The middleware:
- Logs exceptions using ILogger.
- Returns HTTP 400 for argument exceptions.
- Returns HTTP 404 for missing resources.
- Returns HTTP 403 for unauthorized access.
- Returns HTTP 500 for unexpected server errors.
- Includes detailed exception messages only in Development.
- Includes a TraceId to help identify errors.

## 3. Additional Fixes
- Corrected booking reference generation to respect the database's 30-character limit.
- Prevented changing a show's screen after its ShowSeats have been generated.

## 4. Final Testing
Tested the complete workflow:
1. Create movies.
2. Create theatres.
3. Create screens.
4. Create seats.
5. Create shows.
6. Verify show-seat availability.
7. Create bookings.
8. Verify booked seats.
9. Cancel bookings.
10. Verify released seats.

## 5. Negative Testing
Test invalid inputs, duplicate seats, overlapping shows, unavailable seats, repeated cancellation, and deletion of records with dependencies.

## 6. Conclusion
The Movie Ticket Booking System backend has completed its planned modules, centralized exception handling, and end-to-end API testing.

**Technology Stack:** ASP.NET Core Web API, .NET 10, Entity Framework Core 10, SQL Server, and Postman.
