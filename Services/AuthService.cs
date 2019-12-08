using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using TodoAPI.Entities;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public interface IAuthService
    {
        AuthenticationResponseModel Authenticate(string username, string password);
        SignupResponseModel CreateUser(string username, string password);
        bool UpdatePassword(int userId, string oldPassword, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly TodoContext _context;

        public AuthService(IOptions<AppSettings> appSettings, IPasswordHasher<User> passwordHasher, TodoContext context)
        {
            _appSettings = appSettings.Value;
            _passwordHasher = passwordHasher;
            _context = context;
        }

        public AuthenticationResponseModel Authenticate(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            if (user == null)
                return new AuthenticationResponseModel{ Success = false };
            
            if (_passwordHasher.VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Failed)
                return new AuthenticationResponseModel{ Success = false };

            // Authentication successful.
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResponseModel
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }

        public SignupResponseModel CreateUser(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            if (user != null)
                return new SignupResponseModel{ Success = false };

            user = new User{
                Username = username,
            };

            user.Password = _passwordHasher.HashPassword(user, password);
            
            _context.Users.Add(user);
            _context.SaveChanges();

            return new SignupResponseModel
            {
                Success = true,
            };
        }

        public bool UpdatePassword(int userId, string oldPassword, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == userId);

            if (user == null)
                return false;
            
            if (_passwordHasher.VerifyHashedPassword(user, user.Password, oldPassword) == PasswordVerificationResult.Failed)
                return false;

            user.Password = _passwordHasher.HashPassword(user, password);
            _context.SaveChanges();

            return true;
        }
    }
}