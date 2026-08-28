using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TihieZori.Code
{   
    [DataContract]
    public class JsonTimeResponce
    {
        [DataMember]
        public int sevicecenter { get; set; }

        [DataMember]
        public int dateShift { get; set; }

        [DataMember]
        public int[] starttime { get; set; }
    }
}