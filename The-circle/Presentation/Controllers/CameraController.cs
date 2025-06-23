using Microsoft.AspNetCore.Mvc;

namespace The_circle.Presentation.Controllers;

public class CameraController : Controller
{
    [HttpGet("/Camera/Stream")]
    public IActionResult Camera()
    {
        // 🔑 Read the very same session key
        var certBytes = HttpContext.Session.Get("TruYouCert");
        if (certBytes == null)
            return RedirectToAction("Login", "Auth");

        // Pass base64−no−newlines into the view
        ViewBag.CertBase64 = Convert.ToBase64String(certBytes);
        ViewBag.Email      = HttpContext.Session.GetString("TruYouEmail") ?? "Onbekend";

        return View(); // will render Camera.cshtml
    }

    [HttpGet("/Camera/ReceiveStream")]
    public IActionResult ReceiveStream(Guid streamId)
    {
        ViewBag.StreamId = streamId;
        return View();
    }

    [HttpGet("/Camera/StreamHub")]
    public IActionResult StreamingHub() => View();
}