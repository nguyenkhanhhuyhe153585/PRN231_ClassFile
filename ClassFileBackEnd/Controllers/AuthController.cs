using ClassFileBackEnd.Authen;
using ClassFileBackEnd.Mapper;
using ClassFileBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassFileBackEnd.Controllers
{
    [Route("api/auth")]
    [Authorize]
    public class AuthController : Controller
    {
        private readonly IConfiguration config;
        private readonly ClassfileContext db;
        public AuthController(IConfiguration config, ClassfileContext db)
        {
            this.config = config;
            this.db = db;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] AccountDTO accountDTO)
        {
            try
            {
                var jwtRepo = new JWTManagerRepository(config, db);
                var token = jwtRepo.Authenticate(accountDTO.Username, accountDTO.Password);
                if (token == null)
                {
                    var responeMessage = new ResponseMessageDTO<Object>("Wrong Username or Password");
                    return Unauthorized(responeMessage);
                }

                return Ok(new TokenResponseDTO<Object>(token));
            }
            catch (Exception ex)
            {
                var responeMessage = new ResponseMessageDTO<string>(ex.Message);
                responeMessage.Data = ex.StackTrace;
                return BadRequest(responeMessage);
            }
        }

        [HttpPost("info")]
        [Authorize(Roles = "TC")]
        public IActionResult Info()
        {
            return Ok(db.Accounts.ToList());
        }
    }
}
