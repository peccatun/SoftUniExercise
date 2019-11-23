using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("SoldProducts")]
    public class CountAndProductsDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlElement("products")]
        public SoldProductDto[] Products { get; set; }
    }
}
