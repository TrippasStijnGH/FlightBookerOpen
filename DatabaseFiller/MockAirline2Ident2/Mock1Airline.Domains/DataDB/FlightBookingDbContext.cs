using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Mock1Airline.Domains.EntitiesDB;

namespace Mock1Airline.Domains.DataDB;

public partial class FlightBookingDbContext : DbContext
{
    public FlightBookingDbContext()
    {
    }

    public FlightBookingDbContext(DbContextOptions<FlightBookingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<FlightClass> FlightClasses { get; set; }

    public virtual DbSet<FlightRoute> FlightRoutes { get; set; }

    public virtual DbSet<FlightRoutesFill> FlightRoutesFills { get; set; }

    public virtual DbSet<HotelBooking> HotelBookings { get; set; }

    public virtual DbSet<Meal> Meals { get; set; }

    public virtual DbSet<RouteItem> RouteItems { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQL22_VIVES; Database=FlightBookingDB_GH;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(256);
            entity.Property(e => e.LastName).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951ACD6FD9B78C");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.ArrivalDate).HasColumnType("datetime");
            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.Class).HasMaxLength(50);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepartureDate).HasColumnType("datetime");
            entity.Property(e => e.HotelBooking).HasMaxLength(450);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("UserID");

            entity.HasOne(d => d.ArrivalCityNavigation).WithMany(p => p.BookingArrivalCityNavigations)
                .HasForeignKey(d => d.ArrivalCity)
                .HasConstraintName("FK_Bookings_Cities1");

            entity.HasOne(d => d.DepartureCityNavigation).WithMany(p => p.BookingDepartureCityNavigations)
                .HasForeignKey(d => d.DepartureCity)
                .HasConstraintName("FK_Bookings_Cities");

            entity.HasOne(d => d.HotelBookingNavigation).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.HotelBooking)
                .HasConstraintName("FK_Bookings_HotelBooking");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_AspNetUsers");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.HasKey(e => e.BookingDetailId).HasName("PK__BookingD__8136D47AD0ACFBCC");

            entity.Property(e => e.BookingDetailId).HasColumnName("BookingDetailID");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.FlightId).HasColumnName("FlightID");
            entity.Property(e => e.MealId).HasColumnName("MealID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingDe__Booki__5BE2A6F2");

            entity.HasOne(d => d.Flight).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingDe__Fligh__5CD6CB2B");

            entity.HasOne(d => d.Meal).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.MealId)
                .HasConstraintName("FK__BookingDe__MealI__5DCAEF64");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__488B0B2ADE3067BB");

            entity.Property(e => e.CartItemId).HasColumnName("CartItemID");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.ShoppingCartId).HasColumnName("ShoppingCartID");

            entity.HasOne(d => d.Booking).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItem__Bookin__6383C8BA");

            entity.HasOne(d => d.ShoppingCart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ShoppingCartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItem__Shoppi__628FA481");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.Property(e => e.CityId)
                .ValueGeneratedNever()
                .HasColumnName("CityID");
            entity.Property(e => e.CityName).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Utcoffset).HasColumnName("UTCoffset");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.FlightId).HasName("PK__Flights__8A9E148EF8312BD8");

            entity.Property(e => e.FlightId).HasColumnName("FlightID");
            entity.Property(e => e.BasePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DateTimeArrive).HasColumnType("datetime");
            entity.Property(e => e.DateTimeDepart).HasColumnType("datetime");
            entity.Property(e => e.JourneyId).HasColumnName("JourneyID");
            entity.Property(e => e.RouteId).HasColumnName("RouteID");

            entity.HasOne(d => d.ArriveCityNavigation).WithMany(p => p.FlightArriveCityNavigations)
                .HasForeignKey(d => d.ArriveCity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flights_Cities1");

            entity.HasOne(d => d.DepartCityNavigation).WithMany(p => p.FlightDepartCityNavigations)
                .HasForeignKey(d => d.DepartCity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flights_Cities");

            entity.HasOne(d => d.Route).WithMany(p => p.Flights)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flights_RouteItems");
        });

        modelBuilder.Entity<FlightClass>(entity =>
        {
            entity.HasKey(e => new { e.FlightId, e.ClassType }).HasName("PK__FlightCl__2BC8D07D5C11A90B");

            entity.Property(e => e.FlightId).HasColumnName("FlightID");
            entity.Property(e => e.ClassType).HasMaxLength(50);

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightClasses)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FlightCla__Fligh__5165187F");
        });

        modelBuilder.Entity<FlightRoute>(entity =>
        {
            entity.HasKey(e => e.FlightRouteId).HasName("PK__FlightRo__812C3CDC7930AE02");

            entity.Property(e => e.FlightRouteId).HasColumnName("FlightRouteID");
            entity.Property(e => e.FlightId).HasColumnName("FlightID");
            entity.Property(e => e.JourneyId).HasColumnName("JourneyID");
            entity.Property(e => e.RouteId).HasColumnName("RouteID");
        });

        modelBuilder.Entity<FlightRoutesFill>(entity =>
        {
            entity.HasKey(e => e.FlightRouteIds).HasName("PK_RoutesFill");

            entity.ToTable("FlightRoutesFill");

            entity.Property(e => e.FlightRouteIds)
                .ValueGeneratedNever()
                .HasColumnName("FlightRouteIDS");
            entity.Property(e => e.OwnRouteIds).HasColumnName("OwnRouteIDS");
            entity.Property(e => e.ParentRouteIds).HasColumnName("ParentRouteIDS");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<HotelBooking>(entity =>
        {
            entity.ToTable("HotelBooking");

            entity.Property(e => e.HotelBookingId).HasColumnName("HotelBookingID");
            entity.Property(e => e.Checkin).HasColumnType("datetime");
            entity.Property(e => e.Checkout).HasColumnType("datetime");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Meal>(entity =>
        {
            entity.HasKey(e => e.MealId).HasName("PK__Meal__ACF6A65D0837A1FF");

            entity.Property(e => e.MealId).HasColumnName("MealID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.AreaCodeNavigation).WithMany(p => p.Meals)
                .HasForeignKey(d => d.AreaCode)
                .HasConstraintName("FK_Meals_Cities");
        });

        modelBuilder.Entity<RouteItem>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("PK__Routes__80979AADD1CAF1DD");

            entity.Property(e => e.RouteId)
                .ValueGeneratedNever()
                .HasColumnName("RouteID");

            entity.HasOne(d => d.ArrivalCityNavigation).WithMany(p => p.RouteItemArrivalCityNavigations)
                .HasForeignKey(d => d.ArrivalCity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RouteItems_Cities1");

            entity.HasOne(d => d.DepartCityNavigation).WithMany(p => p.RouteItemDepartCityNavigations)
                .HasForeignKey(d => d.DepartCity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RouteItems_Cities");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.ShoppingCartId).HasName("PK__Shopping__7A789A844783BE84");

            entity.Property(e => e.ShoppingCartId).HasColumnName("ShoppingCartID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ShoppingCart_AspNetUsers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
