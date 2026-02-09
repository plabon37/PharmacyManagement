using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Data;
using System.Linq;

namespace PharmacyManagement.Controllers
{
    // Controller responsible for all Admin-related operations
    // Includes dashboard, medicine management, customer control, and messages
    public class AdminController : Controller
    {
        // Database context to access application data
        private readonly ApplicationDbContext _context;

        // Constructor injection of ApplicationDbContext
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // ADMIN DASHBOARD
        // =========================
        // Displays admin dashboard after role verification
        public IActionResult Dashboard()
        {
            // Allow access only if the logged-in user is Admin
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            return View();
        }

        // =========================
        // MANAGE MEDICINES
        // =========================
        // Shows list of medicines for Add / Edit / Delete operations
        public IActionResult Medicines()
        {
            // Role-based authorization check
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            // Send all medicines to the view
            return View(_context.Medicines.ToList());
        }

        // =========================
        // CUSTOMER MANAGEMENT
        // =========================
        // Displays all registered customers
        public IActionResult Customers()
        {
            // Only Admin can access this page
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            // Fetch users with Customer role only
            var customers = _context.Users
                .Where(u => u.Role == "Customer")
                .ToList();

            return View(customers);
        }

        // =========================
        // MEDICINE REQUESTS
        // =========================
        // Shows customer medicine requests
        public IActionResult MedicineRequests()
        {
            // Admin authorization check
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            return View(_context.MedicineRequests.ToList());
        }

        // =========================
        // DISABLE CUSTOMER (SOFT DELETE)
        // =========================
        // Marks a customer account as inactive instead of deleting it
        public IActionResult DisableCustomer(int id)
        {
            // Admin-only access
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            // Find the customer by ID
            var user = _context.Users.FirstOrDefault(u => u.UserId == id && u.Role == "Customer");
            if (user == null)
                return NotFound();

            // Soft delete: deactivate user
            user.IsActive = false;
            _context.SaveChanges();

            return RedirectToAction("Customers");
        }

        // =========================
        // ENABLE CUSTOMER
        // =========================
        // Reactivates a previously disabled customer account
        public IActionResult EnableCustomer(int id)
        {
            // Admin-only access
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            // Find the customer by ID
            var user = _context.Users.FirstOrDefault(u => u.UserId == id && u.Role == "Customer");
            if (user == null)
                return NotFound();

            // Reactivate user account
            user.IsActive = true;
            _context.SaveChanges();

            return RedirectToAction("Customers");
        }
    }
}
