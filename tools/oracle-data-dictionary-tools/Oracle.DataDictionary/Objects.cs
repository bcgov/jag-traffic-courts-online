using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Oracle.DataDictionary;

/// <summary>
/// Describes all objects accessible to the current user. 
/// </summary>
[Table("ALL_OBJECTS")]
[PrimaryKey(nameof(Owner), nameof(Name), nameof(ObjectType))]
[DebuggerDisplay("{DebuggerToString(),nq}")]
public class OracleObject
{
    /// <summary>
    /// Owner of the object
    /// </summary>
    [Column("OWNER")]
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Name of the object
    /// </summary>
    [Column("OBJECT_NAME")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of the object (such as TABLE, INDEX) 
    /// </summary>
    [Column("OBJECT_TYPE")]
    public string ObjectType { get; set; } = string.Empty;

    /// <summary>
    /// Status of the object, such as VALID, INVALID, or N/A
    /// </summary>
    [Column("STATUS")]
    public string Status { get; set; } = string.Empty;

    private string DebuggerToString()
    {
        if (!string.IsNullOrEmpty(Owner))
        {
            return $"{Owner}.{Name}";
        }

        return Name;
    }
}