using ClassFileBackEnd.Mapper;
using ClassFileBackEnd.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClassFileBackEnd.Authen
{
    public class JWTManagerRepository
    {
        private readonly IConfiguration iconfiguration;
        private readonly ClassfileContext db;

        public JWTManagerRepository(IConfiguration iconfiguration, ClassfileContext db)
        {
            this.iconfiguration = iconfiguration;
            this.db = db;
        }
        public TokenResponseDTO<Object>? Authenticate(string username, string password)
        {
            Account? user = db.Accounts.FirstOrDefault(x => x.Username == username);
            if (user == null || user.Password != password)
            {
                return null;
            }

            var tokenKey = Encoding.UTF8.GetBytes(iconfiguration["JWT:Key"]);
            var securityKey = new SymmetricSecurityKey(tokenKey);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("Sub", user.Id.ToString()),
                new Claim("Role", user.AccountType.ToString())
            };
            var token = new JwtSecurityToken(
                issuer: iconfiguration["JWT:Issuer"],
                audience: iconfiguration["JWT:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
                );

            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponseDTO<Object>(encodeToken);

        }
    }
}
