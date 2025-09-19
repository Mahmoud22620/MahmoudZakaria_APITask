using MahmoudZakaria_APITask.Models;
using MahmoudZakaria_APITask.Models.DTO;
using MahmoudZakaria_APITask.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace MahmoudZakaria_APITask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly UserManager<User> _UserManager;
        public AuthController(IAuthService authService , UserManager<User> userManager )
        {
            this.authService = authService;
            _UserManager = userManager;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            Console.WriteLine($"DTO -> UserName: {registerDTO.UserName}, Email: {registerDTO.Email}, Password: {registerDTO.Password}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await authService.RegisterAsync(registerDTO);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await authService.GetTokenAsync(loginDTO);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDTO model)
        {
            var user = await _UserManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == model.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid refresh token.");

            var jwtSecurityToken = await authService.CreateJWTTokenAsync(user);
            user.RefreshToken = authService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _UserManager.UpdateAsync(user);

            return Ok(new AuthDTO
            {
                Email = user.Email,
                UserName = user.UserName,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpireOn = jwtSecurityToken.ValidTo,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                IsAuthenticated = true,
                Role = await _UserManager.GetRolesAsync(user) as List<string>
            });
        }
    }
}
