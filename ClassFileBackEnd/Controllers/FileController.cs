using AutoMapper;
using ClassFileBackEnd.Authen;
using ClassFileBackEnd.Common;
using ClassFileBackEnd.Mapper;
using ClassFileBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ClassFileBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly ClassfileContext db;
        private readonly IMapper mapper;
        private readonly string folderName = Const.ROOT_FOLDER_NAME;

        public FileController(ClassfileContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("{fileName}")]
        public IActionResult Get(string fileName)
        {
            try
            {
                string subFolderName = Const.folederModeMapping["post"];
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName, subFolderName, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                };
                Byte[] b = System.IO.File.ReadAllBytes(filePath);

                string fileType = Utils.GetFileExtension(fileName);
                string mimeType = Utils.GetMimeType(fileType);
                return File(b, mimeType);
            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpGet("{folderMode}/{fileName}")]
        [AllowAnonymous]
        public IActionResult GetAvatar(string folderMode, string fileName)
        {
            try
            {
                string subFolderName = Const.folederModeMapping[folderMode];
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName, subFolderName, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                };
                Byte[] b = System.IO.File.ReadAllBytes(filePath);

                string fileType = Utils.GetFileExtension(fileName);
                string mimeType = Utils.GetMimeType(fileType);
                return File(b, mimeType);
            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        // Chưa dùng được Upload!!!
        [HttpPost]
        public async Task<IActionResult> Upload(IFormCollection form)
        {
            try
            {
                string subFolder = "";
                List<FileDTO> fileDTOs = new List<FileDTO>();
                foreach (var file in form.Files)
                {
                    string fileName = file.FileName;
                    string fileType = Utils.GetFileExtension(fileName);

                    string fileNameWithoutExtension = fileName.Split("." + fileType)[0]; ;
                    string fileNameForSaving = "";
                    string filePath = "";
                    if (!Const.folederModeMapping.ContainsKey(form["fileMode"]))
                    {
                        throw new Exception("Folder Mode Not Accepted");
                    };
                    subFolder = Const.folederModeMapping[form["fileMode"]];
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

                    // Lưu file vào entity File trong db
                    //ClassFileBackEnd.Models.File fileDb = new ClassFileBackEnd.Models.File();
                    //fileDb.FileType = Utils.GetMimeType(fileType);
                    //fileDb.FileName = fileNameForSaving;
                    //fileDb.FileNameRoot = fileName;

                    //db.Files.Add(fileDb);
                    //db.SaveChanges();

                    FileDTO fileRes = new FileDTO()
                    {
                        FileType = Utils.GetMimeType(fileType),
                        FileName = fileNameForSaving,
                        FileNameRoot = fileName,
                    };
                    fileDTOs.Add(fileRes);
                }
                return Ok(fileDTOs);
            }
            catch (Exception ex)
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }

        [HttpDelete("{postId}/{fileId}")]
        public IActionResult Delete(int postId, int fileId)
        {
            try
            {
                int currentUserId = JWTManagerRepository.GetCurrentUserId(HttpContext);
                var queryPost = db.Posts.Where(p => p.Id == postId && p.PostedAccountId == currentUserId).Include(p => p.Files).SingleOrDefault();
                if (queryPost == null)
                {
                    return NotFound();
                }
                var queryFile = queryPost.Files.Where(f => f.Id == fileId).SingleOrDefault();
                if (queryFile == null)
                {
                    return NotFound();
                }
                db.Files.Remove(queryFile);
                db.SaveChanges();
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName, queryFile.FileNameRoot);
                if (!System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                };
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
