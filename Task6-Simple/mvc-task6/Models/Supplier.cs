using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc_task6.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string Phone { get; set; } = "";
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
