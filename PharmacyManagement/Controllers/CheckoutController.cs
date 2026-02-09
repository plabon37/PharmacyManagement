using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Data;
using PharmacyManagement.Models;
using System.Text.Json;

namespace PharmacyManagement.Controllers
{
    // Controller responsible for checkout, order confirmation, and billing
    public class CheckoutController : Controller
    {
        // Database context for accessing medicines and sales
        private readonly ApplicationDbContext _context;

        // Constructor injection of ApplicationDbContext
        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // CHECKOUT PAGE
        // =========================
        // Displays checkout page with current cart items
        public IActionResult Index()
        {
            return View(GetCart());
        }

        // =========================
        // CONFIRM ORDER (POST)
        // =========================
        // Confirms order, updates stock, and saves sales records
        // This action should be executed only once per checkout
        [HttpPost]
        public IActionResult Confirm()
        {
            // Get cart items from session
            var cart = GetCart();

            // If cart is empty, redirect back to cart page
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            // Get logged-in user ID from session
            var userId = HttpContext.Session.GetInt32("UserId");

            // If user is not logged in, redirect to login page
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Process each item in the cart
            foreach (var item in cart)
            {
                // Find medicine from database
                var med = _context.Medicines.Find(item.MedicineId);
                if (med == null)
                    return RedirectToAction("Index", "Cart");

                // Check if enough stock is available
                if (med.Quantity < item.Quantity)
                {
                    // Store error message temporarily and redirect
                    TempData["Error"] = $"Not enough stock for {med.Name}";
                    return RedirectToAction("Index", "Cart");
                }

                // Reduce medicine stock quantity
                med.Quantity -= item.Quantity;

                // Save sale record to database
                _context.Sales.Add(new Sale
                {
                    MedicineId = item.MedicineId,
                    MedicineName = item.MedicineName,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    TotalPrice = item.Price * item.Quantity,
                    SaleDate = DateTime.Now,
                    UserId = userId.Value
                });
            }

            // Save all changes to database (stock + sales)
            _context.SaveChanges();

            // Clear cart from session after successful checkout
            HttpContext.Session.Remove("Cart");

            // Redirect to invoice/bill page
            return RedirectToAction("Bill");
        }

        // =========================
        // BILL / INVOICE PAGE
        // =========================
        // Displays order completion or invoice information
        public IActionResult Bill()
        {
            return View();
        }

        // =========================
        // HELPER METHOD
        // =========================
        // Retrieves cart data from session and converts it to List<CartItem>
        private List<CartItem> GetCart()
        {
            var data = HttpContext.Session.GetString("Cart");

            return data == null
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(data);
        }
    }
}
