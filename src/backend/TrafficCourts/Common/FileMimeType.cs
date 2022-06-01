namespace System.IO;

/// <summary>
/// Represents a file mime type.
/// </summary>
/// <param name="Extension">The normalized file extension.</param>
/// <param name="MimeType">The mime type of the file</param>
public record FileMimeType(string MimeType, string Extension);
