using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle.DataDictionary;

/// <summary>
/// Describes all columns in the database that are specified in constraints
/// </summary>
[Table("ALL_CONS_COLUMNS")]
[PrimaryKey(nameof(ConstraintName), nameof(ColumnName))]
public class ConstraintColumn
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
    public string ConstraintName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the table with the constraint definition
    /// </summary>
    [Column("TABLE_NAME")]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the column or attribute of the object type column specified in the constraint definition
    /// </summary>
    [Column("COLUMN_NAME")]
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// Original position of the column or attribute in the definition of the object
    /// </summary>
    [Column("POSITION")]
    public int Position { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Constraint Constraint { get; set; } = null!;

}
