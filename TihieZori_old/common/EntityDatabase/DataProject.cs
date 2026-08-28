using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EntityData
{
    [XmlRoot("DataProject", Namespace = "http://www.atpm-air.ru/dpm", IsNullable = false)]
    public class CDataProject
    {
        public CDataProject()
        {
            DataBaseList = new List<Database>();
        }

        [XmlArray("BaseList")]
        public List<Database> DataBaseList { get; set; }
    }
}
