using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApPlayground.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return WebApPlayground.User.Users;
        }
        
        [HttpPost]
        public IActionResult Post(User user) 
        {
            WebApPlayground.User.Users.Add(user);
            return CreatedAtRoute("User", new { id = user.id }, user);
        }
    }
        
        
}