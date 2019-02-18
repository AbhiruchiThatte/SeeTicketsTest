using System;
using System.Collections.Generic;
using System.Text;

namespace SeeTicketsTest.Interfaces
{
    public interface IDateTimeExtracter
    {
        DateTime Extract(string dateTime);
    }
}
