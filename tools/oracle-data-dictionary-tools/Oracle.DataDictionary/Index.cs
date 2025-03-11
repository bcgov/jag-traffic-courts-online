using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle.DataDictionary;

/// <summary>
/// Describes an index on a table, view etc
/// </summary>
[Table("ALL_INDEXES")]
[PrimaryKey(nameof(Name))]
public class Index
{
    /// <summary>
    /// Owner of the index
    /// </summary>
    [Column("OWNER")]
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Owner of the indexed object
    /// </summary>
    [Column("INDEX_NAME")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Owner of the table
    /// </summary>
    [Column("TABLE_OWNER")]
    public string TableOwner { get; set; } = string.Empty;

    /// <summary>
    /// Name of the indexed object
    /// </summary>
    [Column("TABLE_NAME")]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Type of the indexed object
    /// </summary>
    [Column("TABLE_TYPE")]
    public string TableType { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the index is unique (UNIQUE) or nonunique (NONUNIQUE)
    /// </summary>
    [Column("UNIQUENESS")]
    public string Uniqueness { get; set; } = string.Empty;

    /// <summary>
    /// Type of the index
    /// </summary>
    [Column("INDEX_TYPE")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The table the index is declared on
    /// </summary>
    public Table Table { get; set; } = null!;

    /// <summary>
    /// The columns on the index index
    /// </summary>
    public ICollection<IndexColumn> Columns { get; set;} = new List<IndexColumn>();
}
