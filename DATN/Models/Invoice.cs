using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DATN.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        [DisplayName("Người dùng")]
        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }
        [DisplayName("Ngày lập hoá đơn")]
        [DataType(DataType.Date)]
        public DateTime? IssuedDate { get; set; }
        [DisplayName("Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }
        [DisplayName("Số điện thoại")]

        public string ShippingPhone { get; set; }
        public string? Note { get; set; }

        [DisplayName("Tổng hoá đơn (VNĐ)")]
        [DisplayFormat(DataFormatString = "{0:#,##0} VNĐ")]
        public float Total { get; set; }
        [DisplayName("Trạng thái")]

        public bool? Status { get; set; }

        [DisplayName("Hoàn thành")]
        public bool? Complete { get; set; }

        public List<InvoiceDetail>? InvoiceDetails { get; set; }
    }
}
