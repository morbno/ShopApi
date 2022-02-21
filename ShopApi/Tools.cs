using System.Security.Cryptography;
using System.Text;
using ShopApi.Models;
using ShopApi.Models.Entities;

namespace ShopApi;

public static class Tools
{
    public static string GetUniqueKey(int size)
    {
        var chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        var data = new byte[4 * size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(data);
        }
        var result = new StringBuilder(size);
        for (var i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % chars.Length;

            result.Append(chars[idx]);
        }

        return result.ToString();
    }

    public static async Task UpdateAsync<T>(ShopContext context, T item) where T : EntityModel
    {
        EntityModel? entity;
        switch (item)
        {
            case OrderModel:
                entity = await context.Orders.FindAsync(item.Id);
                break;
            case ShopItemModel:
                entity = await context.ShopItems.FindAsync(item.Id);
                break;
            case UserModel:
                entity = await context.Users.FindAsync(item.Id);
                break;
            case ShopCartModel:
                entity = await context.ShopCartRecords.FindAsync(item.Id);
                break;
            default:
                return;
        }
        
        if (entity is null)
            return;
            
        context.Entry(entity).CurrentValues.SetValues(item);
    }
}