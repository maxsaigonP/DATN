using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DATN.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [DisplayName("Nội dung")]
        public string Content { get; set; }
        [DisplayName("Người dùng")]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        [DisplayName("Thời gian")]
        [DataType(DataType.Date)]
        public DateTime Time { get; set; }
        [DisplayName("Sản phẩm")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [DisplayName("Đánh giá")]
        public int Star { get; set; }
        
        public bool Status { get; set; }
    }
}
