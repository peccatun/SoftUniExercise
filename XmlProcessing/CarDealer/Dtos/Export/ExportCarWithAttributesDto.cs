using System.Xml.Serialization;

namespace CarDealer.Dtos.Export
{
    [XmlRoot(ElementName = "car")]
    public class ExportCarWithAttributesDto
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}
