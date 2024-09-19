namespace Jobs;

public static class JobQueues
{
    public const string Priority1 = "high";
    public const string Priority2 = "low";


}


public class AlertSmsPanelJob(HttpClient httpClient,
    ILogger<AlertSmsPanelJob> logger,
    UserService userService)
{
    public const string JonName = "alert_sms_panel_job";

    private readonly HttpClient _httpClient = httpClient;
	private readonly ILogger<AlertSmsPanelJob> _logger= logger;
    private readonly UserService _userService = userService;

    public async Task ExecuteAsync()
    { 
        try
		{
            _logger.LogInformation("check sms panel balance");

            // _httpClient
            //
            //

            await Task.Delay(1);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "message!");
        }
    }
}
