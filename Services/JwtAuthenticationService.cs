using EventBookingAPI.DBContext;
using EventBookingAPI.DTOs;
using EventBookingAPI.Helpers;
using EventBookingAPI.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventBookingAPI.Services
{
    public class JwtAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _usersRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public JwtAuthenticationService(IConfiguration configuration, IUsersRepository usersRepository, IRefreshTokenRepository refreshTokenRepository) { 
            _configuration = configuration;
            _usersRepository = usersRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponseDto> Authenticate(LoginDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password)) return null;

            var user = await _usersRepository.GetByEmailAsync(request.Email);
            if (user is null || !PasswordHelper.VerifyPassword(request.Password!, user.PasswordHash))
            {
                return null;
            }
            return await GenerateJwtToken(user);
        }

        private async Task<LoginResponseDto> GenerateJwtToken(User usr)
        {
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMinutes");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);
            var expiresInSeconds = (int)(tokenExpiryTimeStamp - DateTime.UtcNow).TotalSeconds;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Correct claim array syntax
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, usr.Username),
                new Claim(ClaimTypes.Email, usr.Email),
                new Claim(ClaimTypes.Role, usr.RoleId.ToString()),
                new Claim(ClaimTypes.SerialNumber, usr.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: tokenExpiryTimeStamp,
                signingCredentials: credentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto
            {
                UserName = usr.Username,
                AccessToken = accessToken,
                Email = usr.Email,
                ExpiresIn = Math.Max(expiresInSeconds, 0),
                RefreshToken = await GenerateRefreshToken(usr.Id) // ensure never negative
            };
        }

        public async Task<string> GenerateRefreshToken(int userId)
        {
            var refreshTokenValidityMins = _configuration.GetValue<int>("JwtConfig:RefreshTokenValidityMinutes");

            var refreshToken = new RefreshToken()
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(refreshTokenValidityMins),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
            };

             await _refreshTokenRepository.Create(refreshToken);

            return refreshToken.Token;

        }

    }
}
