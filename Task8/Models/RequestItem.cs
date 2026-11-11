namespace UniversityCafeteriaSupply8.Models
{
    public class RequestItem
    {
        public int Id { get; set; }

        public int SupplyRequestId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
