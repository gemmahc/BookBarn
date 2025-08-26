namespace BookBarn.Model
{
    /// <summary>
    /// Represents the queryable attributes of a book in BookBarn storage.
    /// </summary>
    public class BookQuery
    {
        /// <summary>
        /// Match on provided title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Match on provided author.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Match on books with a rating greater or equal to value.
        /// </summary>
        public double? RatingFloor { get; set; }

        /// <summary>
        /// Match on books having at lease this many ratings.
        /// </summary>
        public int? MinRatings { get; set; }

        /// <summary>
        /// Matches books that include ALL of the genres specified.
        /// </summary>
        public bool? MatchAllIncludedGenres { get; set; }

        /// <summary>
        /// Matches books containing any of the included genres 
        /// </summary>
        public string[]? IncludedGenres { get; set; }

        /// <summary>
        /// Excludes matches with genres in this list.
        /// </summary>
        public string[]? ExcludedGenres { get; set; }

        /// <summary>
        /// Matches on books that are the first books in a series.
        /// </summary>
        public bool? FirstInSeries { get; set; }

        /// <summary>
        /// Matches on books with the string contained in their description.
        /// </summary>
        public string? DescriptionContains { get; set; }
    }
}
