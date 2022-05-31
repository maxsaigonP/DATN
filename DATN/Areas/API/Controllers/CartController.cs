﻿using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CartController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost]

        public async Task<IActionResult> AddCart(int sanPham,int soLuong,string userID)
        {
            var check = await _context.Cart.Where(c => c.ProductId == sanPham).FirstOrDefaultAsync();
            if (check != null)
            {
                check.Quantity += soLuong;
                _context.Update(check);
                await _context.SaveChangesAsync();
            }
            else
            {
                var cart = new Cart();
                cart.AppUserId = userID;
                cart.ProductId = sanPham;
                cart.Quantity = soLuong;
                cart.Status = false;
                _context.Add(cart);
                await _context.SaveChangesAsync();
            }

            return Ok("Da them vao gio hang");
        }


        [HttpPost]

        public async Task<IActionResult> UpdateCart(int cartID,int sl)
        {
            var cart = await _context.Cart.FindAsync(cartID);
            cart.Quantity+=sl;
            if(cart.Quantity<=0)
            {
                cart.Quantity = 0;
            }
            _context.Update(cart);
            await _context.SaveChangesAsync();

            return Ok("Da them vao gio hang");
        }

        [HttpPost]

        public async Task<IActionResult> RemoveCart(int cartID)
        {
            var cart = await _context.Cart.FindAsync(cartID);

            if (cart!=null)
            {
                _context.Cart.Remove(cart);
                await _context.SaveChangesAsync();
                return Ok("Da xoa khoi gio hang");
            }
           
            

            return BadRequest();
        }

        [HttpPost]

        public async Task<IActionResult> RemoveAllCart(string userID)
        {
            var cart = await _context.Cart.Where(c=>c.AppUserId==userID).ToListAsync();

            if (cart != null)
            {
              _context.Remove(cart);
                await  _context.SaveChangesAsync();
                return Ok("Da xoa toan bo gio hang");
            }



            return BadRequest();
        }
    }
}