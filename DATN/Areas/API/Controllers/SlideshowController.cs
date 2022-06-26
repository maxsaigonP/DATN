﻿using DATN.Data;
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
        public string Url = "C:\\Users\\BAO PHUC- PC\\DATN\\src\\assets\\img\\slide";

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
                             Title=a.Title,
                             Content=a.Content,
                             Image=a.Image
                         }).ToList();  
            
            return Ok(result);
        }

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

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
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
                    var path = Path.Combine(Url, fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var id = int.Parse(fileName.Split('.')[0]);
                    var pro = await _context.SlideShow.FindAsync(id);
                    pro.Image = fileName;
                    _context.Update(pro);

                    //var img = new Images();
                    //img.ProductId = id;
                    //img.Image=
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

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> UpdateImage()
        {


            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');


                    var id = int.Parse(fileName.Split('.')[0]);
                    var pro = await _context.SlideShow.FindAsync(id);


                    //

                    string FileName = pro.Image;
                    string Path1 = Url + FileName;
                    FileInfo file1 = new FileInfo(Path1);
                    if (file1.Exists)
                    {
                        file1.Delete();
                    }
                    var path = Path.Combine(Url, fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
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