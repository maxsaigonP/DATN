using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CommentController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IEnumerable<Comment>> Show()
        {
            var pro = await _context.Comment.ToListAsync();
            return pro;
        }

        [HttpPost]

        public async Task<IActionResult> AddComment(string userID, string content, int star, int productID)
        {
            var cmt = new Comment();
            cmt.AppUserId = userID;
            cmt.Content = content;
            cmt.Star = star;
            cmt.Time = DateTime.Now;
            cmt.Status = true;
            _context.Comment.Add(cmt);


            var rate = await _context.Comment.Where(c => c.ProductId == productID).ToListAsync();
            var avgStar = rate.Average(c => c.Star);

            var pro = await _context.Product.FindAsync(productID);
            pro.Star = avgStar;
            _context.Product.Update(pro);
            await _context.SaveChangesAsync();

            return Ok("Đã gửi bình luận");
        }

        [HttpPost]

        public async Task<IActionResult> RemoveComment(int commentID)
        {
            var cmt = await _context.Comment.FindAsync(commentID);
            
            if(cmt!=null)
            {
               
                _context.Comment.Remove(cmt);
                await _context.SaveChangesAsync();

                var productID = cmt.ProductId;
                var rate = await _context.Comment.Where(c => c.ProductId == productID).ToListAsync();
                var avgStar = rate.Average(c => c.Star);

                var pro = await _context.Product.FindAsync(productID);
                pro.Star = avgStar;
                _context.Product.Update(pro);
                await _context.SaveChangesAsync();
                return Ok("Đã xoá bình luận");
            }
            

            return BadRequest();
        }
    }
}
