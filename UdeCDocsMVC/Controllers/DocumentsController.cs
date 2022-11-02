using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using UdeCDocsMVC.Models;
using UdeCDocsMVC.Models.SysModels;

namespace UdeCDocsMVC.Controllers
{
    
    public class DocumentsController : Controller
    {
        private readonly UdeCDocsContext _context;
        private readonly IWebHostEnvironment _environment;

        public DocumentsController(UdeCDocsContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            var udeCDocsContext = _context.Documents.Include(d => d.IdfieldNavigation);
            return View(await udeCDocsContext.ToListAsync());
        }

        // GET: Documents/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.IdfieldNavigation)
                .FirstOrDefaultAsync(m => m.Iddocument == id);
            if (document == null)
            {
                return NotFound();
            }
            TempData["Iddocument"] = id;
            return View(document);
        }

        // GET: Documents/Create
        [Authorize(Policy = "RequireUdeCUserRole")]
        public IActionResult Create()
        {
            User user = _context.Users.Find(Int32.Parse(User.FindFirst("Iduser").Value));
            ViewData["Idfield"] = new SelectList(_context.Fields.Where(f => f.Idfaculty == user.Idfaculty), "Idfield", "Field1");
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireUdeCUserRole")]
        public async Task<IActionResult> Create([Bind("Name,Abstract,Keywords,Authors,Direction,Idfield")] CDocument document)
        {
            using(_context)
            {
                if (document != null)
                {

                    string documentName = document.Direction.FileName;
                    var guid = Guid.NewGuid();
                    var fileName = System.IO.Path.Combine(_environment.ContentRootPath,"wwwroot\\Uploads", documentName + guid);
                    await document.Direction.CopyToAsync(new System.IO.FileStream(fileName, System.IO.FileMode.Create));
                    int Iduser = Int32.Parse(User.FindFirst("Iduser").Value);
                    DateTime date = DateTime.Now;
                    Document document1 = new Document
                    {
                        Name = document.Name,
                        Abstract = document.Abstract,
                        Keywords = document.Keywords,
                        PublicationDate = date,
                        Authors = document.Authors,
                        Direction = fileName,
                        Idfield = document.Idfield,
                        Iduser = Iduser
                    };
                    _context.Documents.Add(document1);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                User user = _context.Users.Find(Int32.Parse(User.FindFirst("Iduser").Value));
                ViewData["Idfield"] = new SelectList(_context.Fields.Where(f => f.Idfaculty == user.Idfaculty), "Idfield", "Field1", document.Idfield);
                return View(document);
            }
            
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            ViewData["Idfield"] = new SelectList(_context.Fields, "Idfield", "Idfield", document.Idfield);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Iddocument,Name,Abstract,Keywords,PublicationDate,Authors,Direction,Idfield,Iduser")] Document document)
        {
            if (id != document.Iddocument)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Iddocument))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idfield"] = new SelectList(_context.Fields, "Idfield", "Idfield", document.Idfield);
            return View(document);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.IdfieldNavigation)
                .FirstOrDefaultAsync(m => m.Iddocument == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Documents == null)
            {
                return Problem("Entity set 'UdeCDocsContext.Documents'  is null.");
            }
            var document = await _context.Documents.FindAsync(id);
            var listComments = await _context.Comments.Where(c => c.Iddocument == id).ToListAsync();
            var listVotes = await _context.Votes.Where(v => v.Iddocument == id).ToListAsync();
            if (document != null)
            {
                if (listComments != null) 
                {
                    foreach (Comment c in listComments)
                    {
                        _context.Comments.Remove(c);
                    }
                }
                if (listVotes != null)
                {
                    foreach (Vote v in listVotes)
                    {
                        _context.Votes.Remove(v);
                    }
                }
                
                _context.Documents.Remove(document);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireRegistered")]
        public async Task<IActionResult> CreateComment(string body)
        {
            using (_context)
            {
                if (body != null)
                {
                    int Iduser = Int32.Parse(User.FindFirst("Iduser").Value);
                    DateTime date = DateTime.Now;
                    Comment comment = new Comment
                    {
                        Body = body,
                        Date = date,
                        Iddocument = Int32.Parse(TempData["Iddocument"].ToString()),
                        Iduser = Iduser
                    };
                    _context.Comments.Add(comment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireRegistered")]
        public async Task<IActionResult> Upvote()
        {
            using (_context)
            {
                if (true)
                {
                    int Iduser = Int32.Parse(User.FindFirst("Iduser").Value);
                    Vote vote = new Vote
                    {
                        Value = 1,
                        IdtypeVote = 1,
                        Iddocument = Int32.Parse(TempData["Iddocument"].ToString()),
                        Iduser = Iduser
                    };
                    _context.Votes.Add(vote);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }

        }

        private bool DocumentExists(int id)
        {
          return _context.Documents.Any(e => e.Iddocument == id);
        }
    }
}
