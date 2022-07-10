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
        public async Task<IActionResult> GetAllComment()
        {
            var result= (from a in _context.Comment
                         join b in _context.Product on a.ProductId equals b.Id
                         join c in _context.AppUsers on a.AppUserId equals c.Id
                         select new
                         {
                             Id=a.Id,
                             UserId=a.AppUserId,
                             UserName=c.UserName,
                             NoiDung=a.Content,
                             IdSanPham=a.ProductId,
                             TenSanPham=b.Name,
                             DanhGia=a.Star,
                             ThoiGian=a.Time
                         }).ToList();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCommentInProduct(int id)
        {
            var result = (from a in _context.Comment
                          join b in _context.Product on a.ProductId equals b.Id
                          join c in _context.AppUsers on a.AppUserId equals c.Id
                          where a.ProductId == id
                          select new
                          {
                              Id = a.Id,
                              UserId=a.AppUserId,
                              NoiDung = a.Content,
                              IdSanPham = a.ProductId,
                              TenSanPham = b.Name,
                              DanhGia = a.Star,
                              ThoiGian = a.Time,
                              TenNguoiDung=c.FullName,
                    
                              RepId=a.ReplyId
                          }).ToList();
            return Ok(result);
        }

        [HttpPost]

        public async Task<IActionResult> AddComment(string userID, string content, int star, int productID,int replyID)
        {
            var comment = await _context.Comment.Where(c => c.AppUserId == userID && c.ProductId == productID&&c.ReplyId==0).FirstOrDefaultAsync();

            if(replyID<=0)
            {

            
            if(comment!=null)
            {
                comment.Content = content;
                comment.Star = star;
                comment.Time = DateTime.Now;
                _context.Comment.Update(comment);
              
                var rate1 =  _context.Comment.Where(c => c.ProductId == productID&&c.ReplyId==0).ToList();
                var avgStar1 = rate1.Average(c => c.Star);

                var pro1 = _context.Product.Find(productID);
                pro1.Star = Math.Round(avgStar1, 1);
                _context.Product.Update(pro1);
               await _context.SaveChangesAsync();
                return Ok(new
                {
                    status = 200,
                    msg = "Đã gửi bình luận"
                });


            }
            }

            var cmt = new Comment();
            cmt.AppUserId = userID;
            cmt.Content = content;
            cmt.Star = star;
            cmt.Time = DateTime.Now;
            cmt.Status = true;
            cmt.ProductId = productID;
            cmt.ReplyId = replyID;
            _context.Comment.Add(cmt);
            await _context.SaveChangesAsync();


            var rate = await _context.Comment.Where(c => c.ProductId == productID&&c.ReplyId==0).ToListAsync();
            var avgStar = rate.Average(c => c.Star);

            var pro = await _context.Product.FindAsync(productID);
            pro.Star =Math.Round(avgStar,1);
            _context.Product.Update(pro);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status=200,
                msg="Đã gửi bình luận"
            });
        }

        [HttpPost]

        public async Task<IActionResult> RemoveComment(int commentID)
        {
            var cmt = await _context.Comment.FindAsync(commentID);
            
            if(cmt!=null)
            {
                var subCmt = await _context.Comment.Where(c => c.ReplyId == commentID && c.ProductId == cmt.ProductId).ToListAsync();
                if (subCmt.Count>0)
                {
                    foreach(var c in subCmt)
                    {
                        _context.Comment.Remove(c);
                        _context.SaveChanges();
                    }
                }
               
                _context.Comment.Remove(cmt);
                await _context.SaveChangesAsync();

                var productID = cmt.ProductId;
                var rate = await _context.Comment.Where(c => c.ProductId == productID&&c.ReplyId==0).ToListAsync();
                var avgStar = rate.Average(c => c.Star);

                var pro = await _context.Product.FindAsync(productID);
                pro.Star = Math.Round(avgStar, 1);
                _context.Product.Update(pro);
                await _context.SaveChangesAsync();
                return Ok(
                    new
                    {
                        status=200
                    });
            }
            

            return BadRequest();
        }
        [HttpPost]

        public async Task<IActionResult> RemoveCommentUser(int commentID,string userID)
        {
            var cmt = await _context.Comment.FindAsync(commentID);

            if (cmt != null&&cmt.AppUserId==userID)
            {
                var subCmt = await _context.Comment.Where(c => c.ReplyId == commentID && c.ProductId == cmt.ProductId).ToListAsync();
                if (subCmt.Count > 0)
                {
                    foreach (var c in subCmt)
                    {
                        _context.Comment.Remove(c);
                        _context.SaveChanges();
                    }
                }

                _context.Comment.Remove(cmt);
                await _context.SaveChangesAsync();

                var productID = cmt.ProductId;
                var rate = await _context.Comment.Where(c => c.ProductId == productID&&c.ReplyId==0).ToListAsync();
                var avgStar = rate.Average(c => c.Star);

                var pro = await _context.Product.FindAsync(productID);
                pro.Star = Math.Round(avgStar, 1); ;
                _context.Product.Update(pro);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    Status=200,
                    msg="Đã xoá bình luận"

                });
            }


            return BadRequest();
        }
    }
}
