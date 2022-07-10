using DATN.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DATN.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<DATN.Models.Cart>? Cart { get; set; }
        public DbSet<DATN.Models.ImportecInvoiceDetail>? ImportecInvoiceDetail { get; set; }
        public DbSet<DATN.Models.ImportedInvoice>? importedInvoice { get; set; }
        public DbSet<DATN.Models.Product>? Product { get; set; }
        public DbSet<DATN.Models.Category>? Category { get; set; }
        public DbSet<DATN.Models.Comment>? Comment { get; set; }
        public DbSet<DATN.Models.Images>? Images { get; set; }
        public DbSet<DATN.Models.Invoice>? Invoice { get; set; }
        public DbSet<DATN.Models.InvoiceDetail>? invoiceDetail { get; set; }
    
        public DbSet<DATN.Models.TradeMark>? TradeMarks { get; set; }
        public DbSet<DATN.Models.WishList>? WishList { get; set; }
        public DbSet<DATN.Models.Supplier>? Supplier { get; set; }
        public DbSet<DATN.Models.SlideShow>? SlideShow { get; set; }

        public DbSet<DATN.Models.ImportItems>? ImportItems { get; set; }
        public DbSet<DATN.Models.AppUser>? AppUsers { get; set; }
        public DbSet<DATN.Models.About>? About { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
