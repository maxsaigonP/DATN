
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class ImportecInvoiceDetail
    {
        public int Id { get; set; }
        [DisplayName("Hoá đơn nhập")]
        public int ImportedInvoiceId { get; set; }
        public ImportedInvoice ImportedInvoice { get; set; }
        [DisplayName("Sản phẩm")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [DisplayName("Số lượng")]
        public int Quantity { get; set; }
        [DisplayName("Giá tiền")]
        public decimal Price { get; set; }

        public DateTime Date { get; set; }

    }
}
