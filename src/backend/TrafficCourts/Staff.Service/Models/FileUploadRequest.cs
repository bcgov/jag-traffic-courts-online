using System.ComponentModel.DataAnnotations;

namespace TrafficCourts.Staff.Service.Models;

/// <summary>
/// Represents a single file to upload to COMS and its metadata
/// </summary>
public class FileUploadRequest
{
    // TODO: Determine the specific file format mime types allowed from the request
    [Required]
    public IFormFile File { get; set; }

    [Required]
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}
