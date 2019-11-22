using System.Xml.Serialization;

namespace ProductShop.Dtos.Import
{
    [XmlType("Category")]
    public class CategoryDto
    {
        public int Id { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }
    }
}
