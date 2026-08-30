using ForgeQueue.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

List<Job> jobList = new List<Job>();

jobList.Add(new Job{
    Id = 1,
    Type = "IMAGE_PROCESSING",
    Payload = "image-001.png",
    Status = JobStatus.Queued,
    CreatedAt = DateTime.UtcNow
});

jobList.Add(new Job{
    Id = 2,
    Type = "EMAIL_NOTIFICATION",
    Payload = "user123@example.com",
    Status = JobStatus.Queued,
    CreatedAt = DateTime.UtcNow
});

app.MapGet("/api/jobs", () => {
    return Results.Ok(jobList);
});

app.MapPost("/api/jobs", (CreateJobRequest request) => {
    int nextId = jobList.Max(j => j.Id) + 1;

    var newJob = new Job {
        Id = nextId,
        Type = request.Type,
        Payload = request.Payload,
        Status = JobStatus.Queued,
        CreatedAt = DateTime.UtcNow
    };

    jobList.Add(newJob);

    return Results.Created($"/api/jobs/{newJob.Id}", newJob);
});

app.MapGet("/api/jobs/{id}", (int id) => {
    var job = jobList.FirstOrDefault(j => j.Id == id);

    if(job == null){
        return Results.NotFound();
    }else{
        return Results.Ok(job);
    }
});

app.Run();