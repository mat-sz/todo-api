using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
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
        ResponseModel Authenticate(string username, string password);
        bool CreateUser(string username, string password);
        bool UpdatePassword(User user, string oldPassword, string password);
        User GetUserFromIdentity(IIdentity identity);
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

        public ResponseModel Authenticate(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            if (user == null) {
                return new ResponseModel {
                    Success = false,
                    Error = new ErrorModel {
                        Message = "The username or password is incorrect."
                    }
                };
            }
            
            if (_passwordHasher.VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Failed) {
                return new ResponseModel {
                    Success = false,
                    Error = new ErrorModel {
                        Message = "The username or password is incorrect."
                    }
                };
            }

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

            return new ResponseModel {
                Success = true,
                Data = tokenHandler.WriteToken(token)
            };
        }

        public bool CreateUser(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            if (user != null)
                return false;

            user = new User{
                Username = username,
            };

            user.Password = _passwordHasher.HashPassword(user, password);
            
            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }

        public bool UpdatePassword(User user, string oldPassword, string password)
        {
            if (_passwordHasher.VerifyHashedPassword(user, user.Password, oldPassword) == PasswordVerificationResult.Failed)
                return false;

            user.Password = _passwordHasher.HashPassword(user, password);
            _context.SaveChanges();

            return true;
        }

        public User GetUserFromIdentity(IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            int userId;

            if (!Int32.TryParse(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value, out userId))
                return null;

            return _context.Users.SingleOrDefault(x => x.Id == userId);
        }
    }
}