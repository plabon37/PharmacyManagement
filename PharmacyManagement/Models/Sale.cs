namespace PharmacyManagement.Models
{
    public class Sale
    {
        public int SaleId { get; set; }

        public int MedicineId { get; set; }
        public string? MedicineName { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime SaleDate { get; set; }

        public int? UserId { get; set; }   // ✅ FIXED
    }
}
