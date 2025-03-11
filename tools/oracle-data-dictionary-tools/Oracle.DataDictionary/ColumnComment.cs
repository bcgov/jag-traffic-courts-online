using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Oracle.DataDictionary;

/// <summary>
/// 
/// </summary>
[Table("ALL_COL_COMMENTS")]
[PrimaryKey(nameof(TableName), nameof(ColumnName))]
public class ColumnComment
{
    /// <summary>
    /// Owner of the object
    /// </summary>
    [Column("OWNER")]
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Name of the object
    /// </summary>
    [Column("TABLE_NAME")]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the column
    /// </summary>
    [Column("COLUMN_NAME")]
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// Comment on the column
    /// </summary>
    [Column("COMMENTS")]
    public string Comments { get; set; } = string.Empty;

    public TableColumn Column { get; set; } = null!;
}
