using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlRoot("Users")]
    public class UserResultDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray(ElementName = "users")]
        public UserDto[] ResultUserDto { get; set; }
    }
}
