using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProjectWebApp.Model
{
    public partial class FlightlyDBContext : DbContext
    {
        public FlightlyDBContext()
        {
        }

        public FlightlyDBContext(DbContextOptions<FlightlyDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Airline> Airlines { get; set; }
        public virtual DbSet<AirlineStatus> AirlineStatuses { get; set; }
        public virtual DbSet<Airport> Airports { get; set; }
        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<BackupHistory> BackupHistories { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingStatus> BookingStatuses { get; set; }
        public virtual DbSet<ClassType> ClassTypes { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<FeaturedDestination> FeaturedDestinations { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<FlightStatus> FlightStatuses { get; set; }
        public virtual DbSet<Guest> Guests { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }
        public virtual DbSet<PromoCode> PromoCodes { get; set; }
        public virtual DbSet<PromoStatus> PromoStatuses { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<ReportType> ReportTypes { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<TourismPopup> TourismPopups { get; set; }
        public virtual DbSet<TransactionType> TransactionTypes { get; set; }
        public virtual DbSet<UserPreference> UserPreferences { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=FlightlyDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Airline>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Airlines)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Airlines__Countr__74AE54BC");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Airlines)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Airlines__Status__75A278F5");
            });

            modelBuilder.Entity<AirlineStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__AirlineS__C8EE2063E12C835D");
            });

            modelBuilder.Entity<Airport>(entity =>
            {
                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Airports)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Airports__Countr__778AC167");
            });

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AnnouncementUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Announcements_UserProfile");
            });

            modelBuilder.Entity<BackupHistory>(entity =>
            {
                entity.HasKey(e => e.BackupId)
                    .HasName("PK__BackupHi__EB9069C2C35F5F0E");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BackupHistories)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_BackupHistory_UserProfile");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(e => e.BookingDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LoyaltyPointsEarned).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.BookingStatus)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.BookingStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bookings__Bookin__7A672E12");

                entity.HasOne(d => d.ClassType)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ClassTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bookings_ClassType");

                entity.HasOne(d => d.Flight)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.FlightId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bookings__Flight__7B5B524B");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bookings__Paymen__7C4F7684");

                entity.HasOne(d => d.Promo)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.PromoId)
                    .HasConstraintName("FK_Bookings_PromoCodes");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Bookings__UserId__7D439ABD");
            });

            modelBuilder.Entity<BookingStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__BookingS__C8EE20636AC072AD");
            });

            modelBuilder.Entity<FeaturedDestination>(entity =>
            {
                entity.HasKey(e => e.DestinationId)
                    .HasName("PK__Featured__DB5FE4CC8DD5A0DC");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FeaturedDestinations)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_FeaturedDestinations_UserProfile");
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Airline)
                    .WithMany(p => p.Flights)
                    .HasForeignKey(d => d.AirlineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Flights__Airline__02084FDA");

                entity.HasOne(d => d.DestinationAirport)
                    .WithMany(p => p.FlightDestinationAirports)
                    .HasForeignKey(d => d.DestinationAirportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Flights__Destina__02FC7413");

                entity.HasOne(d => d.OriginAirport)
                    .WithMany(p => p.FlightOriginAirports)
                    .HasForeignKey(d => d.OriginAirportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Flights__OriginA__03F0984C");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Flights)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Flights__StatusI__04E4BC85");
            });

            modelBuilder.Entity<FlightStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__FlightSt__C8EE206396919E9D");
            });

            modelBuilder.Entity<Guest>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Logs)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Logs__UserId__06CD04F7");
            });

            modelBuilder.Entity<LoyaltyTransaction>(entity =>
            {
                entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.LoyaltyTransactions)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK__LoyaltyTr__Booki__07C12930");

                entity.HasOne(d => d.TransactionType)
                    .WithMany(p => p.LoyaltyTransactions)
                    .HasForeignKey(d => d.TransactionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LoyaltyTransactions_TransactionTypes");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LoyaltyTransactions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LoyaltyTr__UserI__08B54D69");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Payments__Paymen__0A9D95DB");

                entity.HasOne(d => d.PaymentStatus)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PaymentStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payments_PaymentStatuses");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Payments__UserId__0B91BA14");
            });

            modelBuilder.Entity<PromoCode>(entity =>
            {
                entity.HasKey(e => e.PromoId)
                    .HasName("PK__PromoCod__33D334B0EF55C0F6");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.PromoCodes)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Promo_Status");
            });

            modelBuilder.Entity<PromoStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__PromoSta__C8EE206363D18FFA");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.Property(e => e.GeneratedOn).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ReportType)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.ReportTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reports_ReportTypes");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Booking)
                    .WithOne(p => p.Review)
                    .HasForeignKey<Review>(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Reviews__Booking__123EB7A3");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Reviews__UserId__1332DBDC");
            });

            modelBuilder.Entity<TourismPopup>(entity =>
            {
                entity.HasKey(e => e.PopupId)
                    .HasName("PK__TourismP__2FE251A96C38E60B");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TourismPopups)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_TourismPopup_UserProfile");
            });

            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.HasKey(e => e.PreferenceId)
                    .HasName("PK__UserPref__E228496FE126DFF3");

                entity.HasOne(d => d.PreferredOriginAirport)
                    .WithMany(p => p.UserPreferences)
                    .HasForeignKey(d => d.PreferredOriginAirportId)
                    .HasConstraintName("FK__UserPrefe__Prefe__160F4887");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPreferences)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserPrefe__UserI__17036CC0");
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__UserProf__1788CC4C4ECADCE3");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LoyaltyPoints).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserProfiles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProfile_Roles");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
