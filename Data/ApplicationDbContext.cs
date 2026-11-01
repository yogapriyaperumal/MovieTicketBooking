using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Database tables
    public DbSet<Movie> Movies { get; set; }

    //public DbSet<Theatre> Theatres { get; set; }

    
    //public DbSet<Seat> Seats { get; set; }

    public DbSet<Show> Shows { get; set; }

    //public DbSet<ShowSeat> ShowSeats { get; set; }

    //public DbSet<Booking> Bookings { get; set; }

    //public DbSet<BookingSeat> BookingSeats { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================
        // MOVIE CONFIGURATION
        // =====================================

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(m => m.MovieId);

            entity.Property(m => m.Title)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(m => m.Description)
                .HasMaxLength(500);

            entity.Property(m => m.DurationMinutes)
                .IsRequired();

            entity.Property(m => m.ReleaseDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(m => m.Language)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(m => m.Genre)
                .HasMaxLength(50)
                .IsRequired();

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Movies_DurationMinutes",
                    "[DurationMinutes] > 0");
            });
        });


        // =====================================
        // THEATRE CONFIGURATION
        // =====================================

        /*modelBuilder.Entity<Theatre>(entity =>
        {
            entity.HasKey(t => t.TheatreId);

            entity.Property(t => t.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(t => t.Location)
                .HasMaxLength(200)
                .IsRequired();
        });*/


        // =====================================
        // SCREEN CONFIGURATION
        // =====================================

        /*modelBuilder.Entity<Screen>(entity =>
        {
            entity.HasKey(s => s.ScreenId);

            entity.Property(s => s.ScreenName)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(s => new
            {
                s.TheatreId,
                s.ScreenName
            }).IsUnique();

            entity.HasOne(s => s.Theatre)
                .WithMany(t => t.Screens)
                .HasForeignKey(s => s.TheatreId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        // =====================================
        // SEAT CONFIGURATION
        // =====================================

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(s => s.SeatId);

            entity.Property(s => s.RowName)
                .HasMaxLength(5)
                .IsRequired();

            entity.Property(s => s.SeatNumber)
                .IsRequired();

            entity.Property(s => s.SeatType)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(s => new
            {
                s.ScreenId,
                s.RowName,
                s.SeatNumber
            }).IsUnique();

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Seats_SeatNumber",
                    "[SeatNumber] > 0");
            });

            entity.HasOne(s => s.Screen)
                .WithMany(sc => sc.Seats)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        

        // =====================================
        // SHOW CONFIGURATION
        // =====================================

        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(s => s.ShowId);

            entity.Property(s => s.StartTime)
                .IsRequired();

            entity.Property(s => s.EndTime)
                .IsRequired();

            entity.Property(s => s.TicketPrice)
                .HasPrecision(10, 2)
                .IsRequired();

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Shows_EndTime",
                    "[EndTime] > [StartTime]");

                table.HasCheckConstraint(
                    "CK_Shows_TicketPrice",
                    "[TicketPrice] >= 0");
            });

            entity.HasOne(s => s.Movie)
                .WithMany(m => m.Shows)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Screen)
                .WithMany(sc => sc.Shows)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        
        // =====================================
        // SHOW SEAT CONFIGURATION
        // =====================================

        modelBuilder.Entity<ShowSeat>(entity =>
        {
            entity.HasKey(ss => ss.ShowSeatId);

            entity.Property(ss => ss.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(ss => new
            {
                ss.ShowId,
                ss.SeatId
            }).IsUnique();

            entity.HasOne(ss => ss.Show)
                .WithMany(s => s.ShowSeats)
                .HasForeignKey(ss => ss.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ss => ss.Seat)
                .WithMany(s => s.ShowSeats)
                .HasForeignKey(ss => ss.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        // =====================================
        // BOOKING CONFIGURATION
        // =====================================

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.BookingId);

            entity.Property(b => b.BookingReference)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(b => b.BookingReference)
                .IsUnique();

            entity.Property(b => b.CustomerName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(b => b.CustomerEmail)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(b => b.CustomerPhone)
                .HasMaxLength(15)
                .IsRequired();

            entity.Property(b => b.BookingDate)
                .IsRequired();

            entity.Property(b => b.TotalAmount)
                .HasPrecision(10, 2)
                .IsRequired();

            entity.Property(b => b.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Bookings_TotalAmount",
                    "[TotalAmount] >= 0");
            });

            entity.HasOne(b => b.Show)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ShowId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        // =====================================
        // BOOKING SEAT CONFIGURATION
        // =====================================

        modelBuilder.Entity<BookingSeat>(entity =>
        {
            entity.HasKey(bs => bs.BookingSeatId);

            entity.Property(bs => bs.Price)
                .HasPrecision(10, 2)
                .IsRequired();

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_BookingSeats_Price",
                    "[Price] >= 0");
            });

            entity.HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSeats)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(bs => bs.Seat)
                .WithMany(s => s.BookingSeats)
                .HasForeignKey(bs => bs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
        });*/
    }
}