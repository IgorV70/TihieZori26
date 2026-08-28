using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DbCommon.Json;

namespace TihieZori.Code
{
    public class AdmOrdersJson:JsonArray
    {
        public string ServiceName;
        public int OrderTime;
        public string ClientName;
        public string ClientPhone;
        public string Uslugi;
        public int Duration;

    }
}