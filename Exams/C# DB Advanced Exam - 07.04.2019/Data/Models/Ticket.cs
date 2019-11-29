using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Data.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01d,(double)decimal.MaxValue)]
        public decimal Price { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        [ForeignKey("Projection")]
        public int ProjectionId { get; set; }

        public Projection Projection { get; set; }
    }
}
