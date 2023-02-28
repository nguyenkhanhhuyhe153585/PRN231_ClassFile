using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassFileBackEnd.Controllers
{
    [ApiController]
    [Route("api/controller")]
    [Authorize]
    public class FileController : ControllerBase
    {

    }
}
