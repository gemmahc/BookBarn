using System.Runtime.Serialization;

namespace BookBarn.Model
{
    /// <summary>
    /// Represents a book.
    /// </summary>
    [DataContract]
    public class Book
    {
        /// <summary>
        /// Unique identifier for this book in the Book Barn
        /// </summary>
        [DataMember(Name="_id")]
        public string? Id { get; set; }

        /// <summary>
        /// The title of the book.
        /// </summary>
        [DataMember]
        public string? Title { get; set; }

        /// <summary>
        /// The author of the book (assumes a primary author)
        /// </summary>
        [DataMember]
        public string? Author { get; set; }

        /// <summary>
        /// The description of the book.
        /// </summary>
        [DataMember]
        public string? Description { get; set; }

        /// <summary>
        /// The rating out of 5 of this book.
        /// </summary>
        [DataMember]
        public double Rating { get; set; }

        /// <summary>
        /// The number of ratings the book has.
        /// </summary>
        [DataMember]
        public int RatingCount { get; set; }

        /// <summary>
        /// The genres the book is listed under.
        /// </summary>
        [DataMember]
        public string[]? Genres { get; set; }

        /// <summary>
        /// The url to the primary source page for this book.
        /// </summary>
        [DataMember]
        public string? Url { get; set; }

        /// <summary>
        /// If the book is part of a series - the url to the series information.
        /// </summary>
        [DataMember]
        public string? SeriesUrl { get; set; }

        /// <summary>
        /// If the book is part of a series - the name of the series.
        /// </summary>
        [DataMember]
        public string? SeriesName { get; set; }

        /// <summary>
        /// If the book is part of a series - which book in the series this is.
        /// </summary>
        [DataMember]
        public string? SeriesRank { get; set; }

        /// <summary>
        /// The date the book was published (specific format)
        /// </summary>
        [DataMember]
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// The number of pages this book has (specific format)
        /// </summary>
        [DataMember]
        public int? Pages { get; set; }

        /// <summary>
        /// The specific format this book represents (e.g. ebook vs hardcover)
        /// </summary>
        [DataMember]
        public string? Format { get; set; }

        /// <summary>
        /// The year this edition of the book was published
        /// </summary>
        [DataMember]
        public int? PublishYear { get; set; }

        /// <summary>
        /// The location of the cover image for this edition of the book.
        /// </summary>
        [DataMember]
        public string? CoverImage { get; set; }

        /// <summary>
        /// The media details for the cover image in BookBarn storage.
        /// </summary>
        [DataMember]
        public Media? CoverMedia { get; set; }

        /// <summary>
        /// The time of the last update to this record.
        /// </summary>
        [DataMember]
        public DateTime? LastUpdated { get; set; }
    }
}
