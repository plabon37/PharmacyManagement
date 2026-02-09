using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Data;
using PharmacyManagement.Models;
using System;
using System.Linq;

namespace PharmacyManagement.Controllers
{
    // Controller responsible for all Customer-related actions
    // Includes dashboard, order history, medicine requests, and messages
    public class CustomerController : Controller
    {
        // Database context to access application data
        private readonly ApplicationDbContext _context;

        // Constructor injection of ApplicationDbContext
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // CUSTOMER DASHBOARD
        // =========================
        // Displays customer dashboard (menu only)
        public IActionResult Dashboard()
        {
            // Allow access only for logged-in customers
            if (HttpContext.Session.GetString("UserRole") != "Customer")
                return RedirectToAction("Login", "Account");

            return View();
        }

        // =========================
        // ORDER HISTORY
        // =========================
        // Displays all past orders of the logged-in customer
        public IActionResult Orders()
        {
            // Customer authorization check
            if (HttpContext.Session.GetString("UserRole") != "Customer")
                return RedirectToAction("Login", "Account");

            // Get logged-in user ID from session
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Fetch orders placed by the customer
            var orders = _context.Sales
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SaleDate)
                .ToList();

            return View(orders);
        }

        // =========================
        // REQUEST MEDICINE (GET)
        // =========================
        // Displays medicine request form
        public IActionResult RequestMedicine()
        {
            return View();
        }

        // =========================
        // REQUEST MEDICINE (POST)
        // =========================
        // Saves a medicine request submitted by customer
        [HttpPost]
        public IActionResult RequestMedicine(string medicineName)
        {
            // Get logged-in user ID
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Create a new medicine request
            var req = new MedicineRequest
            {
                UserId = userId,
                MedicineName = medicineName,
                RequestDate = DateTime.Now,
                Status = "Pending"
            };

            // Save request to database
            _context.MedicineRequests.Add(req);
            _context.SaveChanges();

            // Redirect back to dashboard after submission
            return RedirectToAction("Dashboard");
        }

        // =========================
        // CUSTOMER MESSAGES
        // =========================
        // Displays messages sent by admin to the customer
        public IActionResult Messages()
        {
            // Get logged-in user ID
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Fetch messages for the customer
            var msgs = _context.Messages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            return View(msgs);
        }
    }
}
