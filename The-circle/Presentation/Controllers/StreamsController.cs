using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using The_circle.Application.Services;

namespace The_circle.Presentation.Controllers
{
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
            var activeIds = _buffer.GetActiveStreamIds(TimeSpan.FromSeconds(10));

            var streams = activeIds.Select(id =>
            {
                var email = "Onbekend";

                var frame = _buffer.GetFrameWithMetadata(id);
                if (frame?.Certificate != null)
                {
                    try
                    {
                        var cert = new X509Certificate2(frame.Certificate);
                        var cn = cert.GetNameInfo(X509NameType.SimpleName, false);
                        if (!string.IsNullOrEmpty(cn))
                            email = cn;
                    }
                    catch
                    {
                        Console.WriteLine($"[GetStreams] Error extracting CN for stream");
                    }
                }

                return new
                {
                    StreamId = id,
                    Email    = email
                };
            });

            return Ok(streams);
        }
    }
}