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
        public string Url = "C:\\Users\\BAO PHUC- PC\\DATN\\src\\assets\\img\\product";

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<ActionResult> Show1()
        {
            var result = await _context.Product.ToListAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> Show()
        {
            var result = (from a in _context.Product
                          select new
                          {
                              Id=a.Id,
                              Name = a.Name,
                              Category = a.Category.Name,
                              Price = a.Price,
                              Description = a.Description,
                              Stock = a.Quantily,
                              TradeMark = a.TradeMark,
                              Star = a.Star,
                              Image = a.Image
                          }).ToArray();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> Detail(int id)
        {
           

            var result= (from a in _context.Product
                         where a.Id==id
                         select new
                         {
                             Name=a.Name,
                             Category=a.Category.Name,
                             Price= a.Price,
                             Description=a.Description,
                             Stock=a.Quantily,
                             TradeMark=a.TradeMark,
                             Star=a.Star,
                             Image=a.Image
                         }).FirstOrDefault();

         
            return Ok(
            
                result

            );
        }

        [HttpGet]
        public async Task<ActionResult> Detail1(int id)
        {
            var pro = _context.Product.Find(id);

          


            return Ok(

                pro

            );
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> Search(string txtSearch)
        {
            txtSearch = txtSearch.ToLower();
            var pro = await _context.Product.Where(p=>p.Name.ToLower().Contains(txtSearch)).ToListAsync();
            return pro;
        }


        [HttpPost]
        public async Task<IActionResult> PostProduct([FromForm] Product product)
        {
           
            var pro = await _context.Product.Where(p => p.Name == product.Name).ToListAsync();
            var trade= await _context.TradeMarks.Where(t=>t.Name.ToUpper().Equals(product.TradeMark.ToUpper())).ToListAsync();
            if(pro.Count!=0)
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
                    var uploadPath = Path.Combine(Url);
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        product.ImageFile.CopyTo(fs);
                        fs.Flush();
                    }
                    product.Image = fileName;
                    product.Star = 5;
                    if (trade.Count == 0)
                    {
                        var tr = new TradeMark();
                        tr.Name = product.TradeMark.ToUpper();
                        _context.TradeMarks.Add(tr);
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    var img = new Images();
                    img.Image = fileName;
                    img.ProductId= product.Id;
                   
                    _context.Add(img);
                    await _context.SaveChangesAsync();
                }
                return Ok();
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
                        var fileDelete = Path.Combine(Url, product.Image);
                        FileInfo file = new FileInfo(fileDelete);
                        file.Delete();
                    }
                    if (product.ImageFile != null)
                    {
                        var fileName = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
                        var uploadPath = Path.Combine(Url);
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
