# ReelTime — Movie Ticket Booking Frontend

Angular standalone frontend created against the uploaded `MovieTicketBooking` ASP.NET Core Web API project.

## Included

- Customer registration and login (`/api/auth/register`, `/api/auth/login`)
- JWT persistence and automatic `Authorization: Bearer ...` header
- Role-aware admin route guard
- Movie catalogue, title search and movie details
- Show-time selection and live show-seat status display
- Seat selection and booking request
- Admin dashboard: create movies, theatres, screens, bulk-generate seats, schedule shows, list/cancel bookings
- Responsive dark cinema-inspired layout

## Requirements

- Node.js 22.20+ (your reported Node 22.20.0 is suitable)
- npm 10+
- Angular CLI 21 (the project was created for Angular 21)
- Your ASP.NET Core backend running at `https://localhost:7216`

## Run the frontend

Open PowerShell in this folder:

```powershell
npm cache clean --force
npm install
npm start
```

Then open `http://localhost:4200`.

If npm repeats `Cannot read properties of null (reading 'edgesOut')`, close other npm/Angular processes, then run:

```powershell
Remove-Item -Recurse -Force node_modules -ErrorAction SilentlyContinue
Remove-Item -Force package-lock.json -ErrorAction SilentlyContinue
npm cache clean --force
npm install --legacy-peer-deps
```

If the API URL changes, edit `src/environments/environment.ts`.

## Required backend configuration

### 1. Enable CORS for Angular

In `Program.cs`, register this before `var app = builder.Build();`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
```

Then add `app.UseCors("AngularFrontend");` after `app.UseHttpsRedirection();` and before `app.UseAuthentication();`:

```csharp
app.UseHttpsRedirection();
app.UseCors("AngularFrontend");
app.UseAuthentication();
app.UseAuthorization();
```

### 2. Allow customers to create bookings

Your uploaded `BookingsController` currently has `[Authorize(Policy = "AdminOnly")]` on the entire controller. That blocks customers from booking. Remove that class-level attribute and apply authorization per action as below:

```csharp
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    // Admin booking management:
    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public async Task<IActionResult> GetAll() { /* keep your existing method body */ }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) { /* keep your existing method body */ }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("reference/{reference}")]
    public async Task<IActionResult> GetByReference(string reference) { /* keep your existing method body */ }

    // Customers and admins can create a booking:
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest request) { /* keep your existing method body */ }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id) { /* keep your existing method body */ }
}
```

Keep the existing method bodies; only move/add the attributes as shown. The backend currently does not expose a customer-specific booking-history endpoint, so this frontend intentionally shows the bookings list only in the Admin dashboard.

### 3. Trust the local HTTPS development certificate

If the browser reports a certificate error when Angular calls the API, run this in an elevated or normal PowerShell window (depending on your machine's certificate permissions):

```powershell
dotnet dev-certs https --trust
```

Then visit `https://localhost:7216` once and accept/trust the local development certificate if prompted.

## API endpoints used

| Feature | Backend endpoint |
|---|---|
| Register | `POST /api/auth/register` |
| Login | `POST /api/auth/login` |
| Movie list | `GET /api/movies` (JWT required by current backend) |
| Movie search | `GET /api/movies/search?title=...` |
| Movie detail | `GET /api/movies/{id}` |
| Theatres | `GET /api/theatres` (JWT required by current backend) |
| Screens | `GET /api/screens` |
| Shows | `GET /api/shows?movieId=...` |
| Show detail | `GET /api/shows/{id}` |
| Show seats | `GET /api/show-seats/show/{showId}` |
| Create booking | `POST /api/bookings` |
| Create movie | `POST /api/movies` |
| Create theatre | `POST /api/theatres` |
| Create screen | `POST /api/screens` |
| Bulk create seats | `POST /api/seats/bulk` |
| Schedule show | `POST /api/shows` |
| Admin bookings | `GET /api/bookings` |
| Cancel booking | `PUT /api/bookings/{id}/cancel` |

## Notes

- The API DTOs in the uploaded backend do not include movie poster URLs or image uploads. The frontend therefore uses generated typographic poster artwork rather than pretending that poster images come from the API.
- Customers can browse/search movies and see showtimes. The full movie list and theatre list require a signed-in user because that is how the current backend authorization attributes are configured.
- The current booking request requires customer name, email, phone and selected `SeatIds`, which this frontend sends as-is.
- This is a frontend integration package. Database setup, Identity role seeding, backend authorization and API availability remain responsibilities of the ASP.NET Core backend.
