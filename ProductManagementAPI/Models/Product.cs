using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
namespace ProductManagementAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [Column(TypeName="decimal(18,2)")]
        public decimal Price { get; set; }

        public int Stock { get; set; }
        public string? Description { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}