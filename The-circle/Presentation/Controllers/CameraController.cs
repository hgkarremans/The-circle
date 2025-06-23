using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

namespace The_circle.Presentation.Controllers;

public class CameraController : Controller
{
    [HttpGet("/Camera/Stream")]
    public IActionResult Camera()
    {
        var certBytes = HttpContext.Session.Get("TruYouCert");
        if (certBytes == null)
            return RedirectToAction("Login", "Auth");

        ViewBag.CertBase64 = Convert.ToBase64String(certBytes);
        ViewBag.Email      = HttpContext.Session.GetString("TruYouEmail") ?? "Onbekend";

        return View();
    }

    [HttpGet("/Camera/ReceiveStream")]
    public IActionResult ReceiveStream(Guid streamId)
    {
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