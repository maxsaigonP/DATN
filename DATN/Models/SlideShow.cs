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
    public class SlideShow
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]

        public string? Title { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống"), MaxLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string? Link { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string? Image { get; set; }


       
    }
}
