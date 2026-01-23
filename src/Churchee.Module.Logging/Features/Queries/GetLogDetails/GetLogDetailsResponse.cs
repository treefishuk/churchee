using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Churchee.Module.Logging.Features.Queries
{
    public class GetLogDetailsResponse
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string MessageTemplate { get; set; }

        public string Level { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Exception { get; set; }

        public string PropertiesString { get; set; }

        public List<Property> Properties => ParseXmlString(PropertiesString);

        private static List<Property> ParseXmlString(string xmlString)
        {
            var xdoc = XDocument.Parse(xmlString);
            return xdoc.Descendants("property")
                       .Select(p => new Property
                       {
                           Key = p.Attribute("key").Value,
                           Value = p.Value,
                       }).ToList();
        }


        public class Property
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public List<Property> SubProperties { get; set; }
        }

    }
}
