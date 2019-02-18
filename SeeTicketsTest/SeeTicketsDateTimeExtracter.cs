using SeeTicketsTest.Interfaces;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SeeTicketsTest
{
    public class SeeTicketsDateTimeExtracter : IDateTimeExtracter
    {
        public DateTime Extract(string dateTime)
        {
            // get the date and time text
            string[] dateArray = dateTime.Split(" ");

            string date = string.Empty;
            if (dateArray.Length == 4)
            {
                // contains just date
                Match day = Regex.Match(dateArray[1], "\\d+");
                dateArray[1] = day.Value;

                // day month year
                date = string.Format("{0} {1} {2}", dateArray[1], dateArray[2], dateArray[3]);
            }
            else if (dateArray.Length == 7)
            {
                // contains date and time
                Match day = Regex.Match(dateArray[1], "\\d+");
                dateArray[1] = day.Value;
                
                // day month year time
                date = string.Format("{0} {1} {2} {3}", dateArray[1], dateArray[2], dateArray[3], dateArray[5]);
            }

            bool success = DateTime.TryParse(date, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime parsedDateTime);
            
            if (success == false)
            {
                // make a new exception type
                throw new Exception("Parse failed. Date: " + dateTime);
            }

            return parsedDateTime;
        }
    }
}
