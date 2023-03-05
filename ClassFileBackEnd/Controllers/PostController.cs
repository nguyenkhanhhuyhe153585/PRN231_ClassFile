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
        private readonly string imageFolder = "Resources";

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
        [RequestSizeLimit(100_000_000_000_000)]
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

                foreach (var file in form.Files)
                {
                    string fileName = file.FileName;
                    string fileType = Utils.GetFileExtension(fileName);

                    string fileNameWithoutExtension = fileName.Split("." + fileType)[0];
                    string fileNameForSaving = $"{fileNameWithoutExtension}_{post.Id}_{DateTime.Now.ToString("HHmmssddMMyyyy")}.{fileType}";

                    // Lưu file vào tệp của Server
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imageFolder, fileNameForSaving);
                    Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                    fileStream.Close();

                    // Tạo đối tượng file gắn với Post
                    ClassFileBackEnd.Models.File fileDb = new ClassFileBackEnd.Models.File();

                    fileDb.FileType = Utils.GetMimeType(fileType);
                    fileDb.FileName = fileNameForSaving;
                    fileDb.FileNameRoot = fileName;
                    fileDb.PostId = post.Id;

                    db.Files.Add(fileDb);
                }

                await db.SaveChangesAsync();
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
