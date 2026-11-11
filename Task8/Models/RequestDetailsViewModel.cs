namespace UniversityCafeteriaSupply8.Models
{
    public class RequestDetailsViewModel
    {
        public SupplyRequest Request { get; set; } = new SupplyRequest();
        public string FullName { get; set; } = "";
        public List<RequestProductViewModel> Items { get; set; } = new List<RequestProductViewModel>();
    }
}
