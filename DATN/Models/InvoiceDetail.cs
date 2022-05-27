using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DATN.Models
{
    public class InvoiceDetail
    {
        public int Id { get; set; }

        [DisplayName("Mã hoá đơn")]
        public int InvoiceId { get; set; }

        public Invoice Invoice { get; set; }

        [DisplayName("Sản phẩm")]
        public int ProductId { get; set; }

        public Product Product { get; set; }
        [DisplayName("Số lượng")]
        public int Quantity { get; set; }
        [DisplayName("Giá")]
        [DisplayFormat(DataFormatString = "{0:#,##0} VNĐ")]
        public int UnitPrice { get; set; }
        public bool? Status { get; set; }
    }
}
