using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
    public class Medicine
    {
        public int MedicineId { get; set; }   // 🔴  MATCH WITH DB

        public string Name { get; set; } = "";

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}


