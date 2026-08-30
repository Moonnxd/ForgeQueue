namespace ForgeQueue.Models;

public enum JobStatus{
    Queued,
    Processing,
    Completed,
    Failed
}

public class Job{
    public int Id {get; set;}
    public required string Type {get; set;}
    public required string Payload {get; set;}
    public JobStatus Status {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime? StartedAt {get; set;}
    public DateTime? CompletedAt {get; set;}
}