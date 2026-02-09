namespace PharmacyManagement.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "Customer";

        public bool IsActive { get; set; } = true;   // ✅ SOFT DELETE
    }
}
