using Microsoft.AspNetCore.Mvc;
using The_circle.Application.Services;

namespace The_circle.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamsController : ControllerBase
{
    private readonly VideoFrameBufferService _buffer;

    public StreamsController(VideoFrameBufferService buffer)
    {
        _buffer = buffer;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var active = _buffer.GetActiveStreamIds(TimeSpan.FromSeconds(10));
        return Ok(active);
    }
}
