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
    public class TradeMarkController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public string Url = "D:\\Do An Tot Ngiep\\DATN\\src\\assets\\img\\brand\\";

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
                             Image=a.Image,
                             Count=(from c in _context.Product
                                    where c.TradeMarkId==a.Id
                                    select c.Id).Count()
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
                              Image=a.Image
                          }).FirstOrDefault();

            return Ok(result);
        }



        [HttpPost]

        public async Task<IActionResult> PostTradeMark(string name)
        {
            var check = await _context.TradeMarks.Where(t => t.Name.ToLower().Contains(name.ToLower())).ToListAsync();
            if (check.Count > 0)
            {
                return Ok(new
                {
                    status = 500,
                    msg = "Tên nhãn hiệu đã tồn tại"
                });
            }
            var tradeMark = new TradeMark();
            tradeMark.Name = name;
            _context.TradeMarks.Add(tradeMark);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    status = 200,
                    msg = "Thêm thành công",
                    id = tradeMark.Id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest();
        }


        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(int id1)
        {

            try
            {
        
                var formCollection = await Request.ReadFormAsync();
                var a = 0;
                var file = formCollection.Files.First();


             
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fname = id1.ToString() + "." + fileName.Split('.')[1];
                    var path = Path.Combine(Url, fname);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }



                    var sup = await _context.TradeMarks.FindAsync(id1);
                    if (sup != null)
                    {
                        sup.Image = fname;
                        _context.Update(sup);
                    }
                    await _context.SaveChangesAsync();

                }
                else
                {
                    return BadRequest();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
            return BadRequest();
        }
        [HttpPut]

        public async Task<IActionResult> EditTradeMark(int id,TradeMark tradeMark)
        {
            var check= await _context.TradeMarks.FindAsync(id);
            if(check==null)
            {
                return BadRequest();
            }
            if(check.Name.ToLower().Contains(tradeMark.Name.ToLower()))
            {
                return Ok(new
                {
                    status = 500,
                    msg = "Nhãn hiệu đã tồn tại"
                });
            }

            check.Name=tradeMark.Name;
            _context.TradeMarks.Update(check);
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
        [HttpGet]

        public async Task<IActionResult> Search(string txtSearch)
        {
            var result= await _context.TradeMarks.Where(t => t.Name.ToLower().Contains(txtSearch.ToLower())).ToListAsync();
            return Ok(result);
        }
        

        [HttpPost] 
         public async Task<IActionResult> RemoveTradeMark(int id)
        {
            var check = await _context.TradeMarks.FindAsync(id);
            if(check==null)
            {
                return BadRequest();
            }
            var lst=await _context.Product.Where(t=>t.TradeMarkId==id).ToListAsync();
            if(lst.Count>0)
            {
                return Ok(new
                {
                    status = 500
                });
            }
            if (check.Image != null)
            {
                var fileDelete = Path.Combine(Url, check.Image);
                FileInfo file = new FileInfo(fileDelete);
                file.Delete();
            }
            _context.TradeMarks.Remove(check);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = 200,
                msg = "Xoá thành công"
            });
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload1(int id)
        {


            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');


                    var fname = id.ToString() + "." + fileName.Split('.')[1];
                    var pro = await _context.TradeMarks.FindAsync(id);


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
    }
}
