using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbConn : DbContext
    {
        public AppDbConn(DbContextOptions options) : base(options){}

        public DbSet<TaskModel> task {  get; set; }
    }
}
