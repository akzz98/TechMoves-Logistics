using Microsoft.AspNetCore.Mvc;
using TechMovesLogistics.Api.Dtos;
using TechMovesLogistics.Api.Services;

namespace TechMovesLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IConfiguration configuration, IJwtTokenService jwtTokenService)
        {
            _configuration = configuration;
            _jwtTokenService = jwtTokenService;
        }

        // POST /api/auth/login
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            var validUsername = _configuration["Auth:Username"];
            var validPassword = _configuration["Auth:Password"];

            if (request.Username != validUsername || request.Password != validPassword)
                return Unauthorized();

            var token = _jwtTokenService.GenerateToken(request.Username, out var expiresAt);

            return Ok(new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                Username = request.Username
            });
        }
    }
}
