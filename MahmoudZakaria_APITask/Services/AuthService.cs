using MahmoudZakaria_APITask.Helpers;
using MahmoudZakaria_APITask.Models;
using MahmoudZakaria_APITask.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MahmoudZakaria_APITask.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _UserManager;
        private readonly JWT _JWT;

        public AuthService(UserManager<User> userManager , IOptions<JWT> Jwt )
        {
            _UserManager = userManager;
            _JWT = Jwt.Value;
        }
        public async Task<AuthDTO> GetTokenAsync(LoginDTO model)
        {
            var Auth = new AuthDTO();
            var user = await _UserManager.FindByEmailAsync(model.Email);
            if (user is null || !await _UserManager.CheckPasswordAsync(user, model.Password))
            {
                Auth.IsAuthenticated = false;
                Auth.Message = "Email or Password is incorrect";
                return Auth;
            }
            var jwtSecurityToken = await CreateJWTTokenAsync(user);
            var roles = await _UserManager.GetRolesAsync(user);
            Auth.IsAuthenticated = true;
            Auth.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            Auth.Email = user.Email;
            Auth.UserName = user.UserName;
            Auth.ExpireOn = jwtSecurityToken.ValidTo;
            Auth.Role = await _UserManager.GetRolesAsync(user) as List<string>;

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _UserManager.UpdateAsync(user);

            Auth.RefreshToken = user.RefreshToken;
            Auth.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime ?? default(DateTime);

            return Auth;
        }

        public async Task<AuthDTO> RegisterAsync(RegisterDTO model)
        {
            if (await _UserManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthDTO { Message = "Email is already registered", IsAuthenticated = false };
            }

            if (await _UserManager.FindByNameAsync(model.UserName) is not null)
            {
                return new AuthDTO { Message = "Username is already registered", IsAuthenticated = false };
            }

            var user = new User
            {
                Email = model.Email,
                UserName = model.UserName
            };

            var result = await _UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthDTO { Message = errors, IsAuthenticated = false };
            }

            await _UserManager.AddToRoleAsync(user, model.Role);

            var jwtSecurityToken = await CreateJWTTokenAsync(user);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(60);
            await _UserManager.UpdateAsync(user);

            return new AuthDTO
            {
                Email = user.Email,
                ExpireOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Role = await _UserManager.GetRolesAsync(user) as List<string>,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime?? default(DateTime)
            };

        }
        public async Task<JwtSecurityToken> CreateJWTTokenAsync(User user)
        {
            var userClaims = await _UserManager.GetClaimsAsync(user);
            var roles = await _UserManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("uid", user.Id.ToString()) 
            }.Union(userClaims).Union(roleClaims);

            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWT.key));
            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _JWT.Issuer,
                audience: _JWT.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_JWT.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
