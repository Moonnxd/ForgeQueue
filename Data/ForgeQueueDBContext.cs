namespace ForgeQueue.Data;

using Microsoft.EntityFrameworkCore;
using ForgeQueue.Models;

public class ForgeQueueDbContext : DbContext {
    public ForgeQueueDbContext(DbContextOptions<ForgeQueueDbContext> options) : base(options){

    }

    public DbSet<Job> Jobs {get; set;}
}