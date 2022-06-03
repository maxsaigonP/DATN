﻿using DATN.Data;
using DATN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN.Areas.API.Controllers
{
    [Route("api/[controller]/[action]")]
   // [Authorize("Admin")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {

            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost]  
        public async Task<IActionResult> Create([FromForm] Category category)
        {
            if (ModelState.IsValid)
            {
                var cate = await _context.Category.Where(p => p.Name == category.Name).ToListAsync();
                if (cate.Count!=0)
                {
                    return BadRequest("Loại sản phẩm đã tồn tại");
                }

                _context.Add(category);

                await _context.SaveChangesAsync();
                return Ok(new
                {
                    ok = "abc"
                });
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] Category category,int id)
        {
            if (ModelState.IsValid&&id==category.Id)
            {
                var cate = await _context.Category.FindAsync(id);

                _context.Update(cate);

                await _context.SaveChangesAsync();
            }
            return Ok("Ok");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if(category!=null)
            {
                _context.Remove(category);
                await _context.SaveChangesAsync();
            }
            return Ok(category.Name);
        }

    
    }
}
