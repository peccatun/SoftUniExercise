using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Data.Models
{
    public class Seat
    {
        public int Id { get; set; }

        [ForeignKey("Hall")]
        public int HallId { get; set; }

        public Hall Hall { get; set; }
    }
}
