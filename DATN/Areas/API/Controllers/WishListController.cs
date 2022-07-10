using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public WishListController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost]
        public async Task<IActionResult> PostWishList([FromBody] WishList wish)
        {
            if (ModelState.IsValid)
            {
                
                var wishl = await _context.WishList.Where(w => w.AppUserId == wish.AppUserId && w.ProductId == wish.ProductId).FirstOrDefaultAsync();
                if (wishl != null)
                {
                    _context.WishList.Remove(wishl);
                   await _context.SaveChangesAsync();
                    return Ok(new
                    {
                        Status=200,
                        msg= "Đã xoá khỏi yêu thích"
                    });
                }
                else
                {
                    _context.WishList.Add(wish);
                    await _context.SaveChangesAsync();
                    return Ok(new
                    {
                        Status = 200,
                        msg = "Đã thêm vào yêu thích"
                    });
                }
            

            }
            return BadRequest(ModelState);
        }


        [HttpGet]
        
        public async Task<IActionResult> GetWishList(string userID)
        {
            var ds = await _context.WishList.ToListAsync();
            var result= (from a in _context.WishList
                         join b in _context.Product on a.ProductId equals b.Id
                         where a.AppUserId==userID
                         select new
                         {
                             Id=a.Id,
                             Image=b.Image,
                             IdSanPham=a.ProductId,
                             TenSanPham=b.Name,
                             Gia=b.SalePrice==0?b.Price:b.SalePrice,
                             Soluong= (from c in _context.WishList
                                       where c.AppUserId==userID
                                       select c).Count()
                         }).ToList();

            return Ok(result);
        }


        [HttpPost]

        public async Task<IActionResult> RemoveWishList(int id)
        {
            var wish=await _context.WishList.FindAsync(id);
            if (wish != null)
            {
                _context.WishList.Remove(wish);
                await _context.SaveChangesAsync();
                return Ok("Đã xoá khỏi yêu thích");
                
            }

            return BadRequest("Xoá thất bại");
        }

    }
}
