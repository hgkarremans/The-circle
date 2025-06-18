using System.Net.Sockets;

namespace The_circle.Application.Services;

public class UdpVideoListenerService : BackgroundService
{
    private readonly VideoFrameBufferService _buffer;

    public UdpVideoListenerService(VideoFrameBufferService buffer)
    {
        _buffer = buffer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var udpClient = new UdpClient(9000);
        udpClient.EnableBroadcast = true;

        Console.WriteLine("[UDP Listener] Listening on UDP port 9000...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = await udpClient.ReceiveAsync();
                using var ms = new MemoryStream(result.Buffer);
                using var reader = new BinaryReader(ms);

                var streamIdBytes = reader.ReadBytes(16);
                var streamId = new Guid(streamIdBytes);
                var chunkIndex = reader.ReadInt32();
                var chunkSize = reader.ReadInt32();
                var chunk = reader.ReadBytes(chunkSize);

                _buffer.SetFrame(streamId, chunkIndex, chunk);

                Console.WriteLine($"[UDP Listener] Received chunk {chunkIndex} for stream {streamId}, size {chunkSize} bytes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UDP Listener] Error: {ex.Message}");
            }
        }
    }
}
