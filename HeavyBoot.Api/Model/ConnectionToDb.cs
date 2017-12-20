using Microsoft.EntityFrameworkCore;

namespace HeavyBoot.Api.Model
{
    public class ConnectionToDb : DbContext
    {
        public DbSet<HBDataTable> HbDataTables { get; set; }
        public ConnectionToDb(DbContextOptions<ConnectionToDb> options)
            : base(options)
        {

        }
    }
}
