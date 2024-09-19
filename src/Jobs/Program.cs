using Hangfire;
using Hangfire.Redis.StackExchange;
using Jobs;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<AlertSmsPanelJob>();
builder.Services.AddHostedService<JobsRegisterHostedService>();

builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<VideoConverter>();

builder.Services.AddHangfire((sp, configure) =>
{
    var jobSettings = sp.GetRequiredService<IOptions<AppSettings>>().Value.JobSettings;

    configure.UseRedisStorage(jobSettings.RedisStorage.ConnectionString, jobSettings.RedisStorage)
                 .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseRecommendedSerializerSettings();
});

builder.Services.AddHangfireServer(options =>
{
    options.Queues = [
        JobQueues.Priority1,
        JobQueues.Priority2,
        ];
    options.WorkerCount = 10;
    options.StopTimeout = TimeSpan.FromHours(1);
});

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    StatsPollingInterval = 5000,
    DarkModeEnabled = true,
    DashboardTitle = "Payment service handfire dashboard"
});


app.MapPost("/upload", async (IFormFile videoFrom, VideoConverter videoConverter) =>
{

    if (videoFrom is null || videoFrom.Length == 0)
        return Results.BadRequest("No file uploaded!");

    string _videoDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "rawdata");

    if (!Directory.Exists(_videoDirectory))
        Directory.CreateDirectory(_videoDirectory);

    var filePath = Path.Combine(_videoDirectory, videoFrom.FileName);
    using var stream = new FileStream(filePath, FileMode.Create);
    await videoFrom.CopyToAsync(stream);
     
    var job1 = BackgroundJob.Enqueue(JobQueues.Priority1, () => videoConverter.ConvertTo(filePath, VideoSize.FullHD));
    var job2 = BackgroundJob.ContinueJobWith(job1, JobQueues.Priority1, () => videoConverter.ConvertTo(filePath, VideoSize.HD));
    BackgroundJob.ContinueJobWith(job2, JobQueues.Priority1, () => videoConverter.ConvertTo(filePath, VideoSize.FourHD));

    return Results.Ok("Video uploaded and processing started.");
}).DisableAntiforgery();


app.Run();

