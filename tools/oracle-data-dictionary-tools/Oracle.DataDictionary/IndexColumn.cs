using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle.DataDictionary;

/// <summary>
/// Describes the columns of indexes on tables
/// </summary>
[Table("ALL_IND_COLUMNS")]
[PrimaryKey(nameof(IndexName), nameof(TableName), nameof(ColumnName))]
public class IndexColumn
{
    /// <summary>
    /// Owner of the index
    /// </summary>
    [Column("INDEX_OWNER")]
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Name of the index
    /// </summary>
    [Column("INDEX_NAME")]
    public string IndexName { get; set; } = string.Empty;

    /// <summary>
    /// Owner of the table
    /// </summary>
    [Column("TABLE_OWNER")]
    public string TableOwner { get; set; } = string.Empty;

    /// <summary>
    /// Name of the table
    /// </summary>
    [Column("TABLE_NAME")]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Column name 
    /// </summary>
    [Column("COLUMN_NAME")]
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// Position of the column or attribute within the index
    /// </summary>
    [Column("COLUMN_POSITION")]
    public int Position { get; set; }

    /// <summary>
    /// Indicates whether the column is sorted in descending order (DESC) or ascending order (ASC)
    /// </summary>
    [Column("DESCEND")]
    public string Order { get; set; } = string.Empty;

    public Index Index { get; set; } = null!;
}
