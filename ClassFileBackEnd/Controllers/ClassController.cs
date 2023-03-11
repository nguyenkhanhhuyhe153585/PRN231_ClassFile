using AutoMapper;
using ClassFileBackEnd.Authen;
using ClassFileBackEnd.Common;
using ClassFileBackEnd.Mapper;
using ClassFileBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClassFileBackEnd.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ClassController : Controller
    {
        private readonly ClassfileContext db;
        private readonly IMapper mapper;
        public ClassController(ClassfileContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId (HttpContext);
                Account currentUser = db.Accounts.Single (a => a.Id == currentUserId);             
                var queryClassWithLastPost = db.Classes.Include(c => c.Accounts).Include(c => c.Posts).Include(c => c.TeacherAccount)
                    .Where(c => c.Accounts.Contains(currentUser))
                    .Select(c => new ClassDTO
                    {
                        Id = currentUserId,
                        ClassName = c.ClassName,
                        TeacherAccount = mapper.Map<AccountProfileDTO>(c.TeacherAccount),
                        LastPost = c.Posts.OrderByDescending(p => p.DateCreated).First().DateCreated,
                    });

                List<ClassDTO> classDTOs = queryClassWithLastPost.ToList();
                return Ok(classDTOs);
            }
            catch (Exception ex) 
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetClass(int id)
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                Account? currentUser = db.Accounts.Where(a => a.Id== currentUserId).Include(a=>a.Classes).SingleOrDefault();
                if (currentUser == null)
                {
                    return NotFound();
                }
                Class classGet = currentUser.Classes.SingleOrDefault(c => c.Id == id);
                if(classGet == null)
                {
                    return NotFound();
                }
                ClassDTO classDTO = mapper.Map<ClassDTO>(classGet);
                return Ok(classDTO);
            }catch(Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }
    }
}
