using ForgeQueue.Models;
using ForgeQueue.Data;
using Microsoft.EntityFrameworkCore;
using ForgeQueue.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddHostedService<JobProcessorService>();

builder.Services.AddDbContext<ForgeQueueDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ForgeQueueDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/jobs", async (ForgeQueueDbContext db) => {
    var jobs = await db.Jobs.ToListAsync();
    return Results.Ok(jobs);
});

app.MapPost("/api/jobs", async (CreateJobRequest request ,ForgeQueueDbContext db) => {
    var newJob = new Job {
        Type = request.Type,
        Payload = request.Payload,
        Status = JobStatus.Queued,
        CreatedAt = DateTime.UtcNow
    };

    db.Jobs.Add(newJob);
    await db.SaveChangesAsync();

    return Results.Created($"/api/jobs/{newJob.Id}", newJob);
});

app.MapGet("/api/jobs/{id}", async (int id, ForgeQueueDbContext db) => {
    var job = await db.Jobs.FindAsync(id);

    if(job == null){
        return Results.NotFound();
    }else{
        return Results.Ok(job);
    }
});

app.MapPatch("/api/jobs/{id}/status", async (int id, UpdateJobStatusRequest request, ForgeQueueDbContext db) => {
    var job = await db.Jobs.FindAsync(id);

    if(job == null){
        return Results.NotFound();
    }

    bool isTransitioned = job.TryTransitionTo(request.NewStatus, out var errorMessage);

    if(!isTransitioned){
        return Results.Conflict(errorMessage);
    }else{
        await db.SaveChangesAsync();
        return Results.Ok(job);
    }
});

app.Run();