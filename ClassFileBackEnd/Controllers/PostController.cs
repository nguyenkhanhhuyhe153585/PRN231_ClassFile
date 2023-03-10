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

                string folderName = Const.ROOT_FOLDER_NAME;
                string filePath = "";
                string fileNameForSaving = "";

                string subFolder = Const.folederModeMapping[form["fileMode"]];
                if (!Const.folederModeMapping.ContainsKey(form["fileMode"]))
                {
                    throw new Exception("Folder Mode Not Accepted");
                };

                foreach (var file in form.Files)
                {
                    string fileName = file.FileName;
                    string fileType = Utils.GetFileExtension(fileName);
                    string fileNameWithoutExtension = fileName.Split("." + fileType)[0];         
                    
                    // Triển khai khởi tạo tên file tới khi không có file nào trùng trong Dir
                    int index = 0;
                    string indexString = "";
                    do
                    {
                        if (index != 0)
                        {
                            indexString = $"_({index})_";
                        }
                        fileNameForSaving = $"{fileNameWithoutExtension}{indexString}{DateTime.Now.ToString("HHmmssddMMyyyy")}.{fileType}";
                        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName, subFolder, fileNameForSaving);
                    }
                    while (System.IO.File.Exists(filePath));

                    // Lưu file vào tệp của Server
                    Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                    fileStream.Close();

                    // Tạo đối tượng file gắn với Post
                    ClassFileBackEnd.Models.File fileDb = new()
                    {
                        FileType = Utils.GetMimeType(fileType),
                        FileName = fileNameForSaving,
                        FileNameRoot = fileName,
                        PostId = post.Id
                    };

                    db.Files.Add(fileDb);
                }

                await db.SaveChangesAsync();

                #endregion

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

                foreach (var file in form.Files)
                {
                    string fileName = file.FileName;
                    string fileType = Utils.GetFileExtension(fileName);

                    string fileNameWithoutExtension = fileName.Split("." + fileType)[0];
                    string fileNameForSaving = $"{fileNameWithoutExtension}_{postDb.Id}_{DateTime.Now.ToString("HHmmssddMMyyyy")}.{fileType}";

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
                    fileDb.PostId = postDb.Id;

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
