using HashPasswordApi.Model;
using Microsoft.EntityFrameworkCore;
namespace HashPasswordApi.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Users> users { get; set; }
    }

}
