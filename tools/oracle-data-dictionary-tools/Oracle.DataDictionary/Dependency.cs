using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Oracle.DataDictionary;

/// <summary>
/// escribes dependencies between procedures, packages, functions, package bodies, and 
/// triggers owned by the current user, including dependencies on views created without 
/// any database links. Its columns are the same as those in ALL_DEPENDENCIES. 
/// </summary>
[Table("ALL_DEPENDENCIES")]
[PrimaryKey(nameof(Owner), nameof(Name))]
[DebuggerDisplay("{DebuggerToString(),nq}")]
public class Dependency
{
    /// <summary>
    /// Owner of the object
    /// </summary>
    [Column("OWNER")]
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Name of the object
    /// </summary>
    [Column("NAME")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of the object
    /// </summary>
    [Column("TYPE")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Owner of the referenced object (remote owner if remote object)
    /// </summary>
    [Column("REFERENCED_OWNER")]
    public string ReferencedOwner { get; set; } = string.Empty;

    /// <summary>
    /// Name of the referenced object
    /// </summary>
    [Column("REFERENCED_NAME")]
    public string ReferencedName { get; set; } = string.Empty;

    /// <summary>
    /// Type of the referenced object
    /// </summary>
    [Column("REFERENCED_TYPE")]
    public string ReferencedType { get; set; } = string.Empty;

    /// <summary>
    /// Name of the link to the parent object (if remote)
    /// </summary>
    [Column("REFERENCED_LINK_NAME")]
    public string? ReferencedLinkName { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the dependency is a REF dependency (REF) or not (HARD) 
    /// </summary>
    [Column("DEPENDENCY_TYPE")]
    public string DependencyType { get; set; } = string.Empty;

    private string DebuggerToString()
    {
        if (!string.IsNullOrEmpty(Owner))
        {
            return $"{Owner}.{Name}";
        }

        return Name;
    }
}
