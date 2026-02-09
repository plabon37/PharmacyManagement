namespace PharmacyManagement.Models
{
    public class CartItem
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
