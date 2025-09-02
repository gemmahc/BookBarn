using System.Text;

namespace BookBarn.Model
{
    /// <summary>
    /// Extension methods for the <c>Book</c> class.
    /// </summary>
    public static class BookExtensions
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
    }
}
