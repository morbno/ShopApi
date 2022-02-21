using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using ShopApi.Models;
using ShopApi.Models.Entities;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly IUserService _userService;

        public ShopController(ShopContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShopItemModel>>> GetAllShopItems([FromQuery] PaginationModel pagination)
        {
            try
            {
                var data = await _context.ShopItems
                    .Skip((pagination.StartPage - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync();

                var totalItemsCount = await _context.ShopItems.CountAsync();

                return Ok(new PageResponse<List<ShopItemModel>>(data, pagination, totalItemsCount));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("{id:long}")]
        public async Task<IActionResult> GetShopItemById(long id)
        {
            try
            {
                var item = await _context.ShopItems
                    .FirstAsync(i => i.Id == id);
                return Ok(new Response<ShopItemModel>(item));
            }
            catch
            {
                return BadRequest(new Response<ShopItemModel>("Товар не найден!"));
            }
        }
        
        [Route("modifyCart")]
        [HttpPost]
        public async Task<ActionResult> ModifyCart(string token, OrderRecordBase input)
        {
            var loggedUser = await _userService.Authorize(token);
            if (loggedUser is null) return Unauthorized();
            
            var currentRecord = await _context.ShopCartRecords
                .FirstOrDefaultAsync(r => r.ShopItemId == input.ShopItemId && r.UserId == loggedUser.Id);
            
            if (currentRecord is null)
            {
                if (input.Quantity is 0)
                    return BadRequest();
                
                currentRecord = new ShopCartModel
                {
                    UserId = loggedUser.Id,
                    ShopItemId = input.ShopItemId,
                    Quantity = input.Quantity
                };

                await _context.AddAsync(currentRecord);
            }
            else
            {
                if (input.Quantity is 0)
                    _context.Entry(currentRecord).State = EntityState.Deleted;
                
                else
                {
                    currentRecord.Quantity = input.Quantity;
                    await Tools.UpdateAsync(_context, currentRecord);
                }
            }
            
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
