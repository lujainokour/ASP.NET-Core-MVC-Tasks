namespace UniversityCafeteriaManagement1.Models
{
    public class Product1   
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public decimal price { get; set; } 
        public int quantity { get; set; }
        public int CategoryId { get; set; }

        public int SupplierId { get; set; }
    }
}
