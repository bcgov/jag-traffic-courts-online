using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Models;

/// <summary>
/// A class that contains metadata for a file that was uploaded through COMS
/// </summary>
[ExcludeFromCodeCoverage]
public class FileMetadata
{
    /// <summary>
    /// Unique identifier for a file in COMS
    /// </summary>
    public Guid? FileId { get; set; }

    /// <summary>
    /// The file name of the uploaded file
    /// </summary>
    public string? FileName { get; set; } = String.Empty;

    /// <summary>
    /// The notice of dispute guid notice-of-dispute-id
    /// </summary>
    public string? NoticeOfDisputeGuid { get; set; }

    /// <summary>
    /// The document type document-type
    /// </summary>
    public string? DocumentType { get; set; }

    /// <summary>
    /// The virus scan status virus-scan-status
    /// </summary>
    public string? VirusScanStatus { get; set; } 

    /// <summary>
    /// Document status (can be pending for disputant uploaded) document-status
    /// </summary>
    public string? DocumentStatus { get; set; }

    /// <summary>
    /// The ticket number ticket-number
    /// </summary>
    public string? TicketNumber { get; set; }

    /// <summary>
    /// JJ Dispute Id dispute-id
    /// </summary>
    public long? DisputeId { get; set; }

    /// <summary>
    /// File stream that pending for upload
    /// </summary>
    public string? PendingFileStream { get; set; }

    /// <summary>
    /// The document uploaded was request to be deleted by the disputant
    /// </summary>
    public Boolean? DeleteRequested { get; set; }
}
