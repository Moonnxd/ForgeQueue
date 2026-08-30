namespace ForgeQueue.Models;

public class CreateJobRequest{
    public required string Type {get; set;}
    public required string Payload {get; set;}
}