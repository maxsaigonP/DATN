using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using VNPAY_CS_ASPX;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class PayController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PayController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Pay(int total,string id, string Address, string? Phone, string? Note)
        {
          

            //
            var cart = _context.Cart.Where(c => c.AppUserId == id && c.Status == false).ToList();
            var total1 = 0;

            if (cart != null)
            {
                foreach (var c in cart)
                {
                    total1 = total1 + (UnitPrice(c.ProductId) * c.Quantity);
                }
                var iv = new Invoice();
                iv.AppUserId = id;
                iv.IssuedDate = DateTime.Now.Date;
                iv.ShippingAddress = Address;
                iv.ShippingPhone = Phone;
                iv.Total = total1;
                if (Note != null && Note != "")
                {
                    iv.Note = Note;
                }
                iv.Status = true;
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
                   
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }else
            {
                return BadRequest();
            }
       
            //
            //Get Config Info
            string vnp_Returnurl = "http://localhost:4200/cart"; //URL nhan ket qua tra ve 
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = "M5R7JGFX"; //Ma website
            string vnp_HashSecret = "THSOQMPPOBWGCLPLDANVSTTMDXZVBOQG"; //Chuoi bi mat

            //Get payment input
            OrderInfo order = new OrderInfo();
            //Save order to db
            order.OrderId = DateTime.Now.Ticks; ; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            order.Amount = 20000000; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending"
            order.OrderDesc = "";
            order.CreatedDate = DateTime.Now;

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

            vnpay.AddRequestData("vnp_BankCode", "NCB");

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "billpayment"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
                                                                               //Add Params of 2.1.0 Version

            //Billing
            vnpay.AddRequestData("vnp_Bill_Mobile", "0388645726");
            vnpay.AddRequestData("vnp_Bill_Email", "nhbaophuc3008@gmail.com");
            var fullName = "Test";
            if (!String.IsNullOrEmpty(fullName))
            {
                var indexof = fullName.IndexOf(' ');
                vnpay.AddRequestData("vnp_Bill_FirstName", fullName);
                vnpay.AddRequestData("vnp_Bill_LastName", fullName);
            }
            vnpay.AddRequestData("vnp_Bill_Address", "Test");
            vnpay.AddRequestData("vnp_Bill_City", "Ho Chi Minh");
            vnpay.AddRequestData("vnp_Bill_Country", "Viet Nam");
            vnpay.AddRequestData("vnp_Bill_State", "");
            // Invoice
            vnpay.AddRequestData("vnp_Inv_Phone", "0388745726");
            vnpay.AddRequestData("vnp_Inv_Email", "baophuc3008@gmail.com");
            vnpay.AddRequestData("vnp_Inv_Customer", "Bao Phuc");
            vnpay.AddRequestData("vnp_Inv_Address", "HCM");
            vnpay.AddRequestData("vnp_Inv_Company", "2P SHop");
            vnpay.AddRequestData("vnp_Inv_Taxcode", "123");
            vnpay.AddRequestData("vnp_Inv_Type", "test");

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Ok(new
            {
                url = paymentUrl
            });

        }

        [HttpGet]

        public IActionResult GetRes(int i)
        {
            string a = Request.QueryString.ToString();
            
            return Ok(a);
        }
    }
}
