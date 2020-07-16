using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProcessData.Repositories.SSIS;

namespace ProcessData
{
    public class ContextoSSISDB : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<DtsxModel> Model { get; set; }

        public ContextoSSISDB(IConfiguration configuration)
        {
            this._configuration = configuration;
            Database.SetCommandTimeout(0);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectString = _configuration.GetConnectionString("SSISConnection");
            optionsBuilder.UseSqlServer(connectString);
            
        }
    }
}
