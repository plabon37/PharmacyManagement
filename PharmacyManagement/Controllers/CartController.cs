using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Data;
using PharmacyManagement.Models;
using System.Text.Json;

namespace PharmacyManagement.Controllers
{
    // Controller responsible for managing shopping cart operations
    // Cart data is stored in Session as JSON
    public class CartController : Controller
    {
        // Database context to access medicines
        private readonly ApplicationDbContext _context;

        // Constructor injection of ApplicationDbContext
        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // VIEW CART (Customer only)
        // =========================
        // Displays all items currently in the cart
        public IActionResult Index()
        {
            // Allow only logged-in customers
            if (HttpContext.Session.GetString("UserRole") != "Customer")
                return RedirectToAction("Login", "Account");

            // Send cart items to the view
            return View(GetCart());
        }

        // =========================
        // ADD ITEM TO CART
        // =========================
        // Adds a medicine to the cart or increases quantity
        public IActionResult Add(int id)
        {
            // Only customers can add items to cart
            if (HttpContext.Session.GetString("UserRole") != "Customer")
                return RedirectToAction("Index", "Medicine");

            // Find medicine by ID
            var med = _context.Medicines.Find(id);
            if (med == null)
                return RedirectToAction("Index", "Medicine");

            // Retrieve existing cart from session
            var cart = GetCart();

            // Check if item already exists in cart
            var item = cart.FirstOrDefault(x => x.MedicineId == id);

            if (item == null)
            {
                // Add new item to cart
                cart.Add(new CartItem
                {
                    MedicineId = id,
                    MedicineName = med.Name,
                    Price = med.Price,
                    Quantity = 1
                });
            }
            else
            {
                // Increase quantity if item already exists
                item.Quantity++;
            }

            // Save updated cart back to session
            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // =========================
        // INCREASE ITEM QUANTITY
        // =========================
        // Increases quantity of a specific cart item
        public IActionResult Increase(int id)
        {
            // Customer authorization check
            if (HttpContext.Session.GetString("UserRole") != "Customer")
                return RedirectToAction("Login", "Account");

            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MedicineId == id);

            if (item != null)
                item.Quantity++;

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // =========================
        // DECREASE ITEM QUANTITY
        // =========================
        // Decreases quantity or removes item if quantity reaches zero
        public IActionResult Decrease(int id)
        {
            // Customer authorization check
            if (HttpContext.Session.GetString("UserRole") != "Customer")
                return RedirectToAction("Login", "Account");

            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MedicineId == id);

            if (item != null)
            {
                item.Quantity--;

                // Remove item if quantity becomes zero
                if (item.Quantity <= 0)
                    cart.Remove(item);
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // =========================
        // REMOVE ITEM FROM CART
        // =========================
        // Completely removes a specific item from the cart
        public IActionResult Remove(int id)
        {
            // Customer authorization check
            if (HttpContext.Session.GetString("UserRole") != "Customer")
                return RedirectToAction("Login", "Account");

            var cart = GetCart();

            // Remove all matching items by MedicineId
            cart.RemoveAll(x => x.MedicineId == id);

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // =========================
        // HELPER METHODS (UNCHANGED)
        // =========================

        // Retrieves cart data from session and converts it to List<CartItem>
        private List<CartItem> GetCart()
        {
            var data = HttpContext.Session.GetString("Cart");

            return data == null
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(data);
        }

        // Saves cart data into session after converting it to JSON
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(
                "Cart",
                JsonSerializer.Serialize(cart)
            );
        }
    }
}
