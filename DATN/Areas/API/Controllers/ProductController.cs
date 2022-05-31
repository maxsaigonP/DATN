using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> Show()
        {
            var pro = await _context.Product.ToListAsync();
            return pro;
        }

        [HttpGet]
        public async Task<ActionResult> Detail(int id)
        {
            var pro = await _context.Product.FindAsync(id);

            var result= (from a in _context.Product
                         where a.Id==id
                         select new
                         {
                             Name=a.Name,
                             Category=a.Category,
                             Price= a.Price,
                             Description=a.Description,
                             Stock=a.Quantily,
                             TradeMark=a.TradeMark,
                             Star=a.Star,
                             Image=a.Image
                         }).ToArray();

            var result1 = (from a in _context.Comment
                           join b in _context.Product on a.ProductId equals b.Id
                          where b.Id == id
                          select new
                          {
                               User= (from c in _context.Comment
                                      join d in _context.AppUsers on c.AppUserId equals d.Id
                                      where c.ProductId==id
                                      select d.UserName),
                               Content= a.Content,
                               Star=a.Star
                          }).ToArray();
            return Ok(new
            {
                result,
                result1,

            });
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> Search(string txtSearch)
        {
            txtSearch = txtSearch.ToLower();
            var pro = await _context.Product.Where(p=>p.Name.ToLower().Contains(txtSearch)).ToListAsync();
            return pro;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Product product)
        {
            var pro = await _context.Product.Where(p => p.Name == product.Name).ToListAsync();
            if(pro!=null)
            {
                return BadRequest("Sản phẩm đã tồn tại");
            }
            if (ModelState.IsValid)
            {

                _context.Add(product);
                await _context.SaveChangesAsync();
                if (product.ImageFile != null)
                {
                    var fileName = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
                    var uploadPath = Path.Combine("C:\\Users\\BAO PHUC- PC\\OneDrive\\Desktop\\Hmart - Electronics eCommerce HTML Template\\hmart\\assets\\images\\product-image");
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        product.ImageFile.CopyTo(fs);
                        fs.Flush();
                    }
                    product.Image = fileName;
                    product.Star = 5;
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    var img = new Images();
                    img.Image = fileName;
                    img.ProductId= product.Id;
                   
                    _context.Add(img);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
           
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,[FromForm] Product product)
        {

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    if (product.ImageFile != null)
                    {
                        var fileDelete = Path.Combine("C:\\Users\\BAO PHUC- PC\\OneDrive\\Desktop\\Hmart - Electronics eCommerce HTML Template\\hmart\\assets\\images\\product-image", product.Image);
                        FileInfo file = new FileInfo(fileDelete);
                        file.Delete();
                    }
                    if (product.ImageFile != null)
                    {
                        var fileName = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
                        var uploadPath = Path.Combine("C:\\Users\\BAO PHUC- PC\\OneDrive\\Desktop\\Hmart - Electronics eCommerce HTML Template\\hmart\\assets\\images\\product-image");
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            product.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                        product.Image = fileName;
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
                return RedirectToAction(nameof(Index));
            }
           
            return Ok("Update success");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product.ImageFile != null)
            {
                var fileDelete = Path.Combine("C:\\Users\\BAO PHUC- PC\\OneDrive\\Desktop\\Hmart - Electronics eCommerce HTML Template\\hmart\\assets\\images\\product-image", product.Image);
                FileInfo file = new FileInfo(fileDelete);
                file.Delete();
            }
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return Ok("Delete success");
        }
    }
}
