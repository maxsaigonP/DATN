using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class Log
    {
        public int Id { get; set; }
        [DisplayName("Tên người dùng")]
        public string UserName { get; set; }
        [DisplayName("Mô tả")]
        public string Description { get; set; }
        [DisplayName("Thời gian")]
     
        public DateTime CreatedDate { get; set; }
    }
}
