using Microsoft.AspNetCore.Mvc;

namespace ClassFileBackEnd.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
