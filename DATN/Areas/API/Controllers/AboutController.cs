using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AboutController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]

        public async Task<IActionResult> GetAllAbout()
        {
            var result = (from a in _context.About
                          select new
                          {
                              Title = a.Title,
                              Content = a.Content,
                          }).ToList();

            return Ok(result);
        }

        [HttpPost]

        public async Task<IActionResult> PostAbout(About about)
        {
            if(about.Title!=""&&about.Content!="")
            {
                
                try
                {
                    _context.Add(about);
                    await _context.SaveChangesAsync();
                    return Ok(new
                    {
                        status = 200,
                        msg = "Them thanh cong"
                    });
                }catch (Exception ex)
                {
                    return BadRequest(ex.Message);  
                }
            }
            return BadRequest();
        }

        [HttpPost]

        public async Task<IActionResult> DeleteAbout(int id)
        {
            var ab = await _context.About.FindAsync(id);
            if(ab!=null)
            {
                try
                {
                    _context.About.Remove(ab);
                    await _context.SaveChangesAsync();
                    return Ok(new
                    {
                        status = 200,
                        msg = "Xoa thanh cong"
                    });
                }catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
               
            }
            return BadRequest();
        }
    }
}
