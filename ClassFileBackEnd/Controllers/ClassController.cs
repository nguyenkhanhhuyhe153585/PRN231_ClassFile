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
                        Id = c.Id,
                        ClassName = c.ClassName,
                        TeacherAccount = mapper.Map<AccountProfileDTO>(c.TeacherAccount),
                        LastPost = c.Posts.OrderByDescending(p => p.DateCreated).First().DateCreated,
                        ImageCover = c.ImageCover
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
                Account? currentUser = db.Accounts.Find(currentUserId);
                Class? classFromDB = db.Classes.Where(c => c.Id == id && c.Accounts.Contains(currentUser)).FirstOrDefault();
                if(classFromDB == null)
                {
                    return NotFound();
                }
                ClassDTO classDTO = mapper.Map<ClassDTO>(classFromDB);
                return Ok(classDTO);
            }catch(Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = Const.Role.TEACHER)]
        public IActionResult CreateClass([FromBody] ClassCreateDTO classCreateDTO)
        {
            try
            {
                Class newClass = mapper.Map<Class>(classCreateDTO);
                int currentId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                newClass.ClassCode = Utils.RandomClassCode();
                newClass.TeacherAccountId = currentId;
                newClass.Accounts.Add(db.Accounts.Single(c => c.Id == currentId));
                db.Classes.Add(newClass);
                db.SaveChanges();

                return Ok();
            }
            catch(Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpGet("profile/{id:int}")]
        [Authorize(Roles = Const.Role.TEACHER)]
        public IActionResult GetClassProfileTeacher(int id)
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                Account? currentUser = db.Accounts.Find(currentUserId);
                Class? classFromDB = db.Classes.Include(a => a.Accounts).Where(c => c.Id == id && c.Accounts.Contains(currentUser)).FirstOrDefault();
                if (classFromDB == null)
                {
                    return NotFound();
                }
                List<AccountProfileDTO> profiles = mapper.Map<List<AccountProfileDTO>>(classFromDB.Accounts);
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }
    }
}
