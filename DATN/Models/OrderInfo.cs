namespace DATN.Models
{
    public class OrderInfo
    {


        public long OrderId { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
        public string OrderDesc { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
