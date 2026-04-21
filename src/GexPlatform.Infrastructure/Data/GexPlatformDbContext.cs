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
    /// Configures the model on model creation.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
