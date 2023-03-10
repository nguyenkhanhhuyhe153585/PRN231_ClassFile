using AutoMapper;
using ClassFileBackEnd.Authen;
using ClassFileBackEnd.Common;
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
                var queryAccount = db.Accounts.Where(a => a.Id == currentUserId)
                    .Include(a => a.Classes).ThenInclude(c => c.Posts).ThenInclude(p => p.Files)
                    .Include(a => a.Classes).ThenInclude(c => c.Posts).ThenInclude(p => p.PostedAccount);
                Class? classGet = queryAccount.Single().Classes.Where(c => c.Id == classId).SingleOrDefault();
                if (classGet == null)
                {
                    return NotFound();
                }
                List<Post> posts = classGet.Posts.OrderByDescending(p => p.DateCreated).ToList();
                List<PostInClassDTO> postDTO = mapper.Map<List<PostInClassDTO>>(posts);
                return Ok(postDTO);
            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(IFormCollection form)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
                string? classIdRaw = form["classId"];
                int? classId = int.Parse(classIdRaw);

                string? title = form["content"];
                int accountId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                DateTime? created = DateTime.Now;

                Post post = new Post() {
                    ClassId = classId,
                    Title = title,
                    PostedAccountId = accountId,
                    DateCreated = created 
                };

                db.Posts.Add(post);             
                await db.SaveChangesAsync();


                # region Lưu Files

                Utils utils = new Utils();
                await utils.FileUpload(form, post, db);
                # endregion

                await transaction.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ResponseMessageDTO<string> responseMsg = new(ex.Message)
                {
                    Data = ex.StackTrace
                };
                return BadRequest(responseMsg);
            }
        }

        [HttpGet("{postId}")]
        public IActionResult GetPost(int postId)
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                var queryPost = db.Posts.Where(p => p.Id == postId && p.PostedAccountId == currentUserId)
                    .Include(p=>p.PostedAccount).Include(p=>p.Files).SingleOrDefault();
                if (queryPost == null)
                {
                    return NotFound();
                }

                PostInClassDTO postDTO = mapper.Map<PostInClassDTO>(queryPost);
                return Ok(postDTO);
            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost(IFormCollection form)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
                string? title = form["content"];
                int accountId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                DateTime? created = DateTime.Now;

                string? postIdRaw = form["id"];
                int? postId = int.Parse(postIdRaw);

                Post? postDb = db.Posts.Where(p => p.Id == postId).SingleOrDefault();
                if(postDb == null) { return NotFound(); }

                postDb.Title = title;

                db.Posts.Update(postDb);
                await db.SaveChangesAsync();

                # region Lưu Files

                Utils utils = new Utils();
                await utils.FileUpload(form, postDb, db);
                # endregion
                
                await transaction.CommitAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }
    }
}
