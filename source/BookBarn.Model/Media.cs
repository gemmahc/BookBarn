using System.Runtime.Serialization;

namespace BookBarn.Model;

/// <summary>
/// Represents media in storage.
/// </summary>
[DataContract]
public class Media
{
    /// <summary>
    /// Gets or sets the file name of the media.
    /// </summary>
    [DataMember]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the location of the media (relative to storage root)
    /// </summary>
    [DataMember]
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the checksum of the media in storage.
    /// </summary>
    [DataMember]
    public string? Checksum { get; set; }

    /// <summary>
    /// Gets or sets the content type information of the media.
    /// </summary>
    [DataMember]
    public string? ContentType { get; set; }
}
