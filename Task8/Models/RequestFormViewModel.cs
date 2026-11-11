namespace UniversityCafeteriaSupply8.Models
{
    public class RequestFormViewModel
    {
        public int Id { get; set; }
        public List<RequestProductViewModel> Products { get; set; } = new List<RequestProductViewModel>();
    }

    public class RequestProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public int Quantity { get; set; }
    }
}
