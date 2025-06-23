using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using The_circle.Application.Services;

namespace The_circle.Presentation.Controllers
{
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly VideoFrameBufferService _buffer;

        public StreamController(VideoFrameBufferService buffer)
        {
            _buffer = buffer;
        }

        [HttpGet("/stream/{streamId}")]
        public async Task Stream(Guid streamId)
        {
            Response.ContentType = "text/event-stream";
            Response.Headers["Cache-Control"]     = "no-cache";
            Response.Headers["X-Accel-Buffering"] = "no";  // for Nginx

            int lastChunkIndex = -1;

            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                var frame = _buffer.GetFrameWithMetadata(streamId);
                if (frame != null && frame.ChunkIndex != lastChunkIndex)
                {
                    // ── Extract CN from the DER‐encoded cert in this frame
                    string email = "Onbekend";
                    try
                    {
                        var cert = new X509Certificate2(frame.Certificate);
                        var cn   = cert.GetNameInfo(X509NameType.SimpleName, false);
                        if (!string.IsNullOrEmpty(cn))
                            email = cn;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[SSE] CN‐extractie mislukt: {ex.Message}");
                    }

                    var ssePayload = new
                    {
                        chunk       = Convert.ToBase64String(frame.Chunk),
                        signature   = Convert.ToBase64String(frame.Signature),
                        certificate = Convert.ToBase64String(frame.Certificate),
                        chunkIndex  = frame.ChunkIndex,
                        email       = email
                    };

                    var json = JsonSerializer.Serialize(ssePayload);
                    await Response.WriteAsync($"data: {json}\n\n");
                    await Response.Body.FlushAsync();

                    lastChunkIndex = frame.ChunkIndex;
                }

                await Task.Delay(100);
            }

            Console.WriteLine($"[SSE] Verbinding voor stream {streamId} beëindigd.");
        }
    }
}
