namespace The_circle.Application.Services;

public class VideoFrameBufferService
{
    private readonly object _lock = new();
    private string _lastFrameBase64 = "";
    private int _lastChunkIndex = -1;

    public void SetFrame(int chunkIndex, byte[] frame)
    {
        lock (_lock)
        {
            if (chunkIndex <= _lastChunkIndex)
                return; 

            _lastChunkIndex = chunkIndex;
            _lastFrameBase64 = Convert.ToBase64String(frame);
        }
    }

    public string GetFrame()
    {
        lock (_lock)
        {
            return _lastFrameBase64;
        }
    }
}

