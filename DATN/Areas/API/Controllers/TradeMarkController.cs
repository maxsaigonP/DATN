using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TradeMarkController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public TradeMarkController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]

        public async Task<IActionResult> GetAllTradeMark()
        {
            var result= (from a in _context.TradeMarks
                         select new
                         {
                             Id=a.Id,
                             Name=a.Name,
                         }).ToList();

            return Ok(result);
        }

        [HttpGet]

        public async Task<IActionResult> GetTradeMarkById(int id)
        {
            var result = (from a in _context.TradeMarks
                          where a.Id == id
                          select new
                          {
                              Id = a.Id,
                              Name = a.Name,
                          }).ToList();

            return Ok(result);
        }



        [HttpPost]

        public async Task<IActionResult> PostTradeMark(string name)
        {
            var tradeMark=new TradeMark();
            tradeMark.Name = name;
            _context.TradeMarks.Add(tradeMark);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    status=200,
                    msg="Thêm thành công"
                });
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest();
            

           ;    
        }

        [HttpPost]

        public async Task<IActionResult> EditTradeMark(TradeMark tradeMark)
        {
            var check= await _context.TradeMarks.FindAsync(tradeMark.Id);
            if(check==null)
            {
                return BadRequest();
            }
            if(check.Name.Equals(tradeMark.Name))
            {
                return Ok(new
                {
                    status = 500,
                    msg = "Nhãn hiệu đã tồn tại"
                });
            }

            
            _context.TradeMarks.Add(tradeMark);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    status = 200,
                    msg = "Cập nhật thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest();


            
        }

        [HttpPost] 
         public async Task<IActionResult> RemoveTradeMark(int id)
        {
            var check = await _context.TradeMarks.FindAsync(id);
            if(check==null)
            {
                return BadRequest();
            }
            _context.TradeMarks.Remove(check);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = 200,
                msg = "Xoá thành công"
            });
        }
    }
}
