using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle.DataDictionary;

/// <summary>
/// Describes all constraint definitions
/// </summary>
[Table("ALL_CONSTRAINTS")]
[PrimaryKey(nameof(Name))]
public class Constraint
{
    /// <summary>
    /// Owner of the constraint definition
    /// </summary>
    [Column("OWNER")]
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Name of the constraint definition
    /// </summary>
    [Column("CONSTRAINT_NAME")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of the constraint
    /// 
    /// C - Check constraint on a table
    /// P - Primary key
    /// U - Unique key
    /// R - Referential integrity
    /// V - With check option, on a view
    /// O - With read only, on a view
    /// H - Hash expression
    /// F - Constraint that involves a REF column
    /// S - Supplemental logging
    /// </summary>
    [Column("CONSTRAINT_TYPE")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Name associated with the table (or view) with the constraint definition
    /// </summary>
    [Column("TABLE_NAME")]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Text of search condition for a check constraint. 
    /// </summary>
    [Column("SEARCH_CONDITION")]
    public string? Condition { get; set; }

    /// <summary>
    /// Owner of the table referred to in a referential constraint
    /// </summary>
    [Column("R_OWNER")]
    public string? ReferencedOwner { get; set; }

    /// <summary>
    /// Name of the unique constraint definition for the referenced table
    /// </summary>
    [Column("R_CONSTRAINT_NAME")]
    public string? ReferencedConstraintName { get; set; }

    [Column("INDEX_OWNER")]
    public string? IndexOwner { get; set; }

    [Column("INDEX_NAME")]
    public string? IndexName { get; set; }

    public Table Table { get; set; } = null!;
    public ICollection<ConstraintColumn> Columns { get; set;} = new List<ConstraintColumn>();
}
