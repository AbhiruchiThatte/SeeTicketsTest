using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeeTicketsTest.Interfaces
{
    public interface ICrawler
    {
        Task<IEnumerable<CrawledInfo>> Crawl();
    }
}
