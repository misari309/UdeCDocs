using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using UdeCDocsMVC.Models;

namespace UdeCDocsMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UdeCDocsContext _context;

        public HomeController(ILogger<HomeController> logger, UdeCDocsContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var udeCDocsContext = _context.Documents.Include(d => d.IdfieldNavigation);
            return View(await udeCDocsContext.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}