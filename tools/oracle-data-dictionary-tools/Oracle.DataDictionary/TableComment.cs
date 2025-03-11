using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle.DataDictionary;

[Table("ALL_TAB_COMMENTS")]
[PrimaryKey(nameof(Owner),nameof(TableName))]
public class TableComment
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
    /// Type of the object
    /// </summary>
    [Column("TABLE_TYPE")]
    public string TableType { get; set; } = string.Empty;

    /// <summary>
    /// Comment on the object
    /// </summary>
    [Column("COMMENTS")]
    public string Comments { get; set; } = string.Empty;

    public Table Table { get; set; } = null!;
}
