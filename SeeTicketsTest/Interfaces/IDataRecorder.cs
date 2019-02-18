using System.Collections.Generic;

namespace SeeTicketsTest.Interfaces
{
    public interface IDataRecorder
    {
        bool Record(IEnumerable<CrawledInfo> data);
    }
}
