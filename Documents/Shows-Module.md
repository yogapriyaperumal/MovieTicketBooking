# Shows Module

## Overview
The Shows module manages movie screening schedules.

## Features
- Create shows.
- Retrieve all shows.
- Filter shows by movie and screen.
- Retrieve a show by ID.
- Update show schedules.
- Delete shows without bookings.
- Automatically generate ShowSeats.

## Architecture
Controller → Service → Repository → ApplicationDbContext

## Business Rules
1. Movie must exist.
2. Screen must exist.
3. EndTime must be later than StartTime.
4. TicketPrice cannot be negative.
5. Shows on the same screen cannot overlap.
6. Shows with bookings cannot be deleted.
7. Creating a show generates Available ShowSeats for its screen.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | /api/shows | Create show |
| GET | /api/shows | Get all shows |
| GET | /api/shows/{id} | Get show |
| GET | /api/shows/movie/{movieId} | Filter by movie |
| GET | /api/shows/screen/{screenId} | Filter by screen |
| PUT | /api/shows/{id} | Update show |
| DELETE | /api/shows/{id} | Delete show |

## Database
Table: Shows

Foreign Keys:
- MovieId references Movies.MovieId
- ScreenId references Screens.ScreenId

## Testing
All endpoints should be tested using Postman.