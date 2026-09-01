namespace ForgeQueue.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ForgeQueue.Data;
using ForgeQueue.Models;

public class JobProcessorService : BackgroundService{

    private readonly IServiceScopeFactory _scopeFactory;

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

                    await Task.Delay(5000, stoppingToken);

                    job.TryTransitionTo(JobStatus.Completed, out errorMessage);

                    await db.SaveChangesAsync();
                }
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}