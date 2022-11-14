using Microsoft.AspNetCore.Mvc;

namespace UdeCDocsMVC.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Http(int statusCode)
        {
            if (statusCode == 404)
                return RedirectToAction("Login", "Users");
            return RedirectToAction("Index", "Home");
        }

    }
}
