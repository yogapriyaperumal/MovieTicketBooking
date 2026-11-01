# ShowSeats Module

## Overview
Manages seat availability for individual movie shows.

## Features
- Retrieve all seats for a show.
- Retrieve available seats.
- Retrieve booked seats.
- Retrieve ShowSeat by ID.

## Architecture
Controller → Service → Repository → ApplicationDbContext

## Business Rules
1. ShowSeats are generated automatically when a show is created.
2. Every generated ShowSeat starts with Available status.
3. Booking changes status to Booked.
4. Cancellation changes status back to Available.
5. Status changes are handled internally by the booking workflow.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | /api/show-seats/show/{showId} | Get all seats |
| GET | /api/show-seats/show/{showId}/available | Get available seats |
| GET | /api/show-seats/show/{showId}/booked | Get booked seats |
| GET | /api/show-seats/{id} | Get ShowSeat |

## Database
Table: ShowSeats

Unique Constraint:
- ShowId + SeatId

## Testing
All GET endpoints should be tested using Postman.