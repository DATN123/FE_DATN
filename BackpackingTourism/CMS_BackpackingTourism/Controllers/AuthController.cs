using Microsoft.AspNetCore.Mvc;

namespace CMS_BackpackingTourism.Controllers
{
    public class AuthController : Controller
    {
        
        public IActionResult Login()
        {
            return View();
        }
    }
}
