using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle.DataDictionary;

[Table("ALL_SOURCE")]
[PrimaryKey(nameof(Owner), nameof(Name), nameof(Type))]
public class Source
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
    /// Type of object: FUNCTION, JAVA SOURCE, PACKAGE, PACKAGE BODY, PROCEDURE, TRIGGER, TYPE, TYPE BODY
    /// </summary>
    [Column("TYPE")]
    public string? Type { get; set; } = string.Empty;

    /// <summary>
    /// Line number of this line of source
    /// </summary>
    [Column("LINE")]
    public int Line { get; set; }

    /// <summary>
    /// Text source of the stored object
    /// </summary>
    [Column("TEXT")]
    public string? Text { get; set; }
}
