using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Models;
using System.Diagnostics;

namespace PharmacyManagement.Controllers
{
    // Controller for public and common pages
    public class HomeController : Controller
    {
        // =========================
        // HOME PAGE
        // =========================
        // Displays the main landing page
        public IActionResult Index()
        {
            return View();
        }

        // =========================
        // PRIVACY PAGE
        // =========================
        // Displays privacy policy information
        public IActionResult Privacy()
        {
            return View();
        }

        // =========================
        // ERROR PAGE
        // =========================
        // Displays a friendly error page with request tracking information
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                // Use current activity ID if available, otherwise use HTTP trace identifier
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
