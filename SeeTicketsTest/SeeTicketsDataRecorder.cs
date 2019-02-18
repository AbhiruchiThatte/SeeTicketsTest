using Newtonsoft.Json;
using SeeTicketsTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SeeTicketsTest
{
    public class SeeTicketsDataRecorder : IDataRecorder
    {
        public bool Record(IEnumerable<CrawledInfo> data)
        {
            bool success = false;

            string serialisedData = JsonConvert.SerializeObject(data);

            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePathAndName = string.Format("{0}\\seetickets_crawl_data_{1}.txt", path, DateTime.Now.ToString("yyyyMMddHHmm"));
                using (var file = File.Create(filePathAndName))
                {
                    using (StreamWriter sw = new StreamWriter(file))
                    {
                        sw.WriteLine(serialisedData);
                    }
                }

                success = true;
            }
            catch
            {
                success = false;
            }

            return success;
        }
    }
}
