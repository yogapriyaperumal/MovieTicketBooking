namespace MovieTicketBooking.DTOs.Bookings;

public class BookingResponse
{
    public int BookingId { get; set; }

    public string BookingReference { get; set; } = string.Empty;

    public int ShowId { get; set; }

    public string MovieTitle { get; set; } = string.Empty;

    public string ScreenName { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string CustomerEmail { get; set; } = string.Empty;

    public string CustomerPhone { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<BookingSeatResponse> Seats { get; set; } = new();
}

public class BookingSeatResponse
{
    public int SeatId { get; set; }

    public string RowName { get; set; } = string.Empty;

    public int SeatNumber { get; set; }

    public string SeatType { get; set; } = string.Empty;

    public decimal Price { get; set; }
}