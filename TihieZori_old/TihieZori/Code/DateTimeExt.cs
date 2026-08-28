using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TihieZori.Code
{
    public static class DateTimeExt
    {
        public static int ToPosix(this DateTime dt)
        {
            return (int)(dt - new DateTime(1970, 1, 1)).TotalSeconds;
        }


    }
}