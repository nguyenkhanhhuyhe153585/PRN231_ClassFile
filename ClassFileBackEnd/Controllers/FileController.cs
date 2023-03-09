using ClassFileBackEnd.Authen;
using ClassFileBackEnd.Common;
using ClassFileBackEnd.Mapper;
using ClassFileBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ClassFileBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly ClassfileContext db;
        private readonly string folderName = "Resources";

        public FileController(ClassfileContext db)
        {
            this.db = db;
        }

        [AllowAnonymous]
        [HttpGet("{fileName}")]
        public IActionResult Get(string fileName)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName, fileName);
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
