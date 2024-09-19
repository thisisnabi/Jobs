using Hangfire;

namespace Jobs;

public class HangfireJobActivator(IServiceScopeFactory scopeFactory) : JobActivator
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;


    public override object ActivateJob(Type jobType)
    {
        var _scope = _scopeFactory.CreateScope();
        return _scope.ServiceProvider.GetRequiredService(jobType);
    }
}
