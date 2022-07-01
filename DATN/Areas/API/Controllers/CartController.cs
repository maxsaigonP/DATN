using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CartController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart(string userid)
        {   
            var result = (from a in _context.Cart
                          join b in _context.Product on a.ProductId equals b.Id
                          where a.AppUserId == userid
                          select new
                          {
                              Id=a.Id,
                              Image=b.Image,
                              TenSanPham = b.Name,
                              IdSanPham = a.ProductId,
                              SoLuong = a.Quantity,
                              GiaSanPham=b.Price,
                              Gia =b.SalePrice==0?b.Price:b.SalePrice * a.Quantity,
                              SoLuongCart= (from c in _context.Cart
                                            where c.AppUserId == userid
                                            select c).Count()
                          }).ToList();

            var total = result.Sum(s => s.Gia);
            return Ok(new
            {
                cart= result,
                total=total,
            });
        }


        [HttpPost]

        public async Task<IActionResult> AddCart(int sanPham,int soLuong,string userID)
        {
            var check = await _context.Cart.Where(c => c.ProductId == sanPham&&c.AppUserId==userID).FirstOrDefaultAsync();
            var pro = await _context.Product.FindAsync(sanPham);
           
            if (soLuong > pro.Quantily)
            {
                return Ok(new
                {
                    status=500,
                    msg="Số lượng sản phẩm không đủ"
                });
            }
            
            if (check != null)
            {
                check.Quantity += soLuong;
                _context.Update(check);
                await _context.SaveChangesAsync();
            }
            else
            {
                var cart = new Cart();
                cart.AppUserId = userID;
                cart.ProductId = sanPham;
                cart.Quantity = soLuong;
                cart.Status = false;
                _context.Add(cart);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                status=200,
                msg="Đã thêm vào giỏ hàng"
            });
        }


        [HttpPost]

        public async Task<IActionResult> UpdateCart(int cartID,int sl)
        {
            var cart = await _context.Cart.FindAsync(cartID);
            var proid = cart.ProductId;
            var pro = await _context.Product.FindAsync(proid);
            cart.Quantity+=sl;
            if(cart.Quantity>pro.Quantily)
            {
                return BadRequest("Số lượng sản phẩm không đủ");
            }
            if(cart.Quantity<=0)
            {
                cart.Quantity = 0;
            }
            _context.Update(cart);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status=200
            });
        }

        [HttpPost]

        public async Task<IActionResult> RemoveCart(int cartID)
        {
            var cart = await _context.Cart.FindAsync(cartID);

            if (cart!=null)
            {
                _context.Cart.Remove(cart);
                await _context.SaveChangesAsync();
                return Ok("Da xoa khoi gio hang");
            }
           
            

            return BadRequest();
        }

        [HttpPost]

        public async Task<IActionResult> RemoveAllCart(string userID)
        {
            var cart = await _context.Cart.Where(c=>c.AppUserId==userID).ToListAsync();

            if (cart != null)
            {
                foreach(var c in cart)
                {
                    _context.Remove(c);
                }
              
                await  _context.SaveChangesAsync();
                return Ok(new
                {
                    status=200,
                    msg="Đã xoá toàn bộ giỏ hàng "
                });
            }



            return BadRequest();
        }


     

       
    }
}
