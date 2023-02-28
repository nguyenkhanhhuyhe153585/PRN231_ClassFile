using AutoMapper;
using ClassFileBackEnd.Authen;
using ClassFileBackEnd.Mapper;
using ClassFileBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassFileBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly ClassfileContext db;
        private readonly IMapper mapper;
        public PostController(ClassfileContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetPostOfClass(int classId)
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                var queryAccount = db.Accounts.Where(a => a.Id == currentUserId).Include(a => a.Classes)
                    .ThenInclude(c => c.Posts).ThenInclude(p=>p.Files)
                    .Include(a => a.Classes).ThenInclude(c=>c.Posts).ThenInclude(p=>p.PostedAccount);
                Class? classGet = queryAccount.Single().Classes.Where(c => c.Id == classId).SingleOrDefault();
                if(classGet == null)
                {
                    return NotFound();
                }
                List<Post> posts = classGet.Posts.ToList();
                List<PostInClassDTO> postDTO = mapper.Map<List<PostInClassDTO>>(posts);
                return Ok(postDTO);
            }catch(Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }
    }
}
