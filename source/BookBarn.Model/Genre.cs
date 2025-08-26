using System.Runtime.Serialization;

namespace BookBarn.Model
{
    /// <summary>
    /// Represents a genre of book.
    /// </summary>
    [DataContract]
    public class Genre
    {
        /// <summary>
        /// The unique Id / name of the genre
        /// </summary>
        [DataMember(Name = "_id")]
        public string? Id { get; set; }

        /// <summary>
        /// The number of books in the BookBarn that are included in this genre.
        /// </summary>
        [DataMember]
        public int? Count { get; set; }
    }
}
