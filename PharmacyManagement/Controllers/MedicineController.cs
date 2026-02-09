using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Data;
using PharmacyManagement.Models;
using System;
using System.Linq;

namespace PharmacyManagement.Controllers
{
    // Controller responsible for medicine management
    // Used by both Admin and Customer (with role restrictions)
    public class MedicineController : Controller
    {
        // Database context to access medicine-related data
        private readonly ApplicationDbContext _context;

        // Constructor injection of ApplicationDbContext
        public MedicineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX (Admin + Customer)
        // =========================
        // Displays all medicines to logged-in users
        public IActionResult Index()
        {
            // Ensure user is logged in
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            // Fetch all medicines from database
            var medicines = _context.Medicines.ToList();
            return View(medicines);
        }

        // =========================
        // CREATE - GET (Admin only)
        // =========================
        // Displays medicine creation form
        public IActionResult Create()
        {
            // Only Admin can access this page
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            return View();
        }

        // =========================
        // CREATE - POST (Admin only)
        // =========================
        // Handles medicine creation and customer notification
        [HttpPost]
        public IActionResult Create(Medicine medicine)
        {
            // 🔐 Admin authorization check
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            // ✅ Basic validation checks
            if (medicine == null)
                return Content("Medicine data missing");

            if (string.IsNullOrWhiteSpace(medicine.Name))
                return Content("Medicine name is required");

            if (medicine.Price <= 0)
                return Content("Medicine price must be greater than 0");

            if (medicine.Quantity < 0)
                return Content("Medicine quantity cannot be negative");

            // ✅ Add new medicine to database
            _context.Medicines.Add(medicine);

            // 🔔 Notify customers who previously requested this medicine
            var requests = _context.MedicineRequests
                .Where(r => r.MedicineName == medicine.Name && r.Status == "Pending")
                .ToList();

            foreach (var r in requests)
            {
                // Send message notification to customer
                _context.Messages.Add(new Message
                {
                    UserId = r.UserId,
                    MessageText = $"Your requested medicine '{medicine.Name}' is now available.",
                    CreatedAt = DateTime.Now
                });

                // Update request status
                r.Status = "Completed";
            }

            // 💾 Save all changes (medicine + messages + requests)
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // =========================
        // EDIT - GET (Admin only)
        // =========================
        // Displays edit form for selected medicine
        public IActionResult Edit(int id)
        {
            // Admin-only access
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            // Find medicine by ID
            var medicine = _context.Medicines.Find(id);
            if (medicine == null)
                return NotFound();

            return View(medicine);
        }

        // =========================
        // EDIT - POST (Admin only)
        // =========================
        // Updates medicine details
        [HttpPost]
        public IActionResult Edit(Medicine medicine)
        {
            // Ensure Admin access
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            // Update medicine record
            _context.Medicines.Update(medicine);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // =========================
        // DELETE (Admin only)
        // =========================
        // Permanently deletes a medicine
        public IActionResult Delete(int id)
        {
            // Admin authorization check
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            // Find medicine by ID
            var medicine = _context.Medicines.Find(id);
            if (medicine == null)
                return NotFound();

            // Remove medicine from database
            _context.Medicines.Remove(medicine);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
