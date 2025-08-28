namespace BookBarn.Crawler.Extensions
{
    public static class UriExtensions
    {
        public static Uri SetAuthorityFrom(this Uri relative, Uri baseUri)
        {
            Uri baseAuthority = new Uri(baseUri.GetLeftPart(UriPartial.Authority));

            if (relative.IsAbsoluteUri)
            {
                return new Uri(baseAuthority, relative.PathAndQuery);
            }
            else
            {
                return new Uri(baseAuthority, relative.ToString());
            }
        }
    }
}
