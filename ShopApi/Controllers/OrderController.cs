using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using ShopApi.Models;
using ShopApi.Models.Entities;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly IUserService _userService;

        public OrderController(ShopContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderModel>>> GetOrders(string token)
        {
            if (await _userService.Authorize(token) is null) return Unauthorized();
            
            return await _context.Orders
                .Select(x => x)
                .ToListAsync();
        }

        [HttpPost]
        [Route("new")]
        public async Task<ActionResult> CreateOrder(string token)
        {
            try
            {
                var loggedUser = await _userService.Authorize(token);
                if (loggedUser is null) return Unauthorized();
                
                var userCartItems = await _context.ShopCartRecords
                    .Where(r => r.UserId == loggedUser.Id)
                    .ToListAsync();

                if (!userCartItems.Any()) return BadRequest("Невозможно создать заказ, в котором нет предметов");
                
                var storageItemsToCheck = new List<ShopItemModel>();

                foreach (var cartRecord in userCartItems)
                {
                    var shopItem = await _context.ShopItems.FindAsync(cartRecord.ShopItemId);
                    if (shopItem is null)
                        return BadRequest("Данный товар больше не представлен в базе");
                    storageItemsToCheck.Add(shopItem);
                }
                
                decimal totalOrderCost = 0;

                foreach (var cartItem in userCartItems)
                {
                    var storageItem = storageItemsToCheck
                        .First(i => i.Id == cartItem.ShopItemId);

                    if (storageItem.Quantity < cartItem.Quantity)
                        return BadRequest($"На складе закончился товар {storageItem.Name}");

                    totalOrderCost += storageItem.Price * cartItem.Quantity;
                }

                var newOrder = new OrderModel
                {
                    UserId = loggedUser.Id,
                    Status = OrderStatus.Processing,
                    Price = totalOrderCost
                };
                await _context.Orders.AddAsync(newOrder);
                await _context.SaveChangesAsync();

                foreach (var shopCartItemRecord in userCartItems)
                {
                    var newOrderItem = new OrderItemModel
                    {
                        OrderId = newOrder.Id,
                        Quantity = shopCartItemRecord.Quantity,
                        ShopItemId = shopCartItemRecord.ShopItemId
                    };

                    await _context.OrderRecords.AddAsync(newOrderItem);

                    var itemToUpdate = storageItemsToCheck
                        .First(i => i.Id == shopCartItemRecord.ShopItemId);

                    itemToUpdate.Quantity -= shopCartItemRecord.Quantity;
                    await Tools.UpdateAsync(_context, itemToUpdate);
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("proceed")]
        public async Task<ActionResult> ProceedOrder(string token, long orderId)
        {
            if (await _userService.Authorize(token) is null) return Unauthorized();

            var orderToProceed = await _context.Orders.FirstAsync(o => o.Id == orderId);
            
            if (orderToProceed.Status is OrderStatus.Finished)
                return BadRequest("Заказ уже отправлен");

            orderToProceed.Status = OrderStatus.Finished;

            await Tools.UpdateAsync(_context, orderToProceed);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
