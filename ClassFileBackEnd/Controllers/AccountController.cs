using AutoMapper;
using ClassFileBackEnd.Authen;
using ClassFileBackEnd.Common;
using ClassFileBackEnd.Mapper;
using ClassFileBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassFileBackEnd.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ClassfileContext db;
        private readonly IMapper mapper;

        public AccountController(ClassfileContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        [HttpGet("my")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                Account? currentUser = db.Accounts.SingleOrDefault(c => c.Id == currentUserId);
                AccountProfileDTO accountDto = mapper.Map<AccountProfileDTO>(currentUser);
                return Ok(accountDto);
            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> mess = new ResponseMessageDTO<string>(ex.Message)
                { Data = ex.StackTrace };
                return BadRequest(mess);

            }
        }

        [HttpPut("my/edit")]
        public async Task<IActionResult> Edit(IFormCollection form)
        {
            try
            {
                string? inputUserName = form["username"];
                string? inputFullName = form["fullname"];
                string? imageAvatar = form["imageAvatar"];

                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                Account currentUser = db.Accounts.Single(c => c.Id == currentUserId);
                if (inputUserName != null && inputUserName != currentUser.Username)
                {
                    Account? accountWithUserName = db.Accounts.SingleOrDefault(c => c.Username == inputUserName);
                    if (accountWithUserName != null && !accountWithUserName.Equals(currentUser))
                    {
                        string message = $"This username: {inputUserName} is exist!";
                        ResponseMessageDTO<string> mess = new(message);
                        return BadRequest(mess);
                    }
                    currentUser.Username = inputUserName;
                }
                if (inputFullName != null && inputFullName != currentUser.Fullname)
                {
                    currentUser.Fullname = inputFullName;
                }

                if (imageAvatar != null && imageAvatar != currentUser.ImageAvatar)
                {
                    currentUser.ImageAvatar = imageAvatar;
                }

                db.Accounts.Update(currentUser);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> mess = new ResponseMessageDTO<string>(ex.Message)
                { Data = ex.StackTrace };
                return BadRequest(mess);

            }
        }

        [HttpPut("my/edit/password")]
        public IActionResult EditPassword([FromBody] PasswordDTO passwordDTO)
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                Account currentUser = db.Accounts.Single(c => c.Id == currentUserId);
                if (currentUser.Password != passwordDTO.OldPassword)
                {
                    string message = "Wrong old password";
                    ResponseMessageDTO<string> mess = new ResponseMessageDTO<string>(message);
                    return BadRequest(mess);
                }
                if (passwordDTO.NewPassword.Trim().Length == 0)
                {
                    string message = "New password empty";
                    ResponseMessageDTO<string> mess = new ResponseMessageDTO<string>(message);
                    return BadRequest(mess);
                }
                if (passwordDTO.NewPassword.Trim() != passwordDTO.ConfirmPassword.Trim())
                {
                    string message = "Confirm password is not match";
                    ResponseMessageDTO<string> mess = new ResponseMessageDTO<string>(message);
                    return BadRequest(mess);
                }
                currentUser.Password = passwordDTO.NewPassword;
                db.Accounts.Update(currentUser);
                db.SaveChanges();
                return Ok();

            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> mess = new ResponseMessageDTO<string>(ex.Message)
                { Data = ex.StackTrace };
                return BadRequest(mess);

            }
        }

    }
}
