using System.Security.Cryptography.X509Certificates;
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
        // ▶︎ Extract the CN once from the session-stored cert:
        string email = "Onbekend";
        var certBytes = HttpContext.Session.Get("TruYouCert");
        if (certBytes != null)
        {
            try
            {
                var x509 = new X509Certificate2(certBytes);
                var cn = x509.GetNameInfo(X509NameType.SimpleName, false);
                if (!string.IsNullOrEmpty(cn))
                    email = cn;
                Console.WriteLine($"[ReceiveStream] Extracted CN: {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReceiveStream] Error extracting CN: {ex.Message}");
            }
        }

        ViewBag.Email    = email;
        ViewBag.StreamId = streamId;
        return View();
    }


    [HttpGet("/Camera/StreamHub")]
    public IActionResult StreamingHub() => View();
}