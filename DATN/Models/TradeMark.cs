using System.ComponentModel;

namespace DATN.Models
{
    public class TradeMark
    {
        public int Id { get; set; }

        [DisplayName("Thương hiệu")]
        public string Name { get; set; }

        public List<Product> Products { get; set; }
    }
}
