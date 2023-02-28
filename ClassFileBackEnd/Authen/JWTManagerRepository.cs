using ClassFileBackEnd.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClassFileBackEnd.Authen
{
    public class JWTManagerRepository
    {
        private readonly IConfiguration configuration;
        private readonly ClassfileContext db;

        public JWTManagerRepository(IConfiguration iconfiguration, ClassfileContext db)
        {
            this.configuration = iconfiguration;
            this.db = db;
        }
        public string? Authenticate(string username, string password)
        {
            Account? user = db.Accounts.FirstOrDefault(x => x.Username == username);
            if (user == null || user.Password != password)
            {
                return null;
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Role, user.AccountType.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Id.ToString())
            };
            var tokenKey = Encoding.UTF8.GetBytes(configuration["JWT:Key"]);
            var securityKey = new SymmetricSecurityKey(tokenKey);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
                );
            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);

            return encodeToken;
        }


        public static string? GetClaim(string claimName, HttpContext context)
        {
            string? value = null;
            var identity = context.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                value = identity.FindFirst(claimName)?.Value;
            }
            return value;
        }

        public static int GetCurrentUserId(HttpContext context)
        {
            string userIdRaw = GetClaim(JwtRegisteredClaimNames.Name, context);
            if (userIdRaw == null)
            {
                return 0;
            }
            return Convert.ToInt32(userIdRaw);
        }
    }
}
