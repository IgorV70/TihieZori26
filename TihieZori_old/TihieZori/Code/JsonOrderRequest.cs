using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TihieZori.Code
{
    [DataContract]
    public class JsonOrderRequest
    {
        [DataMember]
        public int sevicecenter { get; set; }

        [DataMember]
        public int[] uslugi { get; set; }

        [DataMember]
        public int dateday { get; set; }


        public DateTime DtDateday { get; set; }

        [DataMember]
        public int offset { get; set; }

        [DataMember]
        public string phone { get; set; }

        [DataMember]
        public int Avtime { get; set; }

    }

}