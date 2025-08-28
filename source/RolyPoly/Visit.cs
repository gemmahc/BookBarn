namespace BookBarn.Crawler
{
    /// <summary>
    /// Represents a visit to a specified resource.
    /// </summary>
    public class Visit
    {
        /// <summary>
        /// The number of times the resource has been visited.
        /// </summary>
        public int Visits { get; set; }

        /// <summary>
        /// The result of the last visit.
        /// </summary>
        public Result LastResult { get; set; }
    }
}
