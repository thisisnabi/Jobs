
using Hangfire;
using Microsoft.Extensions.Options;

namespace Jobs
{
    public class JobsRegisterHostedService(IOptions<AppSettings> options, IServiceScopeFactory scopeFactory) : IHostedService
    {
        private readonly JobSettings _jobSettings = options.Value.JobSettings;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            RecurringJob.AddOrUpdate<AlertSmsPanelJob>(
                AlertSmsPanelJob.JonName,
                JobQueues.Priority1,
                x => x.ExecuteAsync(),
                _jobSettings.AlertSmsPanelJobCron);
             
            GlobalConfiguration.Configuration.UseActivator(new HangfireJobActivator(scopeFactory));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
           return Task.CompletedTask;
        }
    }
}
