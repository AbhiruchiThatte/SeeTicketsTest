using AngleSharp;
using AngleSharp.Dom;
using SeeTicketsTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeeTicketsTest
{
    public class SeeTicketsCrawler : ICrawler
    {
        public string BASE_URL = "https://www.seetickets.com";

        private const string ACT_PAGE_CLASS = "block-link";
        private const string EVENT_PAGE_CLASS = "g-blocklist-link view-order-link";

        private IDataRecorder dataRecorder;
        private IDateTimeExtracter dateTimeExtracter;

        private IEnumerable<char> pagesToVisit;

        /// <summary> The maximum data entries to collect </summary>
        private int maxEntries;

        private List<CrawledInfo> data;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataRecorder"></param>
        /// <param name="dateTimeExtracter"></param>
        /// <param name="maxEntries"> The maximum data entries to collect </param>
        public SeeTicketsCrawler(IDataRecorder dataRecorder, IDateTimeExtracter dateTimeExtracter,
                                 char[] pagesToVisit, int maxEntries)
        {
            this.dataRecorder = dataRecorder;
            this.dateTimeExtracter = dateTimeExtracter;
            this.pagesToVisit = pagesToVisit;
            this.maxEntries = maxEntries;

            this.data = new List<CrawledInfo>();
        }

        public async Task<IEnumerable<CrawledInfo>> Crawl()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            foreach (char letter in this.pagesToVisit)
            {
                await GetEventList(context, letter);
            }

            RecordData();

            return data;
        }

        private void RecordData()
        {
            if (this.data.Count == 0)
            {
                Console.WriteLine("Crawl failed");
            }
            else
            {
                bool success = this.dataRecorder.Record(this.data);

                if (success == true)
                {
                    Console.WriteLine("Writing crawl data succeeded");
                }
                else
                {
                    Console.WriteLine("Writing crawl data failed");
                }
            }
        }

        private async Task GetEventList(IBrowsingContext context, char letter)
        {
            var address = string.Format("{0}/browse/tours/active/{1}", BASE_URL, letter);

            var document = await context.OpenAsync(address);
            
            IEnumerable<IElement> elements = document.All.Where(e => e.ClassName == ACT_PAGE_CLASS);

            foreach (IElement element in elements)
            {
                await GetActPage(context, element);

                if (this.maxEntries == 0)
                {
                    // keep gathering data
                }
                else
                {
                    if (this.data.Count > maxEntries)
                    {
                        // we have all the data we wanted to collect
                        break;
                    }
                }
            }
        }

        private async Task GetActPage(IBrowsingContext context, IElement element)
        {
            string actTitle = element.GetAttribute("title");

            string address = element.GetAttribute("href");

            await GetEventsForAct(context, actTitle, address);
        }

        private async Task GetEventsForAct(IBrowsingContext context, string actTitle, string address)
        {
            IDocument document = await OpenPage(context, address);

            IEnumerable<IElement> elements = document.All.Where(e => e.ClassName == EVENT_PAGE_CLASS);

            foreach (IElement element in elements)
            {
                var eventAddress = element.GetAttribute("href");

                await GetEventInfo(context, actTitle, eventAddress);
            }
        }

        private async Task GetEventInfo(IBrowsingContext context, string actTitle, string eventAddress)
        {
            var document = await OpenPage(context, eventAddress);

            var crawledInfo = new CrawledInfo();
            crawledInfo.ArtistName = actTitle;

            try
            {
                GetVenueInformation(document, crawledInfo);

                this.data.Add(crawledInfo);
                Console.WriteLine(string.Format("Fetched data {0}", this.data.Count));
            }
            catch (Exception e)
            {
                // couldn't get required details
                Console.WriteLine(string.Format("Failed to crawl {0} details at: {1}", actTitle, eventAddress));
            }
        }

        private void GetVenueInformation(IDocument document, CrawledInfo crawledInfo)
        {
            // navigate down to the classes we want
            IHtmlCollection<IElement> elements = document.GetElementsByClassName("eventinfo");
            var element = elements.Where(e => e.Children.HasClass("g-ui-box-content")).First();
            var childElement = element.FirstElementChild;

            foreach (IElement child in childElement.Children)
            {
                if (child.TagName == "H3")
                {
                    GetCityAndVenue(crawledInfo, child.FirstElementChild.GetAttribute("title"));
                }
                else if (child.TagName == "P" && child.ClassName == null)
                {
                    crawledInfo.EventDate = this.dateTimeExtracter.Extract(child.FirstElementChild.Text());
                }
            }
        }

        private void GetCityAndVenue(CrawledInfo crawledInfo, string venueLocation)
        {
            string[] array = venueLocation.Split(",");
            crawledInfo.VenueName = array[0].Trim();
            crawledInfo.CityName = array[1].Trim();
        }

        private async Task<IDocument> OpenPage(IBrowsingContext context, string address)
        {
            return await context.OpenAsync(string.Format("{0}{1}", BASE_URL, address));
        }
    }
}
