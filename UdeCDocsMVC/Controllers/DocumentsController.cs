using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NuGet.Packaging.Signing;
using UdeCDocsMVC.Models;
using UdeCDocsMVC.Models.SysModels;

namespace UdeCDocsMVC.Controllers
{

    public class DocumentsController : Controller
    {
        private readonly UdecDocsContext _context;
        private readonly IWebHostEnvironment _environment;

        public DocumentsController(UdecDocsContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        //Get document details
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
            List<Vote> upvotes = await _context.Votes.Where(v => v.Iddocument == id && v.IdtypeVote == 1).ToListAsync();
            List<Vote> downvotes = await _context.Votes.Where(v => v.Iddocument == id && v.IdtypeVote == 2).ToListAsync();
            List<Comment> comments = await _context.Comments.Where(c => c.Iddocument == id).ToListAsync();
            document.Comments = comments;
            ViewData["upvotes"] = upvotes.Count;
            ViewData["downvotes"] = downvotes.Count;
            TempData["Iddocument"] = id;
            return View(document);
        }

        //Return view create document
        [Authorize(Policy = "RequireUdeCUserRole")]
        public IActionResult Create()
        {
            User user = _context.Users.Find(Int32.Parse(User.FindFirst("Iduser").Value));
            ViewData["Idfield"] = new SelectList(_context.Fields.Where(f => f.Idfaculty == user.Idfaculty), "Idfield", "Field1");
            return View();
        }

        //Upload document method=post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireUdeCUserRole")]
        public async Task<IActionResult> Create([Bind("Name,Abstract,Keywords,Authors,Direction,Idfield")] CDocument document)
        {
            using (_context)
            {
                if (document != null)
                {

                    string documentName = document.Direction.FileName;
                    var guid = Guid.NewGuid();
                    string documentNameDB = documentName + guid + ".pdf";
                    var fileName = System.IO.Path.Combine(_environment.ContentRootPath, "\\wwwroot\\Uploads", documentName + guid + ".pdf");
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
                        Direction = documentNameDB,
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

        //Edit document
        // GET: Documents/Edit/5
        [Authorize(Policy = "RequireUdeCUserRole")]
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
            ViewData["Idfield"] = new SelectList(_context.Fields, "Idfield", "Field1", document.Idfield);
            return View(document);
        }

        //Edit document
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireUdeCUserRole")]
        public async Task<IActionResult> Edit(string Name, string Abstract, string Keywords, string Authors, int id)
        {
            if (!_context.Documents.Any(e => e.Iddocument == id))
            {
                return NotFound();
            }

            int Iduser = Int32.Parse(User.FindFirst("Iduser").Value);

            var document = await _context.Documents.FindAsync(id);

            if (Name != null || Abstract != null || Keywords != null || Authors != null)
            {
                if (document.Iduser == Iduser)
                {
                    document.Name = Name;
                    document.Abstract = Abstract;
                    document.Keywords = Keywords;
                    document.Authors = Authors;
                    _context.Update(document);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", "Users", new { id = Iduser });
                }
            }
            ViewData["Idfield"] = new SelectList(_context.Fields, "Idfield", "Field1", document.Idfield);
            return View(document);
        }

        //Delete document
        // GET: Documents/Delete/5
        [Authorize(Policy = "RequireUdeCUserRole")]
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

        //Delete document
        // POST: Documents/Delete/5
        [Authorize(Policy = "RequireUdeCUserRole")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Documents == null)
            {
                return Problem("Entity set 'UdeCDocsContext.Documents'  is null.");
            }
            var document = await _context.Documents.FindAsync(id);

            int Iduser = Int32.Parse(User.FindFirst("Iduser").Value);

            if (document != null)
            {
                if(document.Iduser == Iduser) 
                {
                    var listComments = await _context.Comments.Where(c => c.Iddocument == id).ToListAsync();
                    var listVotes = await _context.Votes.Where(v => v.Iddocument == id).ToListAsync();
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

                    var fileName = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot\\Uploads", document.Direction);
                    try
                    {
                        FileInfo file = new FileInfo(fileName);
                        file.Delete();
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    _context.Documents.Remove(document);

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Users", new { id = Iduser });
                }
                
            }

            return RedirectToAction("Index", "Home");
        }

        //Create comment for document
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireRegistered")]
        public async Task<IActionResult> CreateComment(string body)
        {
            int Iddocument = Int32.Parse(TempData["Iddocument"].ToString());
            if (body != null)
            {
                int Iduser = Int32.Parse(User.FindFirst("Iduser").Value);
                var user = await _context.Users.Where(u => u.Iduser == Iduser).SingleAsync();
                DateTime date = DateTime.Now;
                Comment comment = new Comment
                {
                    Body = body,
                    Date = date,
                    Iddocument = Int32.Parse(TempData["Iddocument"].ToString()),
                    Iduser = Iduser,
                    UserW = user.Name
                };
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = Iddocument });
            }
            return RedirectToAction(nameof(Details), new { id = Iddocument });
        }

        [Authorize(Policy = "RequireRegistered")]   
        //Delete comment from document
        public async Task<IActionResult> DeleteComment(int? id)
        {
            var comment = await _context.Comments.Where(c => c.Idcomment == id).SingleAsync();
            int Iduser = Int32.Parse(User.FindFirst("Iduser").Value);
            if (Iduser == comment.Iduser)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "Users", new { id = Iduser });
        }

        //Upvote document
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireRegistered")]
        public async Task<IActionResult> Upvote()
        {
            int Iduser = Int32.Parse(User.FindFirst("Iduser").Value);
            int Iddocument = Int32.Parse(TempData["Iddocument"].ToString());
            if(_context.Votes.Any(v => v.Iduser == Iduser && v.Iddocument == Iddocument && v.IdtypeVote == 2)) 
            {
                var vote = _context.Votes.Where(v => v.Iduser == Iduser && v.Iddocument == Iddocument).Single();
                vote.IdtypeVote = 1;
                _context.Update(vote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = Iddocument });
            }
            if (!_context.Votes.Any(v => v.Iduser == Iduser && v.Iddocument == Iddocument))
            {
                Vote vote = new Vote
                {
                    Value = 1,
                    IdtypeVote = 1,
                    Iddocument = Iddocument,
                    Iduser = Iduser
                };
                _context.Votes.Add(vote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = Iddocument });
            }
            return RedirectToAction(nameof(Details), new { id = Iddocument });

        }

        //Downvote document
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireRegistered")]
        public async Task<IActionResult> Downvote()
        {
            int Iduser = Int32.Parse(User.FindFirst("Iduser").Value);
            int Iddocument = Int32.Parse(TempData["Iddocument"].ToString());
            if (_context.Votes.Any(v => v.Iduser == Iduser && v.Iddocument == Iddocument && v.IdtypeVote == 1))
            {
                var vote = _context.Votes.Where(v => v.Iduser == Iduser && v.Iddocument == Iddocument).Single();
                vote.IdtypeVote = 2;
                _context.Update(vote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = Iddocument });
            }
            if (!_context.Votes.Any(v => v.Iduser == Iduser && v.Iddocument == Iddocument))
            {
                Vote vote = new Vote
                {
                    Value = 1,
                    IdtypeVote = 2,
                    Iddocument = Iddocument,
                    Iduser = Iduser
                };
                _context.Votes.Add(vote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = Iddocument });
            }
            return RedirectToAction(nameof(Details), new { id = Iddocument });

        }

        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.Iddocument == id);
        }
    }
}
