namespace Cinema.DataProcessor.ImportDto
{
    using System;
    using System.Xml.Serialization;

    [XmlType("Projection")]
    public class ImportXmlProjectionDto
    {
        [XmlElement("MovieId")]
        public int MovieId { get; set; }

        [XmlElement("HallId")]
        public int HallId { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }
    }
}
