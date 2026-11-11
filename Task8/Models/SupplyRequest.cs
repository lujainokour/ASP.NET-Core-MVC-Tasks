using System;

namespace UniversityCafeteriaSupply8.Models
{
    public class SupplyRequest
    {
        public int Id { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = "Pending";

        public string UserId { get; set; } = "";
    }
}