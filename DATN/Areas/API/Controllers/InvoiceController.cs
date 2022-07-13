using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoMo;
using NBitcoin.Payment;
using Newtonsoft.Json.Linq;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InvoiceController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> GetInvoice()
        {
            var result = (from a in _context.Invoice
                          join b in _context.AppUsers on a.AppUserId equals b.Id
                          select new
                          {
                              Id = a.Id,
                              Username = b.UserName,
                              ShippingAddress = a.ShippingAddress,
                              Phone = a.ShippingPhone,
                              Date = a.IssuedDate,
                              Total = a.Total,
                              Status = a.Status,
                              Complete = a.Complete,
                              Note=a.Note,
                              Cancel=a.Cancel
                          }).ToArray();

            return Ok(new
            {
                inv = result,
                count = result.Count()
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetInvoiceByStatus(int sts)
        {

            if (sts == 4)
            {
                var result1 = (from a in _context.Invoice
                               join b in _context.AppUsers on a.AppUserId equals b.Id
                               where a.Cancel !=true && a.Status==false
                               select new
                               {
                                   Id = a.Id,
                                   Username = b.UserName,
                                   ShippingAddress = a.ShippingAddress,
                                   Phone = a.ShippingPhone,
                                   Date = a.IssuedDate,
                                   Total = a.Total,
                                   Status = a.Status,
                                   Complete = a.Complete,
                                   Cancel = a.Cancel
                               }).ToArray();

                return Ok(new
                {
                    inv = result1,
                    count = result1.Count()
                });
            }
            if (sts==3)
            {
                var result1 = (from a in _context.Invoice
                              join b in _context.AppUsers on a.AppUserId equals b.Id
                              where a.Cancel == true
                              select new
                              {
                                  Id = a.Id,
                                  Username = b.UserName,
                                  ShippingAddress = a.ShippingAddress,
                                  Phone = a.ShippingPhone,
                                  Date = a.IssuedDate,
                                  Total = a.Total,
                                  Status = a.Status,
                                  Complete = a.Complete,
                                  Cancel=a.Cancel
                              }).ToArray();

                return Ok(new
                {
                    inv = result1,
                    count = result1.Count()
                });
            }
            var result = (from a in _context.Invoice
                          join b in _context.AppUsers on a.AppUserId equals b.Id
                          where a.Complete == (sts == 1 ? false : true) && a.Status==true
                          select new
                          {
                              Id = a.Id,
                              Username = b.UserName,
                              ShippingAddress = a.ShippingAddress,
                              Phone = a.ShippingPhone,
                              Date = a.IssuedDate,
                              Total = a.Total,
                              Status = a.Status,
                              Complete = a.Complete,
                              Cancel=a.Cancel
                          }).ToArray();

            return Ok(new
            {
                inv = result,
                count = result.Count()
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceUser(string id)
        {
            var result = (from a in _context.Invoice
                          join b in _context.AppUsers on a.AppUserId equals b.Id
                          where a.AppUserId == id && a.Cancel!=true
                          select new
                          {
                              Id = a.Id,
                              Username = b.UserName,
                              ShippingAddress = a.ShippingAddress,
                              Phone = a.ShippingPhone,
                              Date = a.IssuedDate,
                              Total = a.Total,
                              Status = a.Status,
                              Complete = a.Complete
                          }).ToArray();

            return Ok(result);
        }

    

        [HttpGet]
        public async Task<IActionResult> Filter(DateTime start, DateTime end)
        {
           
            
                var inv = (from a in _context.Invoice
                           join b in _context.AppUsers on a.AppUserId equals b.Id
                           where a.IssuedDate.Value.Day >= start.Day && a.IssuedDate.Value.Day <= end.Day&& a.IssuedDate.Value.Month >= start.Month && a.IssuedDate.Value.Month <= end.Month&& a.IssuedDate.Value.Year >= start.Year && a.IssuedDate.Value.Year <= end.Year
                           select new
                           {
                               Id = a.Id,
                               Username = b.UserName,
                               ShippingAddress = a.ShippingAddress,
                               Phone = a.ShippingPhone,
                               Date = a.IssuedDate,
                               Total = a.Total,
                               Status = a.Status,
                               Complete = a.Complete,
                               Cancel = a.Cancel
                           }).ToArray();



                return Ok(new
                {
                    inv = inv

                });
          
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceConfirm()
        {
            var result = (from a in _context.Invoice
                          join b in _context.AppUsers on a.AppUserId equals b.Id
                          where a.Status == false && a.Cancel != true
                          select new
                          {
                              Id = a.Id,
                              Username = b.UserName,
                              ShippingAddress = a.ShippingAddress,
                              Phone = a.ShippingPhone,
                              Date = a.IssuedDate,
                              Total = a.Total
                          }).ToArray();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Invoice1(int id)
        {
            var result = (from a in _context.Invoice
                          join b in _context.AppUsers on a.AppUserId equals b.Id
                          where a.Id == id
                          select new
                          {
                              Id = a.Id,
                              Username = b.UserName,
                              ShippingAddress = a.ShippingAddress,
                              Phone = a.ShippingPhone,
                              Date = a.IssuedDate,
                              Total = a.Total,
                              Note=a.Note,
                              Cancel=a.Cancel,
                              Status=a.Status,
                              Complete=a.Complete
                          }).FirstOrDefault();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceDetail(int id)
        {
            var result = (from a in _context.invoiceDetail
                          join b in _context.Product on a.ProductId equals b.Id
                          where a.InvoiceId == id
                          select new
                          {
                              SanPham = b.Name,
                              Soluong = a.Quantity,
                              DonGia = a.UnitPrice,

                          }).ToList();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id,int sts)
        {
            var inv = await _context.Invoice.FindAsync(id);
            if(inv!=null)
            {
                if(sts==1)
                {
                    inv.Status = true;
                    inv.Complete = false;
                    inv.Cancel = false;
                }
                if (sts == 2)
                {
                    inv.Cancel=true;
                }
                if(sts==3)
                {
                    inv.Status = true;
                    inv.Complete = true;
                    inv.Cancel = false;
                }
                _context.Update(inv);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    status = 200
                });
            }
            return BadRequest();
        }


        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var inv = await _context.Invoice.FindAsync(id);
           
            if(inv!=null)
            {
                var lstinv = await _context.invoiceDetail.Where(i => i.InvoiceId == id).ToListAsync();
                try
                {
                    foreach(var i in lstinv)
                    {
                        var pro = await _context.Product.FindAsync(i.ProductId);
                        pro.Stock += i.Quantity;
                        _context.Update(pro);
                        await _context.SaveChangesAsync();
                    }
                    inv.Cancel=true;
                    _context.Update(inv);
                    await _context.SaveChangesAsync();
                    return Ok(new
                    {
                        Status = 200,
                        msg = "Huỷ đơn thành công"
                    });
                }catch(Exception ex)
                {
                    return BadRequest(ex.Message);  
                }
            }

            return BadRequest();
        }
        [HttpPost]

        public async Task<IActionResult> Create(string id, string Address, string? Phone,string? Note)
        {


            var cart = _context.Cart.Where(c => c.AppUserId == id && c.Status == false).ToList();
            var total = 0;

            if (cart.Count>0)
            {
                foreach (var c in cart)
                {
                    total = total + (UnitPrice(c.ProductId) * c.Quantity);
                }
                var iv = new Invoice();
                iv.AppUserId = id;
                iv.IssuedDate = DateTime.Now.Date;
                iv.ShippingAddress = Address;
                iv.ShippingPhone = Phone;
                iv.Total = total;
                iv.Cancel = false;
                if(Note!=null&&Note!="")
                {
                    iv.Note = Note;
                }
                iv.Status = false;
                iv.Complete = false;
                _context.Add(iv);
                await _context.SaveChangesAsync();

                foreach (var c in cart)
                {
                    var pro = await _context.Product.FindAsync(c.ProductId);
                    if (pro.Stock < c.Quantity)
                    {
                        return Ok(new
                        {
                            status = 500,
                            msg = "Số lượn sản phẩm không đủ"
                        });
                    }
                    pro.Stock -= c.Quantity;
                    _context.Update(pro);
                    var ivd = new InvoiceDetail();
                    ivd.InvoiceId = iv.Id;
                    ivd.ProductId = c.ProductId;
                    ivd.Quantity = c.Quantity;

                    ivd.UnitPrice = UnitPrice(c.ProductId) * c.Quantity;
                    ivd.Status = true;
                    _context.Add(ivd);

                    _context.Cart.Remove(c);
                    
                }
                try
                {
                   
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                return Ok(new
                {
                    status = 200,
                    msg = "Đặt hàng thành công"
                });
            }
            return NoContent();
        }

        [HttpGet]
        public int UnitPrice(int id)
        {
            var pr = _context.Product.Find(id);
            if (pr.SalePrice != 0)
            {
                return pr.SalePrice;
            }
            return pr.Price;
        }

        [HttpPost]

        public async Task<IActionResult> DuyetDon(int id)
        {
            var iv = await _context.Invoice.FindAsync(id);
            if (ModelState.IsValid && iv != null)
            {

                iv.Status = true;
                _context.Update(iv);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    status = 200,
                    msg = "Duyệt đơn thành công"
                });
            }
            return NoContent();
        }

        [HttpPost]

        public async Task<IActionResult> HoanThanh(int id)
        {
            var iv = await _context.Invoice.FindAsync(id);
            if (ModelState.IsValid && iv != null)
            {

                iv.Complete = true;
                _context.Update(iv);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    status = 200
                });
            }
            return NoContent();
        }




        [HttpPost]

        public async Task<IActionResult> HuyDon([FromForm] Invoice invoice, int id)
        {
            var iv = await _context.Invoice.FindAsync(id);
            if (ModelState.IsValid && iv != null)
            {

                _context.Remove(iv);
                await _context.SaveChangesAsync();
                return Ok("Huy don thanh cong");
            }
            return NoContent();
        }


        [HttpGet]

        public async Task<ActionResult> ViewInvoice(int id)
        {
            var iv = await _context.Invoice.FindAsync(id);
            var result = (from a in _context.invoiceDetail
                          join b in _context.Product on a.ProductId equals b.Id
                          where a.InvoiceId == id
                          select new
                          {
                              SanPham = b.Name,
                              SoLuong = a.Quantity,
                              Gia = a.UnitPrice

                          }).ToList();
            return Ok(result);
        }


        [HttpGet]

        public async Task<ActionResult> ThongKe(DateTime start, DateTime end)
        {
            var inv = (from a in _context.Invoice
                       join b in _context.AppUsers on a.AppUserId equals b.Id
                       where a.IssuedDate.Value.Day >= start.Day && a.IssuedDate.Value.Day <= end.Day && a.IssuedDate.Value.Month >= start.Month && a.IssuedDate.Value.Month <= end.Month && a.Status == true && a.Complete == true
                       select new
                       {
                           Id = a.Id,
                           Username = b.UserName,
                           ShippingAddress = a.ShippingAddress,
                           Phone = a.ShippingPhone,
                           Date = a.IssuedDate,
                           Total = a.Total,
                           Status = a.Status,
                           Complete = a.Complete,
                           Cancel=a.Cancel
                       }).ToArray();

            var slNhap = (from a in _context.ImportecInvoiceDetail
                          join b in _context.importedInvoice on a.ImportedInvoiceId equals b.Id
                          where b.DateImport.Day >= start.Day && b.DateImport.Day <= end.Day && b.DateImport.Month >= start.Month && b.DateImport.Month <= end.Month
                          select a.Quantity).Sum();
            var slBan = (from a in _context.invoiceDetail
                         join b in _context.Invoice on a.InvoiceId equals b.Id
                         where b.IssuedDate.Value.Day >= start.Day && b.IssuedDate.Value.Day <= end.Day &&b.Complete==true
                         select a.Quantity).Sum();
            var result = _context.Invoice.Where(i => i.IssuedDate.Value.Day >= start.Day && i.IssuedDate.Value.Day <= end.Day&& i.IssuedDate.Value.Month >= start.Month && i.IssuedDate.Value.Month <= end.Month && i.Complete == true).Sum(i => i.Total);

            var imp = _context.importedInvoice.Where(i => i.DateImport.Day >= start.Day && i.DateImport.Day <= end.Day&& i.DateImport.Month >= start.Month && i.DateImport.Month <= end.Month).Sum(i => i.Total);



            return Ok(new
            {
               
                inv = inv,
                importTotal = imp,
                saleTotal = result,
                importQuantily = slNhap,
                saleQuantily = slBan

            });
        }
        [HttpGet]

        public async Task<IActionResult> ThongKeKhoang(DateTime start, DateTime end)
        {
            var result = (from a in _context.Product
                          select new
                          {
                              SanPham = a.Name,
                              GiaNhap = a.ImportPrice,
                              GiaBan = a.Price,
                              SoLuongBan = (from b in _context.invoiceDetail
                                            join d in _context.Invoice on b.InvoiceId equals d.Id
                                            where b.ProductId == a.Id && d.IssuedDate.Value.Day>=start.Day&& d.IssuedDate.Value.Day <= end.Day && d.IssuedDate.Value.Month >= start.Month && d.IssuedDate.Value.Month <= end.Month&&d.IssuedDate.Value.Year>=start.Year && d.IssuedDate.Value.Year <= end.Year && d.Complete==true
                                            select b.Quantity).Sum(),

                              SoLuongNhap = (from c in _context.ImportecInvoiceDetail
                                             join e in _context.importedInvoice on c.ImportedInvoiceId equals e.Id
                                             where c.ProductId == a.Id && e.DateImport.Day>=start.Day&&e.DateImport.Day<=end.Day&&e.DateImport.Month >= start.Month && e.DateImport.Month <= end.Month&&e.DateImport.Year>=start.Year && e.DateImport.Year <= end.Year
                                             select c.Quantity).Sum()
                          }
                                   ).ToArray();
            var result1 = result.Where(a => a.SoLuongNhap != 0 || a.SoLuongBan != 0).ToList();
            var sln = result1.Sum(a => a.SoLuongNhap);
            var slb = result1.Sum(a => a.SoLuongBan);
            var sale = _context.Invoice.Where(i => i.IssuedDate.Value.Day >= start.Day && i.IssuedDate.Value.Day <= end.Day && i.IssuedDate.Value.Month >= start.Month && i.IssuedDate.Value.Month <= end.Month && i.Complete == true).Sum(i => i.Total);

            var imp = _context.importedInvoice.Where(i => i.DateImport.Day >= start.Day && i.DateImport.Day <= end.Day && i.DateImport.Month >= start.Month && i.DateImport.Month <= end.Month).Sum(i => i.Total);
            var ban = result1.Sum(a => a.GiaBan * a.SoLuongBan);
            return Ok(new
            {
                lst=result1,
                nhap=sln,
                ban=slb,
                totaln = imp,
                totalb = sale,
                totalban=ban
            });
        }

        [HttpGet]

        public async Task<ActionResult> ThongKeTheoTuan()
        {
            var result = (from a in _context.Product
                          select new
                          {
                              SanPham = a.Name,
                              GiaNhap = a.ImportPrice,
                              GiaBan =  a.Price,
                              SoLuongBan = (from b in _context.invoiceDetail
                                            join d in _context.Invoice on b.InvoiceId equals d.Id
                                            where b.ProductId == a.Id && d.IssuedDate.Value.Day >= DateTime.Now.Day-7 && d.IssuedDate.Value.Day <= DateTime.Now.Day && d.IssuedDate.Value.Month >= DateTime.Now.Month && d.IssuedDate.Value.Month <= DateTime.Now.Month && d.IssuedDate.Value.Year >= DateTime.Now.Year && d.IssuedDate.Value.Year <= DateTime.Now.Year && d.Complete == true
                                            select b.Quantity).Sum(),

                              SoLuongNhap = (from c in _context.ImportecInvoiceDetail
                                             join e in _context.importedInvoice on c.ImportedInvoiceId equals e.Id
                                             where c.ProductId == a.Id && e.DateImport.Day >= DateTime.Now.Day - 7 && e.DateImport.Day <= DateTime.Now.Day && e.DateImport.Month >= DateTime.Now.Month && e.DateImport.Month <= DateTime.Now.Month && e.DateImport.Year >= DateTime.Now.Year && e.DateImport.Year <= DateTime.Now.Year
                                             select c.Quantity).Sum()
                          }
                                   ).ToArray();
            var result1 = result.Where(a => a.SoLuongNhap != 0 || a.SoLuongBan != 0).ToList();
            var sln = result1.Sum(a => a.SoLuongNhap);
            var slb = result1.Sum(a => a.SoLuongBan);
            var sale = _context.Invoice.Where(i => i.IssuedDate.Value.Day >= DateTime.Now.Day - 7 && i.IssuedDate.Value.Day <= DateTime.Now.Day && i.IssuedDate.Value.Month  >= DateTime.Now.Month  && i.IssuedDate.Value.Month <= DateTime.Now.Month && i.Complete == true).Sum(i => i.Total);

            var imp = _context.importedInvoice.Where(i => i.DateImport.Day >= DateTime.Now.Day - 7 && i.DateImport.Day <= DateTime.Now.Day && i.DateImport.Month  >= DateTime.Now.Month && i.DateImport.Month <= DateTime.Now.Month).Sum(i => i.Total);
            var ban = result1.Sum(a => a.GiaBan * a.SoLuongBan);
            return Ok(new
            {
                lst = result1,
                nhap = sln,
                ban = slb,
                totaln = imp,
                totalb = sale,
                totalban = ban
            });



        }
        [HttpGet]

        public async Task<ActionResult> ThongKeTheoThang(int month)
        {
            var result = (from a in _context.Product
                          select new
                          {
                              SanPham = a.Name,
                              GiaNhap = a.ImportPrice,
                              GiaBan = a.Price,
                              SoLuongBan = (from b in _context.invoiceDetail
                                            join d in _context.Invoice on b.InvoiceId equals d.Id
                                            where b.ProductId == a.Id &&  d.IssuedDate.Value.Month >= month && d.IssuedDate.Value.Month <= month && d.IssuedDate.Value.Year >= DateTime.Now.Year && d.IssuedDate.Value.Year <= DateTime.Now.Year && d.Complete == true
                                            select b.Quantity).Sum(),

                              SoLuongNhap = (from c in _context.ImportecInvoiceDetail
                                             join e in _context.importedInvoice on c.ImportedInvoiceId equals e.Id
                                             where c.ProductId == a.Id &&  e.DateImport.Month >= month && e.DateImport.Month <= month && e.DateImport.Year >= DateTime.Now.Year && e.DateImport.Year <= DateTime.Now.Year
                                             select c.Quantity).Sum()
                          }
                                  ).ToArray();
            var result1 = result.Where(a => a.SoLuongNhap != 0 || a.SoLuongBan != 0).ToList();
            var sln = result1.Sum(a => a.SoLuongNhap);
            var slb = result1.Sum(a => a.SoLuongBan);
            var sale = _context.Invoice.Where(i => i.IssuedDate.Value.Month >= month && i.IssuedDate.Value.Month <= month && i.Complete == true).Sum(i => i.Total);

            var imp = _context.importedInvoice.Where(i =>  i.DateImport.Month >= month && i.DateImport.Month <= month).Sum(i => i.Total);
            var ban = result1.Sum(a => a.GiaBan * a.SoLuongBan);
            return Ok(new
            {
                lst = result1,
                nhap = sln,
                ban = slb,
                totaln = imp,
                totalb = sale,
                totalban = ban
            });




        }
    }
}
