using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost]
         
        public async Task<IActionResult> Create(string id,string Address,string Phone,float Total)
        {
            var iv = new Invoice();
            iv.AppUserId = id;
            iv.IssuedDate = DateTime.Now;
            iv.ShippingAddress = Address;
            iv.ShippingPhone = Phone;
            iv.Total = Total;
            _context.Add(iv);

            var cart =  _context.Cart.Where(c => c.AppUserId == id).ToList();
            if (cart != null)
            {


                foreach (var c in cart)
                {
                    var ivd = new InvoiceDetail();
                    ivd.InvoiceId = iv.Id;
                    ivd.ProductId = c.ProductId;
                    ivd.Quantity = c.Quantity;
                    ivd.UnitPrice = UnitPrice(c.ProductId) * c.Quantity;
                    ivd.Status = true;
                    _context.Add(ivd);
                    await _context.SaveChangesAsync();
                }
        
                return Ok("Dat hang thanh cong");
            }
            return NoContent();
        }

        public int UnitPrice(int id)
        {
            var pr = _context.Product.Find(id);
            return pr.Price;
        }

        [HttpPost]

        public async Task<IActionResult> DuyetDon([FromForm] Invoice invoice, int id)
        {
            var iv= await _context.Invoice.FindAsync(id);
            if (ModelState.IsValid&&iv!=null)
            {

                iv.Status = true;
                _context.Update(iv);
                await _context.SaveChangesAsync();
                return Ok("Duyet don thanh cong");
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
            var result = _context.Invoice.Where(i => i.IssuedDate >= start && i.IssuedDate < end).Sum(i => i.Total);

            var imp= _context.importedInvoice.Where(i => i.DateImport >= start && i.DateImport < end).Sum(i => i.Total);

            return Ok(result-imp);
        }
    }
}
