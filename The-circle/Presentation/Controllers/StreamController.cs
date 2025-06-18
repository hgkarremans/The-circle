using Microsoft.AspNetCore.Mvc;
using The_circle.Application.Services;

namespace The_circle.Presentation.Controllers;

[ApiController]
public class StreamController : ControllerBase
{
    private readonly VideoFrameBufferService _buffer;

    public StreamController(VideoFrameBufferService buffer)
    {
        _buffer = buffer;
    }

    [HttpGet("/stream")]
    public async Task Stream()
    {
        Response.ContentType = "text/event-stream";

        while (!HttpContext.RequestAborted.IsCancellationRequested)
        {
            var data = _buffer.GetFrame();
            if (!string.IsNullOrEmpty(data))
            {
                await Response.WriteAsync($"data: {data}\n\n");
                await Response.Body.FlushAsync();
            }

            await Task.Delay(100);
        }
    }
    
}
