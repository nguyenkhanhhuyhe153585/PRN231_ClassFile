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
        public IActionResult GetPostOfClass(int classId, int page)
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                var currentUser = db.Accounts.Find(currentUserId);
                var queryClassCheckRight = db.Classes.Include(c => c.Accounts).Where(c => c.Id == classId && c.Accounts.Contains(currentUser));
                if(queryClassCheckRight.Count() == 0)
                {
                    return Unauthorized();
                }
              
                var queryClass = queryClassCheckRight.First();
                var queryPost = db.Posts.Include(p => p.PostedAccount).Include(p => p.Files).Where(p => p.ClassId == classId)
                    .OrderByDescending(p => p.DateCreated);
                (IQueryable<Post>, int) pagingResult = Utils.MyQuery<Post>.Paging(queryPost, page);
                List<Post> posts = pagingResult.Item1.ToList();

                List<PostInClassDTO> postDTO = mapper.Map<List<PostInClassDTO>>(posts);

                var respone = new PagingResponseDTO<List<PostInClassDTO>>
                {
                    Data = postDTO,
                    PageIndex = page,
                    TotalPage = pagingResult.Item2,
                    PageSize = Const.NUMBER_RECORD_PAGE
                };

                return Ok(respone);
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

                Post post = new Post()
                {
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
                    .Include(p => p.PostedAccount).Include(p => p.Files).Include(p => p.Class).SingleOrDefault();
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
                if (postDb == null) { return NotFound(); }

                postDb.Title = title;

                db.Posts.Update(postDb);
                await db.SaveChangesAsync();

                # region Lưu Files

                Utils utils = new Utils();
                await utils.FileUpload(form, postDb, db);
                #endregion

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
