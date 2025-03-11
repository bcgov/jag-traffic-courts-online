using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Oracle.DataDictionary;

/// <summary>
/// Describes all relational tables in the database.
/// </summary>
[Table("ALL_TABLES")]
[PrimaryKey(nameof(Owner),nameof(Name))]
[DebuggerDisplay("{DebuggerToString(),nq}")]
public class Table
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
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The table comment
    /// </summary>
    public TableComment? Comment { get; set; }
    /// <summary>
    /// The columns of the table
    /// </summary>
    public ICollection<TableColumn> Columns { get; set; } = new List<TableColumn>();

    /// <summary>
    /// The indexes of the table
    /// </summary>
    public ICollection<Index> Indexes { get; set; } = new List<Index>();

    /// <summary>
    /// The constraints of the table
    /// </summary>
    public ICollection<Constraint> Constraints { get; set; } = new List<Constraint>();

    private string DebuggerToString()
    {
        if (!string.IsNullOrEmpty(Owner))
        {
            return $"{Owner}.{Name}";
        }

        return Name;
    }
}
