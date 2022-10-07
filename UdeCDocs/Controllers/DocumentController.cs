using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UdeCDocs.Models.Response;
using UdeCDocs.Models;

namespace UdeCDocs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        [HttpGet("{Iduser}")]
        public IActionResult GetDocument(int Iduser)
        {
            Response response = new Response();
            try
            {
                using (UdeCDocsContext db = new UdeCDocsContext())
                {
                    var user = db.Users.Find(Iduser);
                    var comments = db.Comments.Where(c => c.Iduser == Iduser).ToList();
                    user.Comments = comments;
                    response.State = 1;
                    response.Data = user;
                }
            }
            catch (Exception ex) 
            {
                response.Message = ex.Message;
            }
            
            return Ok(response);
        }
    }
}
