using Microsoft.EntityFrameworkCore;

namespace TrafficCourts.Hotfix.DataMigration.Data
{
    /// <summary>
    /// Base DbContext for hotfix SQLite databases
    /// Provides common functionality and configuration
    /// </summary>
    public abstract class BaseHotfixDbContext : DbContext
    {
        private readonly string _dbPath;
        protected readonly string HotfixName;
        protected readonly string Env;

        protected BaseHotfixDbContext(string hotfixName, string env = "dev")
        {
            HotfixName = hotfixName;
            Env = env;
            var dataDir = Path.Combine(Environment.CurrentDirectory, ".db", "sqlite");
            Directory.CreateDirectory(dataDir);
            _dbPath = Path.Combine(dataDir, $"{hotfixName}_{Env}.db");
        }

        public DbSet<HotfixOccamDispute> HotfixOccamDisputes { get; set; } = null!;
        
        public DbSet<HotfixRSITicketSearch> HotfixRSITicketSearches { get; set; } = null!;

        public DbSet<HotfixViolationTicket> HotfixViolationTickets { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = $"Data Source={GetDatabasePath()};";
                optionsBuilder.UseSqlite(connectionString);
            }

            // Enable detailed logging for development (can be disabled in production)
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);

            // Additional SQLite-specific configurations
            optionsBuilder.EnableSensitiveDataLogging(false);
            optionsBuilder.EnableDetailedErrors(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureBaseModels(modelBuilder);
            
            // Allow derived classes to add their own configurations
            ConfigureHotfixSpecificModels(modelBuilder);
        }

        /// <summary>
        /// Configures the base models that are common across all hotfixes
        /// </summary>
        private void ConfigureBaseModels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HotfixOccamDispute>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TicketNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CachedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.DataJson).HasMaxLength(5000);

                // Add index on TicketNumber for faster lookups (not unique to allow multiple cache entries)
                entity.HasIndex(e => e.TicketNumber);
            });

            modelBuilder.Entity<HotfixRSITicketSearch>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TicketNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CachedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.DataJson).HasMaxLength(5000);

                // Add index on TicketNumber for faster lookups
                entity.HasIndex(e => e.TicketNumber).IsUnique();
            });
            
            modelBuilder.Entity<HotfixViolationTicket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TicketNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CachedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.DataJson).HasMaxLength(5000);

                // Add index on TicketNumber for faster lookups
                entity.HasIndex(e => e.TicketNumber).IsUnique();
            });

        }

        /// <summary>
        /// Override this method in derived classes to configure hotfix-specific models
        /// </summary>
        protected virtual void ConfigureHotfixSpecificModels(ModelBuilder modelBuilder)
        {
            // Base implementation does nothing
            // Derived classes can override to add their own model configurations
        }

        /// <summary>
        /// Helper method to ensure database is created
        /// </summary>
        public async Task EnsureDatabaseCreatedAsync()
        {
            await Database.EnsureCreatedAsync();

            // Set WAL mode using PRAGMA command after database is created
            await Database.ExecuteSqlRawAsync("PRAGMA journal_mode = WAL;");
    
        }

        /// <summary>
        /// Gets the database file path for this hotfix
        /// </summary>
        public string GetDatabasePath() => _dbPath;
    }
}
