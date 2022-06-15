namespace DATN.Models
{
    public class ProductCreateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Quantily { get; set; }
        public int CategoryId { get; set; }
        public string TradeMark { get; set; }
        public double Star { get; set; }
        //public bool Status { get; set; }
        public string CPU { get; set; }
        public string DesignStyle { get; set; }
        public string Monitor { get; set; }
        public string RAM { get; set; }
        public string SizeWeight { get; set; }
        public string VGA { get; set; }
        public IFormFile? ImageFile { get; set; }
     
        


    }
}
