using System.Security.Cryptography;
using System.Text;
using ShopApi.Models;
using ShopApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Services
{
    public interface IUserService
    {
        Task<string> Authenticate(string name, string password);
        Task<string> Register(string name, string password);
        Task<UserModel?> Authorize(string token, object? rules = null);
    }

    public class UserService : IUserService
    {
        private readonly ShopContext _context;

        public UserService(ShopContext context)
        {
            _context = context;
        }

        public async Task<string> Authenticate(string name, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == name);
            if (user is null) return "";
            
            var validPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!validPassword) return "";

            var dtNow = DateTime.Now;    
            var auth = new AuthTokenModel
            {
                UserId = user.Id,
                Token = Tools.GetUniqueKey(20),
                DateIssued = dtNow,
                DateExpire = dtNow.AddDays(1)
            };
            
            await _context.Tokens.AddAsync(auth);
            await _context.SaveChangesAsync();

            return auth.Token;
        }

        public async Task<string> Register(string name, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == name);

            if (existingUser != null)
                return "Пользователь с таким именем уже существует";
            
            var cryptoPass = BCrypt.Net.BCrypt.HashPassword(password);
            var newOrder = new UserModel
            {
                Name = name,
                Password = cryptoPass,
            };

            try
            {
                await _context.Users.AddAsync(newOrder);
                await _context.SaveChangesAsync();

                return "";
            }

            catch (Exception ex)
            {
                return $"При регистрации пользователя произошла внутренняя ошибка: {ex.Message}";
            }
        }

        public async Task<UserModel?> Authorize(string token, object? rules = null)
        {
            var existingToken = await _context.Tokens.FirstOrDefaultAsync(t => t.Token == token);
            if (existingToken is null || !CheckTokenDates(existingToken)) return null;

            var existingUser = await CheckTokenOwner(existingToken);
            return existingUser ?? null;
        }

        private static bool CheckTokenDates(AuthTokenModel authToken)
        {
            return authToken.DateExpire > DateTime.Now && authToken.DateExpire > authToken.DateIssued;
        }

        private async Task<UserModel?> CheckTokenOwner(AuthTokenModel authToken)
        {
            var existingUser = await _context.Users.FindAsync(authToken.UserId);
            if (existingUser is null || existingUser.IsBanned) return null;
            return existingUser;
        }    
    }
}
