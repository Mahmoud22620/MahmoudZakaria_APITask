using MahmoudZakaria_APITask.Models;
using MahmoudZakaria_APITask.Models.DTO;
using System.IdentityModel.Tokens.Jwt;

namespace MahmoudZakaria_APITask.Services
{
    public interface IAuthService
    {
        Task<AuthDTO> RegisterAsync(RegisterDTO model);
        Task<AuthDTO> GetTokenAsync(LoginDTO model);
        Task<JwtSecurityToken> CreateJWTTokenAsync(User user);
        string GenerateRefreshToken();


    }
}
