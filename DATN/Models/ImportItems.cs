namespace DATN.Models
{
    public class ImportItems
    {
        public int Id { get; set; }

        public int SupplierId { get; set; }

        
        public int ProductId { get; set; }
        public Product? Product { get; set; }
  
        public int Quantity { get; set; }

        public int Price { get; set; }
    }
}
