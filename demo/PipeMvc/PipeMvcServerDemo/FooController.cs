using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PipeMvcServerDemo;

[Route("api/[controller]")]
[ApiController]
public class FooController : ControllerBase
{
    public FooController(ILogger<FooController> logger)
    {
        Logger = logger;
    }

    public ILogger<FooController> Logger { get; }

    [HttpGet]
    public IActionResult Get()
    {
        Logger.LogInformation("FooController_Get");
        return Ok(DateTime.Now.ToString());
    }

    [HttpGet("Add")]
    public IActionResult Add(int a, int b)
    {
        Logger.LogInformation($"FooController_Add a={a};b={b}");
        return Ok(a + b);
    }

    [HttpPost]
    public IActionResult Post()
    {
        Logger.LogInformation("FooController_Post");
        return Ok($"POST {DateTime.Now}");
    }

    [HttpPost("PostFoo")]
    public IActionResult PostFooContent(FooContent foo)
    {
        Logger.LogInformation($"FooController_PostFooContent Foo1={foo.Foo1};Foo2={foo.Foo2 ?? "<NULL>"}");
        return Ok($"PostFooContent Foo1={foo.Foo1};Foo2={foo.Foo2 ?? "<NULL>"}");
    }
}
