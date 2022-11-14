using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdeCDocsMVC.Models.API;
using UdeCDocsMVC.Models;

namespace UdeCDocsMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsAPIController : ControllerBase
    {
        private readonly UdecDocsContext _context;

        public DocumentsAPIController(UdecDocsContext context)
        {
            _context = context;
        }
        //GET documento json
        // GET: api/DocumentsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentAPI>> GetDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }
            DocumentAPI documentAPI = new DocumentAPI
            {
                Name = document.Name,
                Abstract = document.Abstract,
                Keywords = document.Keywords,
                PublicationDate = document.PublicationDate,
                Authors = document.Authors,
                Direction = "http://udecdocs.somee.com/Uploads/" + document.Direction
            };

            List<Comment> comments = await _context.Comments.Where(c => c.Iddocument == id).ToListAsync();
            List<CommentAPI> commentAPIs = new List<CommentAPI>();
            foreach (Comment item in comments)
            {
                CommentAPI commentAPIaux = new CommentAPI
                {
                    Body = item.Body,
                    Date = item.Date,
                    UserW = item.UserW
                };
                commentAPIs.Add(commentAPIaux);
            }
            documentAPI.Comments = commentAPIs;
            List<Vote> upvotes = await _context.Votes.Where(c => c.Iddocument == id && c.IdtypeVote == 1).ToListAsync();
            List<Vote> downvotes = await _context.Votes.Where(c => c.Iddocument == id && c.IdtypeVote == 2).ToListAsync();
            documentAPI.Upvotes = upvotes.Count();
            documentAPI.Downvotes = downvotes.Count();
            return documentAPI;
        }
    }
}
