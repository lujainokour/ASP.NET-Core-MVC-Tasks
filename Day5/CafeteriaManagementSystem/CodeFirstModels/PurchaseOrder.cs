using System.ComponentModel.DataAnnotations;

namespace CafeteriaManagementSystem.CodeFirstModels
{
    public class PurchaseOrder
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = string.Empty;

        public int SupplierId { get; set; }

        public Supplier Supplier { get; set; } = null!;
    }
}