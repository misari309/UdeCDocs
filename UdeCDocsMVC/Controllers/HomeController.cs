using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using UdeCDocsMVC.Models;
using UdeCDocsMVC.Models.SysModels;

namespace UdeCDocsMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UdecDocsContext _context;

        public HomeController(ILogger<HomeController> logger, UdecDocsContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var documents = await _context.Documents.ToListAsync();
            foreach (var document in documents) 
            {
                document.Votes = await _context.Votes.Where(v => v.Iddocument == document.Iddocument).ToListAsync();
            }
            HomeModel homeModel = new HomeModel
            {
                Documents = documents
            };
            homeModel.orderDocuments();


            return View(homeModel);
        }

        public async Task<IActionResult> Faculties()
        {
            if (_context.Faculties == null)
            {
                return NotFound();
            }
            List<Faculty> faculties = await _context.Faculties.Where(f => f.Idfaculty < 50).ToListAsync();
            return View(faculties);
        }

        public async Task<IActionResult> Faculty(int? id)
        {
            if (id == null || _context.Faculties == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Idfaculty == id);
            if (faculty == null)
            {
                return NotFound();
            }
            List<Field> fields = await _context.Fields.Where(f => f.Idfaculty == id).ToListAsync();
            for(int i=0; i<fields.Count(); i++) 
            {
                fields.ElementAt(i).Documents = await _context.Documents.Where(d => d.Idfield == fields.ElementAt(i).Idfield).ToListAsync();
                for(int j=0; j < fields.ElementAt(i).Documents.Count(); j++) 
                {
                    List<Vote> votes = await _context.Votes.Where(v => v.Iddocument == fields.ElementAt(i).Documents.ElementAt(j).Iddocument).ToListAsync();
                    fields.ElementAt(i).Documents.ElementAt(j).Votes = votes;
                }
                
                fields.ElementAt(i).Documents = await _context.Documents.Where(d => d.Idfield == fields.ElementAt(i).Idfield).ToListAsync();
            }
            faculty.Fields = fields;
            return View(faculty);
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