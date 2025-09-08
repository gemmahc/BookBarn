using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookBarn.Crawler
{
    public interface IPageClient
    {
        public Task<HtmlDocument> LoadAsync(Uri pageAddress);
    }
}
