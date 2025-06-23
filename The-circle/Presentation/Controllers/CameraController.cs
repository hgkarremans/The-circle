using Microsoft.AspNetCore.Mvc;

namespace The_circle.Presentation.Controllers;

public class CameraController : Controller
{
    [HttpGet("/Camera/ReceiveStream")]
    public IActionResult ReceiveStream(Guid streamId)
    {
        ViewBag.StreamId = streamId;
        return View("ReceiveStream"); 
    }

    [HttpGet("/Camera/StreamHub")]
    public IActionResult StreamingHub()
    {
        return View("Streaminghub");
    } 
    
    public IActionResult Camera()
    {
        return View();
    }
}