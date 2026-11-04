# Backend changes needed for the Angular frontend

The uploaded backend runs at `https://localhost:7216` and currently has no CORS registration in `Program.cs`.

## CORS

Add before `builder.Build()`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularFrontend", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});
```

Add after HTTPS redirection and before authentication:

```csharp
app.UseCors("AngularFrontend");
```

## Booking authorization

`BookingsController` currently uses `[Authorize(Policy = "AdminOnly")]` at controller level. Remove it from the class. Put `AdminOnly` on GET/list/detail/reference and cancel actions, and put `AdminOrCustomer` on the POST create action. Retain the existing action bodies.

The current backend does not have a customer booking-history endpoint. The frontend's booking list is therefore admin-only, matching the API as supplied.
