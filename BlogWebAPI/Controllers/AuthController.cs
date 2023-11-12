using BlogWebAPI.Data;
using BlogWebAPI.Models.DTOs;
using BlogWebAPI.Models.Entities;
using BlogWebAPI.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlogWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly BlogDbContext _context;

        public AuthController(IConfiguration configuration, IUserService userService, BlogDbContext blogDbContext)
        {
            _configuration = configuration;
            _userService = userService;
            this._context = blogDbContext;
        }

        [HttpGet, Authorize]
        [Route("get-email")]
        public ActionResult<string> GetMe()
        {
            var email = _userService.GetMyEmail();
            return Ok(email);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegisterDto request)
        {

            var checkIfUserExists = await _context.Users.FirstOrDefaultAsync(e => e.Email == request.Email);

            if (checkIfUserExists != null)
            {
                return NotFound("User allready exists.");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Id = 0;
            user.Email = request.Email;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticatedResponse>> Login(UserLoginDto request)
        {
            if (request is null)
            {
                return BadRequest("Invalid client request. Insert username and password.");
            }

            var checkUser = await _context.Users.FirstOrDefaultAsync(e => e.Email == request.Email);

            if (checkUser == null)
            {
                return BadRequest("User not found.");
            }

            if (!VerifyPasswordHash(request.Password, checkUser.PasswordHash, checkUser.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(checkUser);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);

            AuthenticatedResponse response = new AuthenticatedResponse()
            {
                Token = token,
                RefreshToken = refreshToken.Token
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("token-valid-or-not-{token}")]
        public ActionResult<bool> TokenIsValid([FromRoute] string token)
        {
            StringBuilder sb = new StringBuilder();
            var tokenToChars = token.ToCharArray();
            for (int i = 0; i < 5; i++)
            {
                sb.Append(tokenToChars[i]);
            }
            if (sb.ToString() == "token")
            {
                sb.Clear();
                for (int i = 0; i < tokenToChars.Length; i++)
                {
                    if (i > 13)
                    {
                        sb.Append(tokenToChars[i]);
                    }
                }
                token = sb.ToString();
            }


            if (string.IsNullOrEmpty(token))
            {
                return Ok(false);
            }

            var jwtToken = new JwtSecurityToken(token);


            if ((jwtToken == null) || (jwtToken.ValidTo < DateTime.Now))
            {
                return Ok(false);
            }

            return Ok(true);
        }

        [HttpGet]
        [Route("invalid-token")]
        public ActionResult<bool> TokenIsInvalid()
        {
            return Ok(false);
        }

            private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);


            return jwt;
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!user.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);

            return Ok(new AuthenticatedResponse()
            {
                Token = token,
                RefreshToken = newRefreshToken.Token
            }); ;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

    }
}
