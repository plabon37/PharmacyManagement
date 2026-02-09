using System;
using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
    public class MedicineRequest
    {
        [Key] // 🔴 MUST BE NEEDED
        public int RequestId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string MedicineName { get; set; } = "";

        public DateTime RequestDate { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
