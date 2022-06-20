﻿using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                              Total = a.Total
                          }).ToArray();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceUser(string id)
        {
            var result = (from a in _context.Invoice
                          join b in _context.AppUsers on a.AppUserId equals b.Id
                          where a.AppUserId== id
                          select new
                          {
                              Id = a.Id,
                              Username = b.UserName,
                              ShippingAddress = a.ShippingAddress,
                              Phone = a.ShippingPhone,
                              Date = a.IssuedDate,
                              Total = a.Total,
                              Status=a.Status
                          }).ToArray();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ThongKeTheoThoiGian(DateTime start,DateTime end)
        {
            var result = (from a in _context.Invoice
                          join b in _context.AppUsers on a.AppUserId equals b.Id
                          where a.IssuedDate>=start && a.IssuedDate<=end
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
        public async Task<IActionResult> ThongKeTheoThang(int month)
        {
            var result = (from a in _context.Invoice
                          join b in _context.AppUsers on a.AppUserId equals b.Id
                          where a.IssuedDate.Value.Month == month
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
        public async Task<IActionResult> GetInvoiceConfirm()
        {
            var result = (from a in _context.Invoice
                          join b in _context.AppUsers on a.AppUserId equals b.Id
                          where a.Status == false
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
                              Total = a.Total
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
                              DonGia = a.UnitPrice
                          }).ToList();

            return Ok(result);
        }

        [HttpPost]

        public async Task<IActionResult> Create(string id, string Address, string Phone)
        {


            var cart = _context.Cart.Where(c => c.AppUserId == id && c.Status == false).ToList();
            var total = 0;

            if (cart != null)
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
                iv.Status = false;
                iv.Complete = false;
                _context.Add(iv);
                await _context.SaveChangesAsync();

                foreach (var c in cart)
                {
                    var pro = await _context.Product.FindAsync(c.ProductId);
                    if (pro.Quantily < c.Quantity)
                    {
                        return Ok(new
                        {
                            status = 500,
                            msg = "Số lượn sản phẩm không đủ"
                        });
                    }
                    pro.Quantily -= c.Quantity;
                    _context.Update(pro);
                    var ivd = new InvoiceDetail();
                    ivd.InvoiceId = iv.Id;
                    ivd.ProductId = c.ProductId;
                    ivd.Quantity = c.Quantity;
                    ivd.UnitPrice = UnitPrice(c.ProductId) * c.Quantity;
                    ivd.Status = true;
                    _context.Add(ivd);

                    _context.Cart.Remove(c);
                    await _context.SaveChangesAsync();
                }
                try
                {
                    _context.Remove(cart);
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

            var imp = _context.importedInvoice.Where(i => i.DateImport >= start && i.DateImport < end).Sum(i => i.Total);

            return Ok(result - imp);
        }


        [HttpPost]

        public async Task<ActionResult> TestPayment()
        {
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOH6FA20220615";
            string accessKey = "HZkfnYZPjF9vffEE";
            string serectkey = "z4lYOkaeVZDibVmTTMX9cZlC6ldm7WmE";
            string orderInfo = "áaaaaa";
            string redirectUrl = "http://localhost:4200/admin";
            string ipnUrl = "http://localhost:4200/admin";
            string requestType = "captureWallet";

            string amount = "100000";
            string orderId = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";


            string rawHash = "accessKey=" + accessKey +
                "&amount=" + amount +
                "&extraData=" + extraData +
                "&notifyUrl=" + ipnUrl +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&returnUrl=" + redirectUrl +
                "&requestId=" + requestId +
                "&requestType=" + requestType
                ;



            MoMoSecurity crypto = new MoMoSecurity();

            string signature = crypto.signSHA256(rawHash, serectkey);


       
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
    
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "returnUrl", redirectUrl },
                { "notifyUrl", ipnUrl },

                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", "c8264690a8de274ce86a3d4189887b703548b9bc0568d9e1a3e5e68659bdd409 " }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Ok(new
            {
                msg = jmessage.GetValue("payUrl")
            });
        }
    }
}
