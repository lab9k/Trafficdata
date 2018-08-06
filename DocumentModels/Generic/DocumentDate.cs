using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace DocumentModels.Generic
{
    public class DocumentDate
    {
        public DocumentDate(DateTime date)
        {
            FullDate = date;
        }


        public DateTime FullDate { get; }

        public DayOfWeek DayOfWeek => FullDate.DayOfWeek;

        public int Day => FullDate.Day;

        public int Month => FullDate.Month;

        public int Year => FullDate.Year;

        public int Hour => FullDate.Hour;
        public int Minute => FullDate.Minute;
        public int Seconds => FullDate.Second;

        public long Unix
        {
            get
            {
                DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                return (long)(FullDate - sTime).TotalSeconds;
            }
        }
    }
}
