using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// TCVP-2109
/// Message to indicate that a document was uploaded through COMS and needs to be virus scanned to 
/// identify whether it's safe to use or not and update the metadata of the document accordingly
/// </summary>
public class VirusScanDocument
{
    /// <summary>
    /// Id of the document uploaded to be virus scanned
    /// </summary>
    public Guid DocumentId { get; set; }
}
