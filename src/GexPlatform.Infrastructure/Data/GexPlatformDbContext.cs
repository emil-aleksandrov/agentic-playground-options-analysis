using GexPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GexPlatform.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for the GEX Platform.
/// </summary>
public class GexPlatformDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GexPlatformDbContext"/> class.
    /// </summary>
    /// <param name="options">The DbContextOptions for EF Core.</param>
    public GexPlatformDbContext(DbContextOptions<GexPlatformDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet for OptionChain entities.
    /// </summary>
    public DbSet<OptionChain> OptionChains { get; set; }

    /// <summary>
    /// DbSet for OptionContract entities.
    /// </summary>
    public DbSet<OptionContract> OptionContracts { get; set; }

    /// <summary>
    /// Configures the model on model creation.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure OptionChain
        modelBuilder.Entity<OptionChain>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Ticker).IsRequired().HasMaxLength(10);
            entity.Property(e => e.ExpirationDate).IsRequired();
            entity.Property(e => e.UnderlyingPrice).HasPrecision(18, 4);

            // Create compound index for unique constraint
            entity.HasIndex(e => new { e.Ticker, e.ExpirationDate })
                .IsUnique();

            // Configure one-to-many relationship
            entity.HasMany(e => e.Contracts)
                .WithOne(c => c.OptionChain)
                .HasForeignKey(c => c.OptionChainId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure OptionContract
        modelBuilder.Entity<OptionContract>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OptionType).IsRequired().HasMaxLength(4);
            entity.Property(e => e.StrikePrice).HasPrecision(18, 4);
            entity.Property(e => e.BidPrice).HasPrecision(18, 4);
            entity.Property(e => e.AskPrice).HasPrecision(18, 4);
            entity.Property(e => e.LastPrice).HasPrecision(18, 4);
            entity.Property(e => e.ImpliedVolatility).HasPrecision(10, 6);
            entity.Property(e => e.Delta).HasPrecision(10, 6);
            entity.Property(e => e.Gamma).HasPrecision(10, 6);
            entity.Property(e => e.Theta).HasPrecision(10, 6);
            entity.Property(e => e.Vega).HasPrecision(10, 6);
            entity.Property(e => e.Rho).HasPrecision(10, 6);

            // Create index for performance
            entity.HasIndex(e => new { e.OptionChainId, e.OptionType, e.StrikePrice })
                .HasDatabaseName("IX_OptionContract_ChainTypeStrike");
        });

        // Configure common entity properties
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Ensure CreatedAt and UpdatedAt have default values
            var createdAtProperty = entityType.FindProperty(nameof(BaseEntity.CreatedAt));
            if (createdAtProperty != null)
            {
                createdAtProperty.SetDefaultValueSql("CURRENT_TIMESTAMP");
            }

            var updatedAtProperty = entityType.FindProperty(nameof(BaseEntity.UpdatedAt));
            if (updatedAtProperty != null)
            {
                updatedAtProperty.SetDefaultValueSql("CURRENT_TIMESTAMP");
            }
        }
    }

    /// <summary>
    /// Override SaveChanges to update the UpdatedAt timestamp.
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to update the UpdatedAt timestamp.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates the UpdatedAt timestamp for modified entities.
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && e.State == EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            if (entityEntry.Entity is BaseEntity entity)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
