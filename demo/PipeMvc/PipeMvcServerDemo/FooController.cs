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

    [HttpGet("Add")]
    public IActionResult Add(int a, int b)
    {
        return Ok(a + b);
    }

    [HttpPost]
    public IActionResult Post()
    {
        return Ok($"POST {DateTime.Now}");
    }

    [HttpPost("PostFoo")]
    public IActionResult PostFooContent(FooContent foo)
    {
        return Ok($"PostFooContent Foo1={foo.Foo1};Foo2={foo.Foo2 ?? "<NULL>"}");
    }
}
