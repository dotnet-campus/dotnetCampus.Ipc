using Microsoft.AspNetCore.Mvc;

namespace PipeMvcServerDemo;

[Route("api/[controller]")]
[ApiController]
public class FooController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(DateTime.Now.ToString());
    }
}
