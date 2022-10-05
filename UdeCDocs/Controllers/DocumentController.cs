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
                    var lst = db.Users.Find(Iduser);
                    db.Comments.Where(c => c.Iduser == Iduser).ToList().ForEach(c => {
                        lst.Comments.Add(c);
                    });
                    response.State = 1;
                    response.Data = lst;
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
