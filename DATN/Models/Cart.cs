using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace DATN.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }   


        public int ProductId { get; set; }
        public Product Product { get; set; }    

        public int Quantity { get; set; }
        public bool Status { get; set; }
    }
}
