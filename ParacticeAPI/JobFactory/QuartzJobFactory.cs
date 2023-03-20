using Quartz.Spi;
using Quartz;

public class QuartzJobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public QuartzJobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJob? NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var jobType = bundle.JobDetail.JobType;
        return _serviceProvider.GetService(jobType) as IJob;
    }

    public void ReturnJob(IJob job)
    {
    }
}
