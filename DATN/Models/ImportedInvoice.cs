
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class ImportedInvoice
    {
        public int Id { get; set; }

        [DisplayName("Ngày nhập")]
        public DateTime DateImport { get; set; }
        [DisplayName("Tổng tiền")]
    
        public double Total { get; set; }
        [DisplayName("Trạng thái")]
        public int Status { get; set; }
    }
}
