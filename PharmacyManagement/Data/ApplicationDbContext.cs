using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;

namespace PharmacyManagement.Data
{
    // This class represents the database context for the application
    // It is responsible for communicating with the database using Entity Framework Core
    public class ApplicationDbContext : DbContext
    {
        // Constructor that receives database configuration options
        // These options are provided from Program.cs (connection string, provider, etc.)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet for application users (Admin / Customer)
        // This table stores login and role-related information
        public DbSet<User> Users { get; set; }

        // DbSet for medicines available in the pharmacy
        // This table stores medicine name, price, quantity, etc.
        public DbSet<Medicine> Medicines { get; set; }

        // DbSet for customers
        // This table stores customer personal information
        public DbSet<Customer> Customers { get; set; }

        // DbSet for sales records
        // This table stores completed purchase transactions
        public DbSet<Sale> Sales { get; set; }

        // 🔴 REQUIRED DbSet
        // DbSet for medicine requests submitted by customers
        // Used when a medicine is not available
        public DbSet<MedicineRequest> MedicineRequests { get; set; }

        // DbSet for messages between admin and customers
        // Used for notifications, replies, and updates
        public DbSet<Message> Messages { get; set; }
    }
}
