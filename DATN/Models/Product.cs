
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DATN.Models
{
    public class Product
    {
        public int Id { get; set; }

        [DisplayName("Tên sản phẩm")]
        [Required(ErrorMessage ="Không được bỏ trống")]
        public string? Name { get; set; }
        [DisplayName("Mô tả")]
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string? Description { get; set; }

        public int Price { get; set; }

        public int? SalePrice { get; set; }
        [DisplayName("Số lượng")]
        public int? Quantily { get; set; }

        [DisplayName("Loại sản phẩm")]
        public int CategoryId { get; set; }

        [DisplayName("Loại sản phẩm")]
        public Category? Category { get; set; }

        [DisplayName("Thương hiệu")]
        public string? TradeMark { get; set; }


        [DisplayName("CPU")]
        public string? CPU { get; set; }


        [DisplayName("RAM")]
        public string? RAM { get; set; }


        [DisplayName("Màn hình")]
        public string? Monitor { get; set; }


        [DisplayName("Card màn hình")]
        public string? VGA { get; set; }


        [DisplayName("Thiết kế")]
        public string? DesignStyle { get; set; }


        [DisplayName("Kích thước-trọng lượng")]
        public string? SizeWeight { get; set; }

        [DisplayName("Đánh giá")]
        public double? Star { get; set; }

        
        [DisplayName("Hình ảnh")]
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public bool? Status { get; set; }

        public List<Cart>? Carts { get; set; }

       

        public List<InvoiceDetail>? InvoiceDetails { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<SlideShow>? SlideShows { get; set; }
        public List<Images>? Images { get; set; }
        public List<WishList>? WishLists { get; set; }
    }
}
