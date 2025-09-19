using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahmoudZakaria_APITask.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [MaxLength(20)]
        public string ProductCode { get; set; } 

        [Required]
        public string Name { get; set; }

        public string ImagePath { get; set; } 

        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MinimumQuantity { get; set; }

        [Range(0, 1)]
        public double DiscountRate { get; set; }
    }
}
