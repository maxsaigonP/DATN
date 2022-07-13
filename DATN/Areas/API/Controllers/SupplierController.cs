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
    public class SupplierController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
       
        public SupplierController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]

        public async Task<IActionResult> GetAllSupplier()
        {
            var sup= (from a in _context.Supplier
                      select new
                      {
                          Id=a.Id,
                          TenNhaCungCap=a.SupplierName,
                          Email=a.Email,
                          Phone=a.Phone,
                          Address=a.Address
                      }).ToList();

            return Ok(new
            {
                sup=sup,
                count=sup.Count,
            });
        }


        [HttpGet]

        public async Task<IActionResult> GetSupplier(int id)
        {
            var sup = (from a in _context.Supplier
                       where a.Id== id
                       select new
                       {
                           Id = a.Id,
                           Name = a.SupplierName,
                           Email = a.Email,
                           Phone = a.Phone,
                           Address = a.Address
                       }).FirstOrDefault();

            return Ok(sup);
        }

        [HttpPost]

        public async Task<IActionResult> PostSupplier(Supplier sup)
        {
            var ncc = await _context.Supplier.Where(s => s.SupplierName.ToLower().Contains(sup.SupplierName.ToLower())||s.Email.ToLower().Contains(sup.Email.ToLower())).ToListAsync();
            if (ncc.Count > 0)
            {
                return Ok(new
                {
                    status = 500,
                    msg = "Nhà sản xuất hoặc email đã tồn tại."
                });
            }
            if(ModelState.IsValid)
            {
                sup.Image = "";
                _context.Supplier.Add(sup);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    status=200,
                    msg="Thêm thành công"
                });
            }

            return BadRequest();

           
        }

        [HttpPost]

        public async Task<IActionResult> EditSupplier(int id,Supplier sup)
        {
            if (ModelState.IsValid)
            {
                var check = await _context.Supplier.FindAsync(id);
                var ncc = await _context.Supplier.Where(s => s.SupplierName.ToLower().Contains(sup.SupplierName.ToLower()) || s.Email.ToLower().Contains(sup.Email.ToLower())&&!check.SupplierName.Contains(sup.SupplierName)).ToListAsync();
                if (ncc.Count > 0)
                {
                    return Ok(new
                    {
                        status = 500,
                        msg = "Nhà sản xuất hoặc email đã tồn tại."
                    });
                }
                if (check!=null)
                {
                    check.SupplierName = sup.SupplierName;
                    check.Email = sup.Email;
                    check.Phone = sup.Phone;
                    check.Address = sup.Address;
                    _context.Supplier.Update(check);
                    await _context.SaveChangesAsync();
                    return Ok(new
                    {
                        status = 200,
                        msg = "Thêm thành công"
                    });
                }
               
            }

            return BadRequest();


        }

        [HttpPost]

        public async Task<IActionResult> RemoveSupplier(int id )
        {
            var sup = await _context.Supplier.FindAsync(id);
            if(sup!=null)
            {
                _context.Supplier.Remove(sup);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    status = 200,
                    msg = "Xoá thành công"
                });
            }
            return BadRequest();
        }

        //

       

    }
}
