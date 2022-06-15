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
                             Id=a.Id,
                             Name=a.Name,
                             Category=a.Category.Name,
                             Price= a.Price,
                             Description=a.Description,
                             Stock=a.Quantily,
                             TradeMark=a.TradeMark,
                             Star=a.Star,
                             Image=a.Image,
                             CPU=a.CPU,
                             DesignStyle=a.DesignStyle,
                             Monitor=a.Monitor,
                             RAM=a.RAM,
                             SizeWeight=a.SizeWeight,
                             VGA=a.VGA
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
        public async Task<ActionResult> Search(string txtSearch)
        {
            var result = (from a in _context.Product
                          where a.Name.Contains(txtSearch)
                          select new
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Category = a.CategoryId,
                              Price = a.Price,
                              Description = a.Description,
                              Stock = a.Quantily,
                              TradeMark = a.TradeMark,
                              Star = a.Star,
                              Image = a.Image
                          }).ToList();
            return Ok(result);
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
            return Ok(new
            {
                status=200
            });
        }



        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> PostProduct1( ProductCreateModel product)
        {

            var pro = await _context.Product.Where(p => p.Name == product.Name).ToListAsync();
            var trade = await _context.TradeMarks.Where(t => t.Name.ToUpper().Equals(product.TradeMark.ToUpper())).ToListAsync();
            if (pro.Count != 0)
            {
                return BadRequest("Sản phẩm đã tồn tại");
            }
            if (ModelState.IsValid)
            {
                var npro = new Product()
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    TradeMark = product.TradeMark,
                    Quantily = product.Quantily,
                    Star = product.Star,
                    CPU = product.CPU,
                    RAM = product.RAM,
                    DesignStyle = product.DesignStyle,
                    SizeWeight = product.SizeWeight,
                    VGA = product.VGA,
                    Status = true,
                    CategoryId= product.CategoryId,
                    Monitor=product.Monitor,

                };


                _context.Add(npro);
                await _context.SaveChangesAsync();

                //try
                //{
                   

                //    if (ImageFile != null)
                //    {

                //        var fileName = npro.Id.ToString() + Path.GetExtension(ImageFile.FileName);
                //        var uploadPath = Path.Combine(Url);
                //        var filePath = Path.Combine(uploadPath, fileName);
                //        using (FileStream fs = System.IO.File.Create(filePath))
                //        {
                //           ImageFile.CopyTo(fs);
                //            fs.Flush();
                //        }
                //        npro.Image = fileName;
                //        product.Star = 5;
                //        if (trade.Count == 0)
                //        {
                //            var tr = new TradeMark();
                //            tr.Name = product.TradeMark.ToUpper();
                //            _context.TradeMarks.Add(tr);
                //        }
                //        _context.Update(product);
                //        await _context.SaveChangesAsync();

                //        var img = new Images();
                //        img.Image = fileName;
                //        img.ProductId = npro.Id;

                //        _context.Add(img);
                //        await _context.SaveChangesAsync();
                //    }
                //}
                //catch (Exception ex)
                //{

                //}
                return Ok(new
                {
                    status=200
                });
            }

            return Ok();
        }


        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Testimg(ProductCreateModel modle)
        {

            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                   
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
