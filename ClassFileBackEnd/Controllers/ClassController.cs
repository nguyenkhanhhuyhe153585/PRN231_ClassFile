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
                var currentUserId = JWTManagerRepository.GetClaim(JwtRegisteredClaimNames.Name, HttpContext);
                int currentId = int.Parse(currentUserId);
                Account? currentUser = db.Accounts.Where(e=>e.Id == currentId).Include(a=>a.Classes)
                    .ThenInclude(c => c.TeacherAccount).SingleOrDefault();
                if(currentUser == null)
                {
                    return NotFound();
                }
                List<Class>? clasese = currentUser.Classes.ToList();
                List<ClassDTO> classDTOs = mapper.Map<List<ClassDTO>>(clasese);
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

        [HttpPost("create")]
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
    }
}
