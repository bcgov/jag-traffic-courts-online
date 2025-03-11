using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle.DataDictionary;

/// <summary>
/// Describes the columns of all tables, views, and clusters in the database
/// </summary>
[PrimaryKey(nameof(TableName), nameof(Name))]
[Table("ALL_TAB_COLUMNS")]
public class TableColumn
{
    /// <summary>
    /// Owner of the table
    /// </summary>
    [Column("OWNER")]
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Name of the table
    /// </summary>
    [Column("TABLE_NAME")]
    public string TableName { get; set; } = string.Empty;

    public Table Table { get; set; } = null!;

    public ColumnComment? Comment { get; set; }

    /// <summary>
    /// Column name
    /// </summary>
    [Column("COLUMN_NAME")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Data type of the column
    /// </summary>
    [Column("DATA_TYPE")]
    public string? DataType { get; set; }

    /// <summary>
    /// Length of the column (in bytes)
    /// </summary>
    [Column("DATA_LENGTH")]
    public int DataLength { get; set; }

    /// <summary>
    /// Decimal precision for NUMBER data type; binary precision for FLOAT data type; NULL for all other data types
    /// </summary>
    [Column("DATA_PRECISION")]
    public int? DataPrecision { get; set; }

    /// <summary>
    /// Digits to the right of the decimal point in a number
    /// </summary>
    [Column("DATA_SCALE")]
    public int? DataScale { get; set; }

    /// <summary>
    /// Indicates whether a column allows NULLs. The value is N if there is a NOT NULL constraint on the column or if the column is part of a PRIMARY KEY.
    /// </summary>
    [Column("NULLABLE")]
    public string? Nullable { get; set; }

    /// <summary>
    /// Sequence number of the column as created
    /// </summary>
    [Column("COLUMN_ID")]
    public int ColumnId { get; set; }
}
