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
        public IActionResult Index(int page)
        {
            try
            {
                int totalPage = 0;

                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                Account currentUser = db.Accounts.Single(a => a.Id == currentUserId);
                var queryClassWithLastPost = db.Classes.Include(c => c.Accounts).Include(c => c.Posts).Include(c => c.TeacherAccount)
                    .Where(c => c.Accounts.Contains(currentUser));
                (queryClassWithLastPost, totalPage, page) = Utils.MyQuery<Class>.Paging(queryClassWithLastPost, page);
                IQueryable<ClassDTO> classDTOQuery  = queryClassWithLastPost.Select(c => new ClassDTO
                    {
                        Id = c.Id,
                        ClassName = c.ClassName,
                        TeacherAccount = mapper.Map<AccountProfileDTO>(c.TeacherAccount),
                        LastPost = c.Posts.OrderByDescending(p => p.DateCreated).First().DateCreated,
                        ImageCover = c.ImageCover
                    });

                List<ClassDTO> classDTOs = classDTOQuery.ToList();
                PagingResponseDTO<List<ClassDTO>> pagingResponseDTO = new();
                pagingResponseDTO.Data = classDTOs;
                pagingResponseDTO.TotalPage = totalPage;
                pagingResponseDTO.PageIndex = page;
                pagingResponseDTO.PageSize = Const.NUMBER_RECORD_PAGE;
                return Ok(pagingResponseDTO);
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
                if (classFromDB == null)
                {
                    return NotFound();
                }
                ClassDTO classDTO = mapper.Map<ClassDTO>(classFromDB);
                return Ok(classDTO);
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }


        [HttpPost("join")]
        [Authorize(Roles = Const.Role.STUDENT)]
        public IActionResult JoinClass([FromBody] ClassCodeDTO classCodeDTO)
        {
            try
            {
                var queryClass = db.Classes.Where(c => c.ClassCode == classCodeDTO.ClassCode).FirstOrDefault();
                if (queryClass == null)
                {
                    ResponseMessageDTO<string> responseMsg = new("Class Not Found!");
                    return NotFound(responseMsg);
                }
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                var currentUser = db.Accounts.Include(a => a.Classes).Single(a => a.Id == currentUserId);
                if (currentUser.Classes.Contains(queryClass))
                {
                    throw new Exception("You have already joined this class");
                }
                queryClass.Accounts.Add(currentUser);
                db.SaveChanges();
                return Ok();
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
