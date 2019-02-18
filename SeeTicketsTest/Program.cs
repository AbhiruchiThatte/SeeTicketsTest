using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeeTicketsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataRecorder = new SeeTicketsDataRecorder();
            var dateTimeExtracter = new SeeTicketsDateTimeExtracter();

            char[] alphabet;
            // alphabet = "-ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            alphabet = "E".ToCharArray();

            SeeTicketsCrawler crawler = new SeeTicketsCrawler(dataRecorder, dateTimeExtracter, alphabet, 20);

            crawler.Crawl().Wait();
        }
    }
}
