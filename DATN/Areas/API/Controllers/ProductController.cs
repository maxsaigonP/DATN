using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

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
                          join b in _context.TradeMarks on a.TradeMarkId equals b.Id
                          select new
                          {
                              Id=a.Id,
                              Name = a.Name,
                              Category = a.Category.Name,
                              Price = a.Price,
                              Description = a.Description,
                              Stock = a.Quantily,
                              TradeMark = b.Name,
                              Star = a.Star,
                              Image = a.Image,
                              HardDisk=a.HardDisk,
                              Port=a.Port,
                              Os=a.OS,
                              ReleaseTime=a.ReleaseTime,
                          }).ToArray();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> ShowNews()
        {
            var result = (from a in _context.Product
                          orderby a.Id descending
                          select new
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Category = a.Category.Name,
                              Price = a.Price,
                              Description = a.Description,
                              Stock = a.Quantily,
                              TradeMark = a.TradeMark,
                              Star = a.Star,
                              Image = a.Image
                          }).Take(6).ToArray();
            return Ok(result);
        }
        [HttpGet]
        public async Task<ActionResult> ShowTop()
        {
            var result = (from a in _context.Product
                          orderby a.Star descending
                          select new
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Category = a.Category.Name,
                              Price = a.Price,
                              Description = a.Description,
                              Stock = a.Quantily,
                              TradeMark = a.TradeMark,
                              Star = a.Star,
                              Image = a.Image
                          }).Take(6).ToArray();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> Detail(int id)
        {
           

            var result= (from a in _context.Product
                         join b in _context.TradeMarks on a.TradeMarkId equals b.Id
                         where a.Id==id
                         select new
                         {
                             Id=a.Id,
                             Name=a.Name,
                             CategoryId=a.CategoryId,
                             Category=a.Category.Name,
                             Price= a.Price,
                             Description=a.Description,
                             Stock=a.Quantily,
                             TradeMark=b.Name,
                             TradeMarkId=b.Id,
                             Star=a.Star,
                             Image=a.Image,
                             CPU=a.CPU,
                             DesignStyle=a.DesignStyle,
                             Monitor=a.Monitor,
                             RAM=a.RAM,
                             SizeWeight=a.SizeWeight,
                             VGA=a.VGA,
                             HardDisk=a.HardDisk,
                             Port=a.Port,
                             OS=a.OS,
                             ReleaseTime=a.ReleaseTime,

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
        public async Task<IActionResult> Update(int id,[FromBody] Product product)
        {

         

            if (ModelState.IsValid)
            {
                try
                {
                    var npro = await _context.Product.FindAsync(id);

                    npro.Name = product.Name;
                    npro.Description = product.Description;
                    npro.Price = product.Price;
                    npro.TradeMark = product.TradeMark;
                    npro.Quantily = product.Quantily;
                    npro.Star = product.Star;
                    npro.CPU = product.CPU;
                    npro.RAM = product.RAM;
                    npro.DesignStyle = product.DesignStyle;
                    npro.SizeWeight = product.SizeWeight;
                    npro.VGA = product.VGA;
                    npro.Status = true;
                    npro.CategoryId = product.CategoryId;
                    npro.Monitor = product.Monitor;

                    _context.Update(npro);   
                  
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
                    TradeMarkId = product.TradeMarkId,
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
                    HardDisk=product.HardDisk,
                    OS=product.OS,
                    Port=product.Port,
                    ReleaseTime=product.ReleaseTime,
                    

                };


                _context.Add(npro);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    status=200,
                    id=npro.Id,
                });
            }

            return Ok();
        }


        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Testimg()
        {

            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                   var fileName=ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var path = Path.Combine(Url, fileName);
                    using (var stream = new FileStream(path,FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var id=int.Parse(fileName.Split('.')[0]);
                    var pro=await _context.Product.FindAsync(id);
                    pro.Image = fileName;
                    _context.Update(pro);
                    await _context.SaveChangesAsync();
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
