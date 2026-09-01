namespace ForgeQueue.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ForgeQueue.Data;
using ForgeQueue.Models;

public class JobProcessorService : BackgroundService{

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly Dictionary<string, int> _jobDurationMs = new Dictionary<string, int>{
        {"IMAGE_PROCESSING", 8000},
        {"EMAIL_NOTIFICATION", 2000},
        {"MESSAGE_READING", 3000}
    };

    public JobProcessorService(IServiceScopeFactory scopeFactory){
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken){
        while(!stoppingToken.IsCancellationRequested){
            Console.WriteLine("JobProcessorService is running...");

            using(var scope = _scopeFactory.CreateScope()){
                var db = scope.ServiceProvider.GetRequiredService<ForgeQueueDbContext>();

                var queuedJobs = await db.Jobs.Where(j => j.Status == JobStatus.Queued).ToListAsync();

                foreach(var job in queuedJobs){
                    job.TryTransitionTo(JobStatus.Processing, out var errorMessage);

                    await db.SaveChangesAsync();

                    int duration;
                    if(!_jobDurationMs.TryGetValue(job.Type, out duration)){
                        duration = 3000;
                    }

                    await Task.Delay(duration, stoppingToken);

                    job.TryTransitionTo(JobStatus.Completed, out errorMessage);

                    await db.SaveChangesAsync();
                }
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}