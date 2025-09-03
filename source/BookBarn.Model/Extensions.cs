using Newtonsoft.Json;
using System.Text;

namespace BookBarn.Model
{
    /// <summary>
    /// Extension methods for contract objects.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the storage Id for this book instance.
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetId(this Book book)
        {
            ArgumentNullException.ThrowIfNull(book);

            if (string.IsNullOrEmpty(book.Title) || string.IsNullOrEmpty(book.Author))
            {
                throw new ArgumentException("A valid Title and Author are required for a book to be valid.");
            }

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] input = Encoding.ASCII.GetBytes($"{book.Title} - {book.Author}");
                byte[] hash = md5.ComputeHash(input);

                return Convert.ToHexString(hash).ToLowerInvariant();
            }
        }

        /// <summary>
        /// Gets the json representation of this query
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>The json string.</returns>
        public static string ToJson(this BookQuery query)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.NullValueHandling = NullValueHandling.Ignore;

            return JsonConvert.SerializeObject(query, settings);
        }
    }
}
