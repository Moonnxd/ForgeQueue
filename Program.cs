using ForgeQueue.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

app.Run();