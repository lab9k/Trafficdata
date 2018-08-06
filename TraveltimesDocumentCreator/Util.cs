using System;
using System.Collections.Generic;
using System.Text;

namespace TraveltimesDocumentCreator
{
    public class Util
    {

        public static DateTime GetDateTimeFromUnixInMillies(long millies)
        {
            // Format our new DateTime object to start at the UNIX Epoch
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

            // Add the timestamp (number of seconds since the Epoch) to be converted
           return dateTime.AddMilliseconds(millies);
        }

    }
}
