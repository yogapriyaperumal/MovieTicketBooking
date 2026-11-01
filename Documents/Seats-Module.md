# Seats Module

## Overview
The Seats module manages physical seats inside theatre screens.

## Features
- Create individual seats.
- Create multiple seats in bulk.
- Retrieve all seats.
- Filter seats by screen.
- Retrieve a seat by ID.
- Update seat details.
- Delete seats without dependencies.

## Architecture
Controller → Service → Repository → ApplicationDbContext

## Business Rules
1. Every seat must belong to an existing screen.
2. Seat number must be greater than zero.
3. Row name and seat number must be unique within a screen.
4. Bulk creation validates all seats before insertion.
5. Seats associated with shows or bookings cannot be deleted.
6. Screen association cannot be changed through the update endpoint.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | /api/seats | Create seat |
| POST | /api/seats/bulk | Bulk create |
| GET | /api/seats | Get all seats |
| GET | /api/seats?screenId=1 | Filter by screen |
| GET | /api/seats/{id} | Get seat |
| PUT | /api/seats/{id} | Update seat |
| DELETE | /api/seats/{id} | Delete seat |

## Database
Table: Seats

Foreign Key:
- ScreenId references Screens.ScreenId

Unique Constraint:
- ScreenId + RowName + SeatNumber

## Testing
All endpoints should be tested using Postman.