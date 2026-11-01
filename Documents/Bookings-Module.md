# Bookings Module

## Overview
The Bookings module manages movie ticket reservations.

## Features
- Create bookings.
- Retrieve all bookings.
- Retrieve booking by ID.
- Retrieve booking using booking reference.
- Cancel confirmed bookings.
- Automatically calculate booking amount.
- Automatically update seat availability.

## Architecture
Controller → Service → Repository → ApplicationDbContext

## Business Rules
1. Show must exist.
2. At least one seat must be selected.
3. Duplicate seat IDs are not allowed.
4. All selected seats must belong to the requested show.
5. Only Available seats can be booked.
6. Total amount is calculated on the server.
7. Booking and BookingSeats are created in a transaction.
8. Selected ShowSeats are marked Booked.
9. Only Confirmed bookings can be cancelled.
10. Cancellation releases the seats.
11. Cancelled booking records are preserved.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | /api/bookings | Create booking |
| GET | /api/bookings | Get all bookings |
| GET | /api/bookings/{id} | Get booking |
| GET | /api/bookings/reference/{reference} | Get by reference |
| PUT | /api/bookings/{id}/cancel | Cancel booking |

## Database Tables
- Bookings
- BookingSeats
- ShowSeats

## Transaction Handling
Booking creation and cancellation use database transactions.

## Testing
All endpoints should be tested using Postman.