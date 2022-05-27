using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public IActionResult new1 (int proID)
            {
                Comment cmt=new Comment();
            cmt.AppUserId = "b47b8a23-b1bf-4af0-b597-10da59ca73d5";
            cmt.Content = "aaaa";
            cmt.Time= DateTime.Now;
            cmt.ProductId = proID;
            cmt.Star = 5;
            cmt.Status = true;
            return NotFound();
            }

    
    }
}
