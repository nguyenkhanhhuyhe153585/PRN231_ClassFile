using AutoMapper;
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
        private readonly IMapper mapper;
        public AuthController(IConfiguration config, ClassfileContext db, IMapper mapper)
        {
            this.config = config;
            this.db = db;
            this.mapper = mapper;
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

        [HttpPost("signup")]
        [AllowAnonymous]
        public IActionResult SignUp([FromBody] AccountSignupDTO accountSignupDTO)
        {
            try
            {
                var jwtRepo = new JWTManagerRepository(config, db);
                var msg = jwtRepo.SignUpAccount(accountSignupDTO.Username, accountSignupDTO.Password, accountSignupDTO.Password2);
                if (msg != null)
                {
                    var response = new ResponseMessageDTO<string>(msg);
                    return BadRequest(response);
                }
                db.Accounts.Add(mapper.Map<Account>(accountSignupDTO));
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                var responeMessage = new ResponseMessageDTO<string>(ex.Message);
                responeMessage.Data = ex.StackTrace;
                return BadRequest(responeMessage);
            }
        }
    }
}
