namespace MovieTicketBooking.Models
{
    public class Show
    {
        public int ShowId { get; set; }

        public int MovieId { get; set; }

        public DateTime ShowDateTime { get; set; }

        public decimal TicketPrice { get; set; }

        public int TotalSeats { get; set; }

        // Navigation property
        public Movie Movie { get; set; } = null!;
    }
}
