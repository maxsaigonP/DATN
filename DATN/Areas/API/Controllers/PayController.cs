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

      

        [HttpPost]
        public IActionResult Pay(int total)
        {
            //Get Config Info
            string vnp_Returnurl = "https://localhost:7043/api/pay/getres"; //URL nhan ket qua tra ve 
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
