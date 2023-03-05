using ClassFileBackEnd.Common;
using ClassFileBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassFileBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly ClassfileContext db;
        private readonly string folderName = "Resources";

        public FileController(ClassfileContext db )
        {
            this.db = db;
        }

        [AllowAnonymous]
        [HttpGet("{fileName}")]
        public IActionResult Get(string fileName)
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
    }
}
