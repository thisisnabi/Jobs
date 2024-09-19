namespace Jobs;

public class VideoConverter(ILogger<VideoConverter> logger)
{
    private readonly ILogger<VideoConverter> _logger = logger;
 
    public async Task ConvertTo(string filePath, VideoSize videoSize)
    {
        await Task.Delay(1000);
        _logger.LogInformation("Start processing on: {videoSize}", videoSize);
    }
}


public enum VideoSize
{ 
    HD = 1,
    FullHD = 2,
    FourHD = 3
}