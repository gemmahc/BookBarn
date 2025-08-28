namespace BookBarn.Crawler.GoodReads.Test
{
    public static class TestDataHelper
    {
        public static string TestDataRoot = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty, "TestData");

        #region Books
        public static string StandaloneBook = File.ReadAllText(Path.Combine(TestDataRoot, "StandaloneBook.txt"));

        public static Uri StandaloneBookUri = new Uri("https://www.goodreads.com/book/show/10210.Jane_Eyre");

        public static string BookInSeries = File.ReadAllText(Path.Combine(TestDataRoot, "BookInSeries.txt"));

        public static Uri BookInSeriesUrl = new Uri("https://www.goodreads.com/book/show/61215351-the-fellowship-of-the-ring");

        #endregion

        #region Series

        public static string SeriesPage = File.ReadAllText(Path.Combine(TestDataRoot, "SeriesPage.txt"));

        public static Uri SeriesPageUrl = new Uri("https://www.goodreads.com/series/66175-middle-earth");

        #endregion

        #region List
        public static string ShortList = File.ReadAllText(Path.Combine(TestDataRoot, "ShortList.txt"));

        public static Uri ShortListUrl = new Uri("https://www.goodreads.com/list/show/158020.2021_Must_read_UF_series_free_with_Kindle_Unlimited_as_at_January_2021");

        public static string FirstPageInList = File.ReadAllText(Path.Combine(TestDataRoot, "FirstPageInList.txt"));

        public static Uri FirstPageInListUrl = new Uri("https://www.goodreads.com/list/show/153030.Best_FICTION_FANTASY_Books_2020_IG_Reads_Choice_Awards?page=1");

        public static string LastPageInList = File.ReadAllText(Path.Combine(TestDataRoot, "LastPageInList.txt"));

        public static Uri LastPageInListUrl = new Uri("https://www.goodreads.com/list/show/153030.Best_FICTION_FANTASY_Books_2020_IG_Reads_Choice_Awards?page=4");

        #endregion
    }
}
