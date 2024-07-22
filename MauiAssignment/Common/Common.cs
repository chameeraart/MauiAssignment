using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MauiAssignment.Common
{
    public class Common
    {
        public string ConvertXmlToJsonfile(XElement xmlElement)
        {
            var json = JsonConvert.SerializeXNode(xmlElement);
            return json;
        }

        public XElement LoadXmlfile(string filePath)
        {
            return XElement.Load(filePath);
        }

    }
}
