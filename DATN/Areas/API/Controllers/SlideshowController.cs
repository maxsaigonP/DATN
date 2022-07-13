using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SlideshowController : ControllerBase
    {

            private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public string Url = "D:\\Do An Tot Ngiep\\DATN\\src\\assets\\img\\slide";

        public SlideshowController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]

        public async Task<IActionResult> GetSlideshow()
        {
            var result= (from a in _context.SlideShow
                         select new
                         {
                             Id=a.Id,
                             Link=a.Link,
                             Image=a.Image,
                             Active=a.Active,
                         }).ToList();  
            
            return Ok(result);
        }

        [HttpGet]

        public async Task<IActionResult> GetSlideshowActive()
        {
            var result = (from a in _context.SlideShow
                          where a.Active== true
                          select new
                          {
                              Id = a.Id,
                              Link = a.Link,
                              Image = a.Image,
                              Active = a.Active,
                          }).ToList();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSlideshowDetail(int id)
        {
            var result = (from a in _context.SlideShow
                          where a.Id==id
                          select new
                          {
                              Id = a.Id,
                              Link = a.Link,
                              Image = a.Image
                          }).FirstOrDefault();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostSlideshow(SlideShow slide)
        {
            if(ModelState.IsValid)
            {
                slide.Image = "";
                _context.Add(slide);

                await _context.SaveChangesAsync();
                return Ok(
                    new
                    {
                        status=200,
                        msg="Thêm thành công",
                        id=slide.Id
                    });
            }
            return BadRequest();

           
        }

        [HttpPost]
        public async Task<IActionResult> EditSlideshow(int id,SlideShow slide)
        {

            var sl = await _context.SlideShow.FindAsync(id);
            if (ModelState.IsValid)
            {
                if(sl!=null)
                {
                    sl.Link = slide.Link;
                    _context.Update(sl);
                  await  _context.SaveChangesAsync();
                }
                return Ok(
                    new
                    {
                        status = 200,
                    });
            }
            return BadRequest();


        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(int id1)
        {

            try
            {
         
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
              
             
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fname = id1.ToString()+"." + fileName.Split('.')[1];
                        var path = Path.Combine(Url, fname);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                    
                        var pro = await _context.SlideShow.FindAsync(id1);
                        pro.Image = fname;
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
            return BadRequest();
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> UpdateImage(int id)
        {


            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');


                    var fname = id.ToString() + "." + fileName.Split('.')[1];
                    var pro = await _context.SlideShow.FindAsync(id);


                    //


                    string Path1 = Url + pro.Image;
                    if (pro.Image != null && pro.Image != "")
                    {
                        FileInfo file1 = new FileInfo(Path1);
                        if (file1.Exists)
                        {
                            file1.Delete();
                        }
                    }
                    var path = Path.Combine(Url, fname);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    pro.Image = fname;
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

        [HttpPost]

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.SlideShow.FindAsync(id);
            if (product.Image!= null)
            {
                var fileDelete = Path.Combine(Url, product.Image);
                FileInfo file = new FileInfo(fileDelete);
                file.Delete();
            }
            _context.SlideShow.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                status = 200
            });
        }

        [HttpPost]

        public async Task<IActionResult> Active(int id)
        {
            var product = await _context.SlideShow.FindAsync(id);
            if (product.Image != null)
            {
               if(product.Active!=true)
                {
                    product.Active = true;
                }else
                {
                    product.Active = false;
                }
               _context.Update(product);
            }
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    Status = 200
                });
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
    }
}
