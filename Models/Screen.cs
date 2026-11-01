namespace MovieTicketBooking.Models
{
    public class Screen
    {
        public int ScreenId { get; set; }

        public int TheatreId { get; set; }

        public string ScreenName { get; set; } = string.Empty;

        // Each screen belongs to one theatre.
        public Theatre? Theatre { get; set; }

        // One screen can contain multiple seats.
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();

        // One screen can have multiple shows.
        public ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}
