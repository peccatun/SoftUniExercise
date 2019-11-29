namespace Cinema.DataProcessor.ExportDto
{
    using System.Xml.Serialization;

    [XmlRoot("Customer")]
    public class ExportXmlCustomer
    {
        [XmlAttribute("FirstName")]
        public string FirstName { get; set; }

        [XmlAttribute("LastName")]
        public string LastName { get; set; }
    }
}
