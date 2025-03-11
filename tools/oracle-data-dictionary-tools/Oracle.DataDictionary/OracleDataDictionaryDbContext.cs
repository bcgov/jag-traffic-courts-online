using Microsoft.EntityFrameworkCore;

namespace Oracle.DataDictionary;

/// <summary>
/// Provides access to the Oracle data dictionary tables and views
/// </summary>
public class OracleDataDictionaryDbContext : DbContext
{
    public OracleDataDictionaryDbContext(DbContextOptions<OracleDataDictionaryDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    /// <summary>
    /// Gets the relational tables accessible to the current user
    /// </summary>
    public DbSet<Table> Tables { get; set; }
    /// <summary>
    /// Gets the columns of the tables, views, and clusters accessible to the current user.
    /// </summary>
    public DbSet<TableColumn> TableColumns { get; set; }

    public DbSet<TableComment> TableComments { get; set; }

    /// <summary>
    /// Gets the comments on the columns of the tables and views accessible to the current user.
    /// </summary>
    public DbSet<ColumnComment> ColumnComments { get; set; }

    /// <summary>
    /// Gets the constraint definitions on tables accessible to the current user
    /// </summary>
    public DbSet<Constraint> Constraints { get; set; }

    /// <summary>
    /// Gets the columns that are accessible to the current user and that are specified in constraints
    /// </summary>
    public DbSet<ConstraintColumn> ConstraintColumns { get; set; }

    /// <summary>
    /// Gets the indexes on the tables accessible to the current user.
    /// </summary>
    public DbSet<Index> Indexes { get; set; }

    /// <summary>
    /// Gets the columns of indexes on all tables accessible to the current user.
    /// </summary>
    public DbSet<IndexColumn> IndexColumns { get; set; }


    public DbSet<Dependency> Dependencies { get; set; }

    /// <summary>
    /// Describes the text source of the stored objects accessible to the current user. 
    /// </summary>
    public DbSet<Source> Sources { get; set; }
    public DbSet<OracleObject> Objects { get; set; }

    //public DbSet<Objects> Objects { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.LogTo(Console.WriteLine);

    /// <summary>
    /// Get the indexes for the specified table
    /// </summary>
    public IQueryable<Index> IndexesFor(Table table) 
        => Indexes.Where(i => i.TableOwner == table.Owner && i.TableName == table.Name);

    /// <summary>
    /// Gets the primary key constraints for the specified table
    /// </summary>
    public IQueryable<Constraint> PrimaryKeyFor(Table table)
        => ConstraintsFor(table).Where(c => c.Type == ConstraintType.PrimaryKey);

    /// <summary>
    /// Gets the foreign key constraints for the specified table
    /// </summary>
    public IQueryable<Constraint> ForeignKeysFor(Table table)
    => ConstraintsFor(table).Where(c => c.Type == ConstraintType.ForeignKey);

    /// <summary>
    /// Gets the constraints for the specified table
    /// </summary>
    public IQueryable<Constraint> ConstraintsFor(Table table) 
        => Constraints.Where(c => c.Owner == table.Owner && c.TableName == table.Name);

    /// <summary>
    /// Gets the unique constraint definition for the referenced table of the specified constraint
    /// </summary>
    public IQueryable<Constraint> ReferencedConstraintFor(Constraint constraint)
        => Constraints.Where(c => c.Owner == constraint.ReferencedOwner && c.Name == constraint.ReferencedConstraintName);

    public IQueryable<TableComment> TableCommentsFor(Table table)
        => TableComments.Where(c => c.Owner == table.Owner && c.TableName == table.Name);

    public IQueryable<Table> TableFor(Constraint constraint)
        => Tables.Where(t => t.Owner == constraint.Owner && t.Name == constraint.TableName);

    public IQueryable<Table> TablesOwnedBy(string owner)
        => Tables.Where(t => t.Owner == owner);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // table < columns
        modelBuilder.Entity<Table>()
            .HasMany(e => e.Columns)
            .WithOne(e => e.Table)
            .HasForeignKey(e => new { e.Owner, e.TableName})
            .HasPrincipalKey(e => new { e.Owner, e.Name });

        // table - comment
        modelBuilder.Entity<Table>()
            .HasOne(e => e.Comment)
            .WithOne(e => e.Table)
            .HasForeignKey<TableComment>(e => new { e.Owner, e.TableName })
            .HasPrincipalKey<Table>(e => new { e.Owner, e.Name })
            .IsRequired(false); // table comment is optional

        // table < index
        modelBuilder.Entity<Table>()
            .HasMany(e => e.Indexes)
            .WithOne(e => e.Table)
            .HasForeignKey(e => new { e.TableOwner, e.TableName })
            .HasPrincipalKey(e => new { e.Owner, e.Name });

        // table < constraint
        modelBuilder.Entity<Table>()
            .HasMany(e => e.Constraints)
            .WithOne(e => e.Table)
            .HasForeignKey(e => new { e.Owner, e.TableName })
            .HasPrincipalKey(e => new { e.Owner, e.Name });

        modelBuilder.Entity<TableColumn>()
            .HasOne(e => e.Comment)
            .WithOne(e => e.Column)
            .HasForeignKey<ColumnComment>(e => new { e.Owner, e.TableName, e.ColumnName })
            .HasPrincipalKey<TableColumn>(e => new { e.Owner, e.TableName, e.Name })
            .IsRequired(false); // table column comment is optional

        modelBuilder.Entity<Index>()
            .HasMany(e => e.Columns)
            .WithOne(e => e.Index)
            .HasForeignKey(e => new { e.Owner, e.IndexName })
            .HasPrincipalKey(e => new { e.Owner, e.Name });

        modelBuilder.Entity<Constraint>()
            .HasMany(e => e.Columns)
            .WithOne(e => e.Constraint)
            .HasForeignKey(e => new { e.Owner, e.ConstraintName })
            .HasPrincipalKey(e => new { e.Owner, e.Name });
    }
}
