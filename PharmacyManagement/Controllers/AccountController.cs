using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Data;
using PharmacyManagement.Models;
using System.Linq;

namespace PharmacyManagement.Controllers
{
    // Controller responsible for user authentication
    // Handles Login, Logout, and Registration
    public class AccountController : Controller
    {
        // Database context for accessing users table
        private readonly ApplicationDbContext _context;

        // Constructor injection of database context
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // LOGIN PAGE (GET)
        // =========================
        // Displays the login form
        public IActionResult Login()
        {
            return View();
        }

        // =========================
        // LOGIN ACTION (POST)
        // =========================
        // Handles login form submission
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Check user credentials and ensure account is active (soft delete check)
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == email &&
                u.Password == password &&
                u.IsActive == true);

            // If user not found or inactive
            if (user == null)
            {
                ViewBag.Error = "Invalid login or account disabled";
                return View();
            }

            // Store user information in session
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.Name);

            // Redirect based on user role
            if (user.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            // Default redirect for customers
            return RedirectToAction("Dashboard", "Customer");
        }

        // =========================
        // LOGOUT
        // =========================
        // Clears all session data and redirects to login page
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // =========================
        // REGISTER PAGE (GET)
        // =========================
        // Displays the registration form
        public IActionResult Register()
        {
            return View();
        }

        // =========================
        // REGISTER ACTION (POST)
        // =========================
        // Handles new user registration
        [HttpPost]
        public IActionResult Register(User user)
        {
            // Check if email already exists
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "Email already registered";
                return View();
            }

            // Set default role and activation status
            user.Role = "Customer";
            user.IsActive = true;

            // Save new user to database
            _context.Users.Add(user);
            _context.SaveChanges();

            // Redirect to login after successful registration
            return RedirectToAction("Login");
        }
    }
}
