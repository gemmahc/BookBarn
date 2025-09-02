namespace BookBarn.Model
{
    /// <summary>
    /// Represents dimension to query genres on.
    /// </summary>
    public class GenreQuery
    {
        /// <summary>
        /// The id of the genre (e.g. "Fiction")
        /// </summary>
        public string? Id { get; set; }
    }
}
