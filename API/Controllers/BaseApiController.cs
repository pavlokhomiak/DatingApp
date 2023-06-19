using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // it will look for the LogUserActivity class and run it before any method in this controller
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}