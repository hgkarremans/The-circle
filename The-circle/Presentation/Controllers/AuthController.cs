using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

namespace The_circle.Presentation.Controllers;

public class AuthController : Controller
{
    [HttpGet("/login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("/login")]
    public IActionResult Login(IFormFile? certFile, string email)
    {
        if (certFile == null || string.IsNullOrEmpty(email))
        {
            ViewBag.Error = "Both email and certificate are required.";
            return View();
        }

        using var ms = new MemoryStream();
        certFile.CopyTo(ms);
        var cert = new X509Certificate2(ms.ToArray());

        var rootCert = new X509Certificate2("../truYou-ca/circle-root.crt");

        var chain = new X509Chain();
        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
        chain.ChainPolicy.CustomTrustStore.Add(rootCert);
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

        bool trusted = chain.Build(cert);
        bool emailMatches = cert.Subject.Contains($"CN={email}", StringComparison.OrdinalIgnoreCase);

        if (trusted && emailMatches)
        {
            HttpContext.Session.SetString("TruYouEmail", email);
            HttpContext.Session.Set("TruYouCert", cert.RawData);
            return RedirectToAction("StreamingHub", "Camera");
        }

        ViewBag.Error = "Invalid certificate or email does not match.";
        return View();
    }

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        HttpContext.Response.Cookies.Delete(".AspNetCore.Session");
        return Redirect("/login");
    }
}