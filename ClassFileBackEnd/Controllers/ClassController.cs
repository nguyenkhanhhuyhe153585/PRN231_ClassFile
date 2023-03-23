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
                    }).OrderByDescending(cd=> cd.LastPost);

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
       
        [HttpPost("leave/{classId}")]
        [Authorize(Roles = Const.Role.STUDENT)]
        public IActionResult LeaveClass(int classId)
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                var currentUser = db.Accounts.Include(a => a.Classes).Where(a => a.Id == currentUserId).SingleOrDefault();
                var classQuery = db.Classes.Where(c => c.Id == classId && c.Accounts.Contains(currentUser)).FirstOrDefault();
                if(classQuery == null)
                {
                    return NotFound();
                }
                
                currentUser.Classes.Remove(classQuery);
                db.SaveChanges();
                return Ok(); 
            }catch (Exception ex) {
                ResponseMessageDTO<string> responseMsg = new(ex.Message)
                {
                    Data = ex.StackTrace
                };
                return BadRequest(responseMsg);
            }
        }

        [HttpGet("member/teacher/{id:int}")]
        [Authorize(Roles = Const.Role.TEACHER)]
        public IActionResult GetClassMemberTeacher(int id)
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                Account? currentUser = db.Accounts.Find(currentUserId);
                if (currentUser == null)
                {
                    return NotFound("This user is not found");
                }
                Class? classFromDB = db.Classes
                    .Include(a => a.Accounts)
                    .Where(c => c.Id == id && c.Accounts.Contains(currentUser))
                    .FirstOrDefault();
                if (classFromDB == null)
                {
                    return NotFound("This class is not found");
                }
                classFromDB.Accounts.Remove(currentUser);
                List<AccountProfileDTO> profiles = mapper.Map<List<AccountProfileDTO>>(classFromDB.Accounts.ToList());
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpDelete("member/teacher/{id:int}")]
        [Authorize(Roles = Const.Role.TEACHER)]
        public IActionResult DeleteStudentFromClass(int id, [FromBody] DeletedStudentDTO deletedStudentDTO)
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                Account? currentUser = db.Accounts.Find(currentUserId);
                Class? classFromDB = db.Classes
                    .Include(a => a.Accounts)
                    .Where(c => c.Id == id && c.Accounts.Contains(currentUser))
                    .FirstOrDefault();
                Account? find = classFromDB.Accounts.Where(f => f.Id == deletedStudentDTO.Id && f != currentUser).FirstOrDefault();
                if (find == null)
                {
                    return NotFound("No result found");
                }
                classFromDB.Accounts.Remove(find);
                db.Classes.Update(classFromDB);
                db.SaveChanges();
                return Ok(new ResponseMessageDTO<string>($"{find.Username} is successfully removed from class"));
            }
            catch(Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpGet("member/student/{id:int}")]
        [Authorize(Roles = Const.Role.STUDENT)]
        public IActionResult GetClassMemberStudent(int id)
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                Account? currentUser = db.Accounts.Find(currentUserId);
                if (currentUser == null)
                {
                    return NotFound("This user is not found");
                }
                Class? classFromDB = db.Classes
                    .Include(a => a.Accounts)
                    .Where(c => c.Id == id && c.Accounts.Contains(currentUser))
                    .FirstOrDefault();
                if (classFromDB == null)
                {
                    return NotFound("This class is not found");
                }
                classFromDB.Accounts.Remove(classFromDB.Accounts.Where(a => a.AccountType == Const.Role.TEACHER).FirstOrDefault());
                List<AccountProfileDTO> profiles = mapper.Map<List<AccountProfileDTO>>(classFromDB.Accounts.ToList());
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpGet("member/{id:int}")]
        public IActionResult GetTeacherOfClass(int id)
        {
            try
            {
                List<Class> classes = db.Classes.ToList();
                Account? teacher = null;
                if (classes.Count <= 0)
                {
                    return NotFound();
                }
                foreach(Class c in classes)
                {
                    if (c.Id == id)
                    {
                        teacher = db.Accounts.Find(c.TeacherAccountId);
                    }
                }
                if (teacher == null)
                {
                    return NotFound();
                }
                return Ok(mapper.Map<AccountProfileDTO>(teacher));
            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpDelete("delete/{id:int}")]
        [Authorize(Roles = Const.Role.TEACHER)]
        public IActionResult DeleteClass(int id)
        {
            try
            {
                Class? currentClass = db.Classes.Find(id);
                if (currentClass == null)
                {
                    return NotFound("This class is not exist");
                }
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                Account? creator = db.Accounts.Include(a => a.Classes)
                    .Where(a => a.Classes.Contains(currentClass) && a.Id == currentUserId)
                    .FirstOrDefault();
                if (creator == null)
                {
                    return Unauthorized("This user don't have permission to delete this class");
                }
                List<Account> accounts = db.Accounts.Include(a => a.Classes).ToList();
                foreach(Account account in accounts)
                {
                    if (account.Classes.Contains(currentClass))
                    {
                        account.Classes.Remove(currentClass);
                    }
                }
                db.Accounts.UpdateRange(accounts);
                db.Classes.Remove(currentClass);
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

        [HttpPut("regen/{id:int}")]
        [Authorize(Roles = Const.Role.TEACHER)]
        public IActionResult RegenClassCode(int id)
        {
            try
            {
                Class? currentClass = db.Classes.Find(id);
                if (currentClass == null)
                {
                    return NotFound("Class not found");
                }
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                Account? creator = db.Accounts.Include(a => a.Classes)
                    .Where(a => a.Classes.Contains(currentClass) && a.Id == currentUserId)
                    .FirstOrDefault();
                if (creator == null)
                {
                    return Unauthorized("This user don't have permission to modify this class");
                }
                Class c = null;
                string newClassCode = null;
                do
                {
                    newClassCode = Utils.RandomClassCode();
                    c = db.Classes.Where(cl => cl.ClassCode == newClassCode).FirstOrDefault();
                }
                while (c != null);
                currentClass.ClassCode = newClassCode;
                db.Classes.Update(currentClass);
                db.SaveChanges();
                return Ok(new ClassCodeDTO { ClassCode = newClassCode});
            }
            catch(Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpPut("edit/{id:int}")]
        [Authorize(Roles = Const.Role.TEACHER)]
        public IActionResult EditClass(int id, [FromBody] ClassEditDTO classEditDTO)
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
                classFromDB.ClassName = classEditDTO.ClassName;
                classFromDB.ImageCover = classEditDTO.ImageCover;
                db.Classes.Update(classFromDB);
                db.SaveChanges();
                return Ok(classEditDTO);
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
