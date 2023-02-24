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
        private readonly JWTManagerRepository jwtRepo;
        private readonly ClassfileContext db;
        public AuthController(JWTManagerRepository jwtRepo, ClassfileContext db)
        {
            this.jwtRepo = jwtRepo;
            this.db = db;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] AccountDTO accountDTO)
        {
            try {
            TokenResponseDTO<Object> token = jwtRepo.Authenticate(accountDTO.Username, accountDTO.Password);
            if(token == null)
            {
                var responeMessage = new ResponseMessageDTO<Object>("Wrong Username or Password");
                return Unauthorized(responeMessage);
            }
            return Ok(token);
            }catch (Exception ex)
            {
                var responeMessage = new ResponseMessageDTO<string>(ex.Message);
                responeMessage.Data = ex.StackTrace;
                return BadRequest(responeMessage);
            }
        }

        [HttpGet("info")]
        [Authorize(Policy = "TeacherRequired")]
        public IActionResult Info()
        {      
            return Ok(db.Accounts.ToList());
        }
    }
}
