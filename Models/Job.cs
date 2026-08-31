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

    public bool TryTransitionTo(JobStatus newStatus, out string? errorMessage){
        bool isValid = false;

        switch(Status){
            case JobStatus.Queued:
            if(newStatus == JobStatus.Processing){
                isValid = true;
            }
            break;

            case JobStatus.Processing:
            if(newStatus == JobStatus.Completed || newStatus == JobStatus.Failed){
                isValid = true;
            }
            break;

            case JobStatus.Completed:
            isValid = false;
            break;

            case JobStatus.Failed:
            isValid = false;
            break;
        }

        if(!isValid){
            errorMessage = $"Cannot transition from {Status} to {newStatus}.";
            return false;
        }

        this.Status = newStatus;

        if(newStatus == JobStatus.Processing){
            StartedAt = DateTime.UtcNow;

        }else if(newStatus == JobStatus.Completed || newStatus == JobStatus.Failed){
            CompletedAt = DateTime.UtcNow;
        }

        errorMessage = null;
        return true;
    }
}