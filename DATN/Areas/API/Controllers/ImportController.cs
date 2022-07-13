using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImportController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
    

        public ImportController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]

        public async Task<IActionResult> GetImport()
        {
            var imp=(from a in _context.importedInvoice
                     select new
                     {
                         Id=a.Id,
                         ThoiGian=a.DateImport,
                         Total=a.Total
                     }).ToList();

            return Ok(imp);
        }
        [HttpGet]

        public async Task<IActionResult> Filter(DateTime start, DateTime end)
        {
            var imp = (from a in _context.importedInvoice
                       where a.DateImport.Day >= start.Day&&a.DateImport.Day <= end.Day&&a.DateImport.Month>= start.Month&&a.DateImport.Month <= end.Month
                       select new
                       {
                           Id = a.Id,
                           ThoiGian = a.DateImport,
                           Total = a.Total
                       }).ToList();

            return Ok(imp);
        }

        [HttpGet]

        public async Task<IActionResult> GetImportDetail(int id)
        {
            var imp = (from a in _context.ImportecInvoiceDetail
                       join b in _context.Product on a.ProductId equals b.Id
                       join c in _context.Supplier on a.SupplierId equals c.Id
                       join d in _context.importedInvoice on a.ImportedInvoiceId equals d.Id
                       where a.ImportedInvoiceId==id
                       select new
                       {
                           Id = a.Id,
                           NgayNhap=d.DateImport,
                           SanPhamId=a.ProductId,
                           TenSanPham=b.Name,
                           GiaNhap=a.Price,
                           SoLuong=a.Quantity,
                           Gia=a.Price*a.Quantity,
                           NhaCungCap=c.SupplierName,
                           Total=d.Total
                         
                       }).ToList();

            return Ok(imp);
        }

        [HttpPost]

        public async Task<IActionResult> PostImport()
        {
            double total = 0;
            var item = await _context.ImportItems.ToListAsync();
            if(item.Count==0)
            {
                return BadRequest();
            }
            foreach(var i in item)
            {
                total += i.Quantity * i.Price;
            }
            var imp = new ImportedInvoice();
            imp.Total = total;
            imp.DateImport = DateTime.Now.Date;

            _context.Add(imp);
            _context.SaveChanges();
            foreach (var i in item)
            {

                var pro = await _context.Product.FindAsync(i.ProductId);
                if (pro == null)
                {
                    _context.Remove(imp);
                    await _context.SaveChangesAsync();
                    return BadRequest();
                }
                pro.Stock += i.Quantity;
                pro.ImportPrice = i.Price;
                _context.Update(pro);
                var impd=new ImportecInvoiceDetail();
                impd.ProductId=i.ProductId;
                impd.SupplierId=i.SupplierId;
                impd.Price=i.Price;
                impd.Quantity=i.Quantity;
                impd.ImportedInvoiceId = imp.Id;

                _context.Add(impd);
                _context.Remove(i);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                status = 200,
                msg = "Thêm thành công"
            });


        }

        [HttpPost]

        public async Task<IActionResult> AddImportItem(int price,int ncc,int soLuong,int sanPham)
        {
            var imp = new ImportItems();
            imp.Price = price;
            imp.SupplierId = ncc;
            imp.Quantity = soLuong;
            imp.ProductId = sanPham;

            _context.Add(imp);
          await  _context.SaveChangesAsync();

            return Ok(new
            {
                status = 200,
                msg = "Thêm thành công"
            });


        }

        [HttpPost]

        public async Task<IActionResult> RemoveImportItem(int id)
        {
            var imp = await _context.ImportItems.FindAsync(id);
            if(imp!=null)
            {
                _context.Remove(imp);
            }
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = 200,
                msg = "Thêm thành công"
            });


        }

        [HttpGet]
        public async Task<IActionResult> GetImportItem()
        {
            var imp = (from a in _context.ImportItems
                       join b in _context.Product on a.ProductId equals b.Id
                       join c in _context.Supplier on a.SupplierId equals c.Id
                       select new
                       {
                           Id = a.Id,
                           TenSanPham = b.Name,
                           NhaCungCap=c.SupplierName,
                           Gia=a.Price,
                           SoLuong=a.Quantity
                       }).ToList();
            double total = 0;
            foreach (var item in imp)
            {
                total += item.Gia * item.SoLuong;
            }
            return Ok(new
            {
                imp1 = imp,
                total=total
            });
        }





        [HttpPost]

        public async Task<IActionResult> Clear()
        {
            var imp = await _context.ImportItems.ToListAsync();
            if (imp.Count>0)
            {
                try
                {
                    foreach (var i in imp)
                    {
                        _context.Remove(i);
                        await _context.SaveChangesAsync();
                    }
                    return Ok(200);
                }catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
             
            }
            return BadRequest();




        }
    }
}
