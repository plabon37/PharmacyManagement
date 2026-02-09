using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Data;
using PharmacyManagement.Models;
using System;
using System.Linq;

namespace PharmacyManagement.Controllers
{
    // Controller responsible for handling sales and purchase-related operations
    public class SalesController : Controller
    {
        // Database context to access medicines and sales data
        private readonly ApplicationDbContext _context;

        // Constructor injection of ApplicationDbContext
        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // BUY PAGE (GET)
        // =========================
        // Displays list of medicines available for purchase
        public IActionResult Buy()
        {
            var medicines = _context.Medicines.ToList();
            return View(medicines);
        }

        // =========================
        // BUY ACTION (POST)
        // =========================
        // Handles medicine purchase and creates a sales record
        [HttpPost]
        public IActionResult Buy(int medicineId, int quantity)
        {
            // Find medicine by ID
            var med = _context.Medicines.FirstOrDefault(m => m.MedicineId == medicineId);

            // Validate stock availability
            if (med == null || med.Quantity < quantity)
            {
                ViewBag.Error = "Not enough stock";
                return RedirectToAction("Buy");
            }

            // Create a new sales record
            var sale = new Sale
            {
                MedicineId = med.MedicineId,
                MedicineName = med.Name,
                Quantity = quantity,
                UnitPrice = med.Price,
                TotalPrice = med.Price * quantity,
                SaleDate = DateTime.Now,
                UserId = HttpContext.Session.GetInt32("UserId").Value
            };

            // Reduce medicine stock quantity
            med.Quantity -= quantity;

            // Save sale to database
            _context.Sales.Add(sale);
            _context.SaveChanges();

            // Redirect customer to order history
            return RedirectToAction("MyOrders");
        }

        // =========================
        // CUSTOMER ORDER HISTORY
        // =========================
        // Displays all orders placed by the logged-in customer
        public IActionResult MyOrders()
        {
            // Customer authorization check
            if (HttpContext.Session.GetString("UserRole") != "Customer")
                return RedirectToAction("Login", "Account");

            // Get logged-in user ID
            int userId = HttpContext.Session.GetInt32("UserId").Value;

            // Fetch sales for this customer
            var sales = _context.Sales
                .Where(s => s.UserId == userId)
                .ToList();

            return View(sales);
        }

        // =========================
        // ADMIN SALES REPORT
        // =========================
        // Displays all sales records (Admin only)
        public IActionResult Index()
        {
            // Admin authorization check
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            // Show all sales records
            return View(_context.Sales.ToList());
        }
    }
}
